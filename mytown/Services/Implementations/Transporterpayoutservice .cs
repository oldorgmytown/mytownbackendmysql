using mytown.DTOs;
using mytown.Enums;
using mytown.Helpers;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text;
using System.Text.Json;

public class TransporterPayoutService : ITransporterPayoutService
{
    private readonly ITransporterPayoutRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TransporterPayoutService(
        ITransporterPayoutRepository repository,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _repository = repository;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    //public async Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId)
    //{
    //    // 1. Prevent duplicate payout
    //    var existingPayout =
    //        await _repository.GetPayoutByStoreOrderIdAsync(storeOrderId);

    //    if (existingPayout != null)
    //    {
    //        return new TriggerPayoutResponseDto
    //        {
    //            Success = true,
    //            TransferId = existingPayout.TransferId,
    //            CfTransferId = existingPayout.CfTransferId,
    //            Status = existingPayout.Status,
    //            TransferAmount = existingPayout.Amount,
    //            TransferUtr = existingPayout.TransferUtr,
    //            Message = "Payout already exists for this store order."
    //        };
    //    }

    //    // 2. Get transporter + beneficiary + amount
    //    var payoutDetails =
    //        await _repository.GetTransporterPayoutDetailsAsync(storeOrderId);

    //    if (payoutDetails == null)
    //    {
    //        return new TriggerPayoutResponseDto
    //        {
    //            Success = false,
    //            Message = "Transporter payout details not found."
    //        };
    //    }

    //    // 3. Check beneficiary
    //    if (string.IsNullOrWhiteSpace(payoutDetails.BeneficiaryId))
    //    {
    //        return new TriggerPayoutResponseDto
    //        {
    //            Success = false,
    //            Message = "Cashfree beneficiary is not configured for this transporter."
    //        };
    //    }

    //    // 4. Check amount
    //    if (payoutDetails.Amount <= 0)
    //    {
    //        return new TriggerPayoutResponseDto
    //        {
    //            Success = false,
    //            Message = "Invalid transporter payout amount."
    //        };
    //    }

    //    // 5. Create unique transfer ID
    //    var transferId = $"MYTOWN_TRANSPORTER_ORDER_{storeOrderId}";

    //    // 6. Create payout record BEFORE calling Cashfree
    //    var payout = new TransporterPayout
    //    {
    //        StoreOrderId = storeOrderId,
    //        SenderOrderId = null,
    //        OrderType = "StoreOrder",
    //        TransporterRegId = payoutDetails.TransporterRegId,
    //        BeneficiaryId = payoutDetails.BeneficiaryId,
    //        Amount = payoutDetails.Amount,
    //        TransferId = transferId,
    //        Status = StorePayoutStatus.Pending,
    //        CreatedDate = DateTime.UtcNow
    //    };

    //    await _repository.AddPayoutAsync(payout);
    //    await _repository.SaveAsync();

    //    try
    //    {
    //        // 7. Cashfree credentials
    //        var clientId = _configuration["CashfreePayout:ClientId"];
    //        var clientSecret = _configuration["CashfreePayout:ClientSecret"];
    //        var baseUrl = _configuration["CashfreePayout:BaseUrl"];
    //        var apiVersion = _configuration["CashfreePayout:ApiVersion"];
    //     //   var signatureClientId = _configuration["CashfreePayout:SignatureClientId"] ?? clientId;

    //        // 8. Cashfree payout request
    //        var payload = new
    //        {
    //            transfer_id = transferId,
    //            transfer_amount = payoutDetails.Amount,
    //            transfer_mode = "banktransfer",
    //            transfer_remarks =
    //                $"Transporter payout for Store Order {storeOrderId}",
    //            beneficiary_details = new
    //            {
    //                beneficiary_id = payoutDetails.BeneficiaryId
    //            }
    //        };

    //        var json = JsonSerializer.Serialize(payload);

    //        using var request = new HttpRequestMessage(
    //            HttpMethod.Post,
    //            $"{baseUrl}/transfers");

    //        request.Headers.Add("x-client-id", clientId);
    //        request.Headers.Add("x-client-secret", clientSecret);
    //        request.Headers.Add("x-api-version", apiVersion);

    //        request.Headers.Add(
    //            "x-cf-signature",
    //            CashfreeSignatureHelper.GenerateSignature(clientId, _configuration["CashfreePayoutPublicKey"]));

    //        request.Content = new StringContent(
    //            json,
    //            Encoding.UTF8,
    //            "application/json");

    //        // 9. ACTUAL CASHFREE PAYOUT API HIT
    //        var response = await _httpClient.SendAsync(request);

    //        var responseContent =
    //            await response.Content.ReadAsStringAsync();

    //        // 10. Cashfree failed
    //        if (!response.IsSuccessStatusCode)
    //        {
    //            payout.Status = StorePayoutStatus.Failed;
    //            payout.StatusDescription = responseContent;
    //            payout.UpdatedDate = DateTime.UtcNow;

    //            await _repository.SaveAsync();

    //            return new TriggerPayoutResponseDto
    //            {
    //                Success = false,
    //                TransferId = transferId,
    //                Status = StorePayoutStatus.Failed,
    //                Message = responseContent
    //            };
    //        }

    //        // 11. Parse Cashfree response
    //        var cashfreeResponse =
    //            JsonSerializer.Deserialize<CashfreePayoutApiResponse>(
    //                responseContent,
    //                new JsonSerializerOptions
    //                {
    //                    PropertyNameCaseInsensitive = true
    //                });

    //        if (cashfreeResponse == null)
    //        {
    //            payout.Status = StorePayoutStatus.Failed;
    //            payout.StatusDescription =
    //                "Invalid response from Cashfree.";
    //            payout.UpdatedDate = DateTime.UtcNow;

    //            await _repository.SaveAsync();

    //            return new TriggerPayoutResponseDto
    //            {
    //                Success = false,
    //                TransferId = transferId,
    //                Status = StorePayoutStatus.Failed,
    //                Message = "Invalid response from Cashfree."
    //            };
    //        }

    //        // 12. Save Cashfree response
    //        payout.Status =
    //            cashfreeResponse.Status ?? StorePayoutStatus.Received;

    //        payout.CfTransferId =
    //            cashfreeResponse.CfTransferId;

    //        payout.TransferUtr =
    //            cashfreeResponse.TransferUtr;

    //        payout.StatusDescription =
    //            cashfreeResponse.StatusDescription;

    //        payout.UpdatedDate = DateTime.UtcNow;

    //        await _repository.SaveAsync();

    //        // 13. Return response
    //        return new TriggerPayoutResponseDto
    //        {
    //            Success = true,
    //            TransferId = cashfreeResponse.TransferId,
    //            CfTransferId = cashfreeResponse.CfTransferId,
    //            Status = cashfreeResponse.Status,
    //            StatusCode = cashfreeResponse.StatusCode,
    //            StatusDescription =
    //                cashfreeResponse.StatusDescription,
    //            TransferAmount =
    //                cashfreeResponse.TransferAmount,
    //            TransferMode =
    //                cashfreeResponse.TransferMode,
    //            TransferUtr =
    //                cashfreeResponse.TransferUtr,
    //            Message =
    //                cashfreeResponse.StatusDescription
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        payout.Status = StorePayoutStatus.Failed;
    //        payout.StatusDescription = ex.Message;
    //        payout.UpdatedDate = DateTime.UtcNow;

    //        await _repository.SaveAsync();

    //        return new TriggerPayoutResponseDto
    //        {
    //            Success = false,
    //            TransferId = transferId,
    //            Status = StorePayoutStatus.Failed,
    //            Message = ex.Message
    //        };
    //    }
    //}

    public async Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId)
    {
        var legs = await _repository.GetLegsByStoreOrderIdAsync(storeOrderId);

        // old order with no legs: exactly the old behaviour
        if (legs.Count == 0)
            return await PayOldOrderAsync(storeOrderId);

        var payableLegs = legs
            .Where(l => !l.IsPaid
                     && l.LegStatus == "Delivered"
                     && (l.IsFinalLeg || l.HubStatus == "Reached hub" || l.HubStatus == "Handed over"))
            .ToList();

        if (payableLegs.Count == 0)
        {
            return new TriggerPayoutResponseDto
            {
                Success = true,
                Message = "No transporter payout is due yet."
            };
        }

        var messages = new List<string>();
        bool allSucceeded = true;
        TriggerPayoutResponseDto? last = null;

        foreach (var leg in payableLegs)
        {
            var result = await PayLegAsync(storeOrderId, leg);
            last = result;

            if (!result.Success) allSucceeded = false;
            messages.Add($"Leg {leg.LegNumber}: {result.Message}");
        }

        last!.Success = allSucceeded;
        last.Message = string.Join(" | ", messages);
        return last;
    }

