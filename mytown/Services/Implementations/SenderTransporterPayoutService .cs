using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Enums;
using mytown.Helpers;
using mytown.Models;
using System.Text;
using System.Text.Json;

public class SenderTransporterPayoutService : ISenderTransporterPayoutService
{
    private readonly ISenderTransporterPayoutRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SenderTransporterPayoutService(
        ISenderTransporterPayoutRepository repository,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _repository = repository;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TriggerPayoutResponseDto> CreatePayoutAsync(int senderOrderId)
    {
        // 1. Prevent duplicate payout
        var existingPayout = await _repository.GetPayoutBySenderOrderIdAsync(senderOrderId);

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
                Message = "Payout already exists for this sender order."
            };
        }

        // 2. Get transporter + beneficiary + amount
        var payoutDetails = await _repository.GetTransporterPayoutDetailsAsync(senderOrderId);

        if (payoutDetails == null)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Transporter payout details not found."
            };
        }

        if (string.IsNullOrWhiteSpace(payoutDetails.BeneficiaryId))
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Cashfree beneficiary is not configured for this transporter."
            };
        }

        if (payoutDetails.Amount <= 0)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Invalid transporter payout amount."
            };
        }

        // 3. Unique transfer ID (distinct prefix from store orders)
        var transferId = $"MYTOWN_TRANSPORTER_SENDERORDER_{senderOrderId}";

        // 4. Create payout record BEFORE calling Cashfree
        var payout = new TransporterPayout
        {
            SenderOrderId = senderOrderId,
            StoreOrderId = null,
            OrderType = "SenderOrder",
            TransporterRegId = payoutDetails.TransporterRegId,
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
            var signatureClientId = _configuration["CashfreePayout:SignatureClientId"] ?? clientId;

            var payload = new
            {
                transfer_id = transferId,
                transfer_amount = payoutDetails.Amount,
                transfer_mode = "banktransfer",
                transfer_remarks = $"Transporter payout for Sender Order {senderOrderId}",
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
            request.Headers.Add("x-cf-signature",
                CashfreeSignatureHelper.GenerateSignature(signatureClientId, _configuration["CashfreePayoutPublicKey"]));

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
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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