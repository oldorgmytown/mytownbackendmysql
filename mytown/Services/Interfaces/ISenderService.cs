using mytown.DTOs;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ISenderService
    {
        Task<(bool success, string message)> RegisterSenderAsync(SenderRegisterDto dto);
        Task<(bool success, string message, int? senderRegId)> VerifyEmailAsync(string token);
        Task<(bool success, string message)> ResendVerificationEmailAsync(string email);

        // Sender Orders

        Task<int> CreateSenderOrderAsync(CreateSenderOrderDto dto);
        Task<MatchingTransporterDto>
    GetMatchingTransportersAsync(int senderOrderId);

        Task<SenderOrderSummaryDto>
    GetOrderSummaryAsync(
        SenderOrderSummaryRequestDto dto);

        // sender payments

        Task<SenderPaymentIntentResponseDto>
            CreatePaymentIntentAsync(
                int senderOrderId);

        Task<bool>
            ConfirmPaymentAsync(
                ConfirmSenderPaymentDto dto);

        //sender order confirmation

        Task<SenderOrderConfirmationDto>
    GetOrderConfirmationAsync(
        int senderOrderId);

        // sender package delivery status
        Task<bool> UpdateSenderPackageDeliveryStatusAsync(
    UpdateSenderPackageDeliveryStatusDto dto);


        Task<List<SenderOrdersTabDto>>
      GetSenderOrdersAsync(
          int senderId,
          string orderType);
    } 

}