    // ── one payout for one leg ──
    private async Task<TriggerPayoutResponseDto> PayLegAsync(int storeOrderId, OrderTransportLeg leg)
    {
        // 1. Prevent duplicate payout for this leg
        var existing = await _repository.GetPayoutByLegIdAsync(leg.OrderTransportLegId);

        if (existing != null)
        {
            bool failedBefore = existing.Status == StorePayoutStatus.Failed;

            if (!failedBefore)
            {
                leg.IsPaid = true;
                leg.PaidAt ??= DateTime.UtcNow;
                await _repository.SaveAsync();
            }

            return new TriggerPayoutResponseDto
            {
                Success = !failedBefore,
                TransferId = existing.TransferId,
                CfTransferId = existing.CfTransferId,
                Status = existing.Status,
                TransferAmount = existing.Amount,
                TransferUtr = existing.TransferUtr,
                Message = failedBefore
                    ? "An earlier payout attempt for this leg failed."
                    : "Payout already exists for this leg."
            };
        }

        // 2. Beneficiary and amount
        var beneficiaryId = await _repository.GetBeneficiaryIdAsync(leg.TransporterRegId);

        if (string.IsNullOrWhiteSpace(beneficiaryId))
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Cashfree beneficiary is not configured for this transporter."
            };
        }

        if (leg.LegCost <= 0)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Invalid transporter payout amount."
            };
        }

        // 3. Unique transfer id per leg
        var transferId = $"MYTOWN_TRANSPORTER_ORDER_{storeOrderId}_LEG_{leg.LegNumber}";

        // 4. Create payout record BEFORE calling Cashfree
        var payout = new TransporterPayout
        {
            StoreOrderId = storeOrderId,
            SenderOrderId = null,
            OrderType = "StoreOrder",
            OrderTransportLegId = leg.OrderTransportLegId,
            TransporterRegId = leg.TransporterRegId,
            BeneficiaryId = beneficiaryId,
            Amount = leg.LegCost,
            TransferId = transferId,
            Status = StorePayoutStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };

        await _repository.AddPayoutAsync(payout);
        await _repository.SaveAsync();

        // 5. Pay through Cashfree
        var response = await SendToCashfreeAsync(
            payout, transferId, leg.LegCost, beneficiaryId,
            $"Transporter payout for Store Order {storeOrderId} leg {leg.LegNumber}");

        // 6. Mark the leg paid only when Cashfree accepted it
        if (response.Success)
        {
            leg.IsPaid = true;
            leg.PaidAt = DateTime.UtcNow;
            await _repository.SaveAsync();
        }

        return response;
    }

    // ── old orders (no legs): your original logic, unchanged ──
    private async Task<TriggerPayoutResponseDto> PayOldOrderAsync(int storeOrderId)
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

        // 2. Get transporter + beneficiary + amount
        var payoutDetails =
            await _repository.GetTransporterPayoutDetailsAsync(storeOrderId);

        if (payoutDetails == null)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Transporter payout details not found."
            };
        }

        // 3. Check beneficiary
        if (string.IsNullOrWhiteSpace(payoutDetails.BeneficiaryId))
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Cashfree beneficiary is not configured for this transporter."
            };
        }

        // 4. Check amount
        if (payoutDetails.Amount <= 0)
        {
            return new TriggerPayoutResponseDto
            {
                Success = false,
                Message = "Invalid transporter payout amount."
            };
        }

        // 5. Create unique transfer ID
        var transferId = $"MYTOWN_TRANSPORTER_ORDER_{storeOrderId}";

        // 6. Create payout record BEFORE calling Cashfree
        var payout = new TransporterPayout
        {
            StoreOrderId = storeOrderId,
            SenderOrderId = null,
            OrderType = "StoreOrder",
            TransporterRegId = payoutDetails.TransporterRegId,
            BeneficiaryId = payoutDetails.BeneficiaryId,
            Amount = payoutDetails.Amount,
            TransferId = transferId,
            Status = StorePayoutStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };

        await _repository.AddPayoutAsync(payout);
        await _repository.SaveAsync();

        return await SendToCashfreeAsync(
            payout, transferId, payoutDetails.Amount, payoutDetails.BeneficiaryId,
            $"Transporter payout for Store Order {storeOrderId}");
    }

    // ── Cashfree call (your original steps 7 to 13), shared by both paths ──
    private async Task<TriggerPayoutResponseDto> SendToCashfreeAsync(
        TransporterPayout payout, string transferId, decimal amount, string beneficiaryId, string remarks)
    {
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
                transfer_amount = amount,
                transfer_mode = "banktransfer",
                transfer_remarks = remarks,
                beneficiary_details = new
                {
                    beneficiary_id = beneficiaryId
                }
            };

            var json = JsonSerializer.Serialize(payload);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{baseUrl}/transfers");

            request.Headers.Add("x-client-id", clientId);
            request.Headers.Add("x-client-secret", clientSecret);
            request.Headers.Add("x-api-version", apiVersion);

            request.Headers.Add(
                "x-cf-signature",
                CashfreeSignatureHelper.GenerateSignature(clientId, _configuration["CashfreePayoutPublicKey"]));

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

            // 12. Save Cashfree response
            payout.Status = cashfreeResponse.Status ?? StorePayoutStatus.Received;
            payout.CfTransferId = cashfreeResponse.CfTransferId;
            payout.TransferUtr = cashfreeResponse.TransferUtr;
            payout.StatusDescription = cashfreeResponse.StatusDescription;
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
    