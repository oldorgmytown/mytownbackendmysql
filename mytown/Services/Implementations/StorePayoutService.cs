using System.Text;
using System.Text.Json;
using mytown.DTOs;
using mytown.Enums;
using mytown.Models;
using mytown.Models.DTO_s;

public class StorePayoutService : IStorePayoutService
{
    private readonly IStorePayoutRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public StorePayoutService(IStorePayoutRepository repository, HttpClient httpClient, IConfiguration configuration)
    {
        _repository = repository;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId)
    {
        var existingPayout = await _repository.GetPayoutByStoreOrderIdAsync(storeOrderId);

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

        var payoutDetails = await _repository.GetStorePayoutDetailsAsync(storeOrderId);

        if (payoutDetails == null)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Store order not found."
            };
        }

        if (string.IsNullOrWhiteSpace(payoutDetails.BeneficiaryId))
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Cashfree beneficiary is not configured for this store."
            };
        }

        if (payoutDetails.Amount <= 0)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Invalid store payout amount."
            };
        }

        var transferId = $"MYTOWN_STORE_ORDER_{storeOrderId}";

        var payout = new StorePayout
        {
            StoreOrderId = storeOrderId,
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
            var clientId = _configuration["CashfreePayout:ClientId"];
            var clientSecret = _configuration["CashfreePayout:ClientSecret"];
            var baseUrl = _configuration["CashfreePayout:BaseUrl"];
            var apiVersion = _configuration["CashfreePayout:ApiVersion"];

            var payload = new
            {
                transfer_id = transferId,
                transfer_amount = payoutDetails.Amount,
                transfer_mode = "banktransfer",
                transfer_remarks = $"Store payout for Store Order {storeOrderId}",
                beneficiary_details = new
                {
                    beneficiary_id = payoutDetails.BeneficiaryId
                }
            };

            var json = JsonSerializer.Serialize(payload);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/transfers");

            request.Headers.Add("x-client-id", clientId);
            request.Headers.Add("x-client-secret", clientSecret);
            request.Headers.Add("x-api-version", apiVersion);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

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

            var cashfreeResponse = JsonSerializer.Deserialize<CashfreePayoutApiResponse>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (cashfreeResponse == null)
            {
                payout.Status = StorePayoutStatus.Failed;
                payout.StatusDescription = "Invalid response from Cashfree.";
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

            payout.Status = cashfreeResponse.Status ?? StorePayoutStatus.Received;
            payout.CfTransferId = cashfreeResponse.CfTransferId;
            payout.TransferUtr = cashfreeResponse.TransferUtr;
            payout.StatusDescription = cashfreeResponse.StatusDescription;
            payout.UpdatedDate = DateTime.UtcNow;

            await _repository.SaveAsync();

            return new TriggerPayoutResponseDto
            {
                Success = true,
                TransferId = cashfreeResponse.TransferId,
                CfTransferId = cashfreeResponse.CfTransferId,
                Status = cashfreeResponse.Status,
                StatusCode = cashfreeResponse.StatusCode,
                StatusDescription = cashfreeResponse.StatusDescription,
                TransferAmount = cashfreeResponse.TransferAmount,
                TransferMode = cashfreeResponse.TransferMode,
                TransferUtr = cashfreeResponse.TransferUtr,
                Message = cashfreeResponse.StatusDescription
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