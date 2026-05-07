using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface ISenderRepository
    {
        Task<(bool isTaken, string message)> IsEmailTaken(string email);

        Task SavePendingSenderVerification(PendingSenderVerification pending);
        Task<PendingSenderVerification> FindPendingSenderVerificationByToken(string token);
        Task<PendingSenderVerification> FindPendingSenderVerificationByEmail(string email);
        Task DeletePendingSenderVerification(string token);

        Task<SenderRegister> RegisterSender(SenderRegister sender);

        Task<SenderRegister> GetSenderByIdAsync(int senderRegId);

        // Sender Orders

        Task<int> CreateSenderOrderAsync(CreateSenderOrderDto dto);

        Task<List<MatchingTransporterDto>>
   GetMatchingTransportersAsync(int senderOrderId);

        Task<SenderOrderSummaryDto>
    GetOrderSummaryAsync(
        SenderOrderSummaryRequestDto dto);



        Task<bool> SelectTransporterAsync(
    SelectTransporterDto dto);

        // sender payment

        Task<SenderOrder>
           GetSenderOrderAsync(int senderOrderId);

        Task AddSenderOrderPaymentAsync(
            SenderOrderPayment payment);

        Task SaveChangesAsync();

        //sender order confirmation

        Task<SenderOrderConfirmationDto>
    GetOrderConfirmationAsync(
        int senderOrderId);

        // sender package delivery status 
        Task<bool> UpdateSenderPackageDeliveryStatusAsync(
    UpdateSenderPackageDeliveryStatusDto dto);

        // update notifcations

        Task AddSenderNotificationAsync(
    SenderDBNotifications notification);

        Task AddTransporterNotificationAsync(
            TransporterDBNotifications notification);

    }
}