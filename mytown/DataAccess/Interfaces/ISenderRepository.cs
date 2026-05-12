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

        Task<MatchingTransporterDto>
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
        Task<TransporterEmailDto> GetTransporterByIdAsync(
    int transporterId);

        Task<List<SenderOrdersTabDto>>
GetSenderOrdersAsync(int senderId, string orderType);

        Task<SenderRegisterDto?> GetSenderProfileAsync(int senderRegId);

        Task<bool> UpdateSenderProfileAsync(
    int senderRegId,
    UpdateSenderProfileDto dto);

        Task<List<SenderDBNotifications>> GetUnreadNotificationsAsync(int senderId);

        Task MarkAllAsReadAsync(int senderId);

        Task MarkEachNotificationReadAsync(int notificationId);

    }
}