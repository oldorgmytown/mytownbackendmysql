using mytown.DTOs;
using mytown.Enums;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text;
using System.Text.Json;

public class CourierPayoutService : ICourierPayoutService
{
    private readonly ICourierPayoutRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CourierPayoutService(
        ICourierPayoutRepository repository,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _repository = repository;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId)
    {
        // 1. Prevent duplicate payout
        var existingPayout =
            await _repository.GetPayoutByStoreOrderIdAsync(storeOrderId);

        if (existingPayout != null)
        {
            return new TriggerPayoutResponseDto
            {
                Success = true,
                TransferId = existingPayout.TransferId,
                CfTransferId = existingPayout.CfTransferId,
                Status = existingPayout.Status,
                TransferAmount = existingPayout.Amount,
                TransferUtr = existingPayout.TransferUtr,
                Message = "Payout already exists for this store order."
            };
        }

        // 2. Get courier + beneficiary + amount
        var payoutDetails =
            await _repository.GetCourierPayoutDetailsAsync(storeOrderId);

        if (payoutDetails == null)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Courier payout details not found."
            };
        }

        // 3. Check beneficiary
        if (string.IsNullOrWhiteSpace(payoutDetails.BeneficiaryId))
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Cashfree beneficiary is not configured for this courier."
            };
        }

        // 4. Check amount
        if (payoutDetails.Amount <= 0)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Invalid courier payout amount."
            };
        }

        // 5. Create unique transfer ID
        var transferId = $"MYTOWN_COURIER_ORDER_{storeOrderId}";

        // 6. Create payout record BEFORE calling Cashfree
        var payout = new CourierPayout
        {
            StoreOrderId = storeOrderId,
            CourierId = payoutDetails.CourierId,
            BeneficiaryId = payoutDetails.BeneficiaryId,
            Amount = payoutDetails.Amount,
            TransferId = transferId,
            Status = StorePayoutStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };

        await _repository.AddPayoutAsync(payout);
        await _repository.SaveAsync();

        try
        {
            // 7. Cashfree credentials
            var clientId = _configuration["CashfreePayout:ClientId"];
            var clientSecret = _configuration["CashfreePayout:ClientSecret"];
            var baseUrl = _configuration["CashfreePayout:BaseUrl"];
            var apiVersion = _configuration["CashfreePayout:ApiVersion"];

            // 8. Cashfree payout request
            var payload = new
            {
                transfer_id = transferId,
                transfer_amount = payoutDetails.Amount,
                transfer_mode = "banktransfer",
                transfer_remarks =
                    $"Courier payout for Store Order {storeOrderId}",
                beneficiary_details = new
                {
                    beneficiary_id = payoutDetails.BeneficiaryId
                }
            };

            var json = JsonSerializer.Serialize(payload);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{baseUrl}/transfers");

            request.Headers.Add("x-client-id", clientId);
            request.Headers.Add("x-client-secret", clientSecret);
            request.Headers.Add("x-api-version", apiVersion);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            // 9. ACTUAL CASHFREE PAYOUT API HIT
            var response = await _httpClient.SendAsync(request);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            // 10. Cashfree failed
            if (!response.IsSuccessStatusCode)
            {
                payout.Status = StorePayoutStatus.Failed;
                payout.StatusDescription = responseContent;
                payout.UpdatedDate = DateTime.UtcNow;

                await _repository.SaveAsync();

                return new TriggerPayoutResponseDto
                {
                    Success = false,
                    TransferId = transferId,
                    Status = StorePayoutStatus.Failed,
                    Message = responseContent
                };
            }

            // 11. Parse Cashfree response
            var cashfreeResponse =
                JsonSerializer.Deserialize<CashfreePayoutApiResponse>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (cashfreeResponse == null)
            {
                payout.Status = StorePayoutStatus.Failed;
                payout.StatusDescription =
                    "Invalid response from Cashfree.";
                payout.UpdatedDate = DateTime.UtcNow;

                await _repository.SaveAsync();

                return new TriggerPayoutResponseDto
                {
                    Success = false,
                    TransferId = transferId,
                    Status = StorePayoutStatus.Failed,
                    Message = "Invalid response from Cashfree."
                };
            }

            // 12. Save Cashfree response
            payout.Status =
                cashfreeResponse.Status ?? StorePayoutStatus.Received;

            payout.CfTransferId =
                cashfreeResponse.CfTransferId;

            payout.TransferUtr =
                cashfreeResponse.TransferUtr;

            payout.StatusDescription =
                cashfreeResponse.StatusDescription;

            payout.UpdatedDate = DateTime.UtcNow;

            await _repository.SaveAsync();

            // 13. Return response
            return new TriggerPayoutResponseDto
            {
                Success = true,
                TransferId = cashfreeResponse.TransferId,
                CfTransferId = cashfreeResponse.CfTransferId,
                Status = cashfreeResponse.Status,
                StatusCode = cashfreeResponse.StatusCode,
                StatusDescription =
                    cashfreeResponse.StatusDescription,
                TransferAmount =
                    cashfreeResponse.TransferAmount,
                TransferMode =
                    cashfreeResponse.TransferMode,
                TransferUtr =
                    cashfreeResponse.TransferUtr,
                Message =
                    cashfreeResponse.StatusDescription
            };
        }
        catch (Exception ex)
        {
            payout.Status = StorePayoutStatus.Failed;
            payout.StatusDescription = ex.Message;
            payout.UpdatedDate = DateTime.UtcNow;

            await _repository.SaveAsync();

            return new TriggerPayoutResponseDto
            {
                Success = false,
                TransferId = transferId,
                Status = StorePayoutStatus.Failed,
                Message = ex.Message
            };
        }
    }
}