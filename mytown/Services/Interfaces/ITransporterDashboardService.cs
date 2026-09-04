using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ITransporterDashboardService
    {
        Task<TransporterDashboardDto> GetDashboardSummaryAsync(int transporterRegId);

        Task<TravelPlanDto?> GetActivePlanAsync(int transporterRegId);
        Task<TravelPlanDto> SaveTravelPlanAsync(TravelPlanDto dto);
        Task<bool> DeactivatePlanAsync(int planId, int transporterRegId);

        Task<List<AvailableTransporterDto>>
 SearchAvailableTransportersAsync(
     string startTown,
     string startCity,
     string startState,
     string startCountry,

     string destinationTown,
     string destinationCity,
     string destinationState,
     string destinationCountry);

        // Shopper creates request → auto-assigned to transporter linked via PlanId
        Task<(bool success, string message, int deliveryReqId)> CreateDeliveryRequestAsync(ShopperDeliveryRequestDto dto);

        // Active = Assigned + ReachedPickup + PickedUp + InTransit
        Task<List<ActiveDeliveryDto>> GetActiveDeliveryAsync(int transporterRegId);

        // Transporter updates status: ReachedPickup → PickedUp → InTransit → Delivered
        Task<bool> UpdateDeliveryStatusAsync(UpdateDeliveryStatusDto dto);

        Task<List<ActiveDeliveryDto>> GetCompletedDeliveriesAsync(int transporterRegId);
        Task<List<TravelPlanDto>> GetAllPlansAsync(int transporterRegId);

        Task<bool> SubmitExceptionReportAsync(ExceptionReportDto dto);

        Task<(bool success, string message)> SubmitKycAsync(TransporterKycDto dto);
        Task<(bool success, string message)> SubmitBankDetailsAsync(TransporterBankDto dto);

        Task<TransporterProfileDto?> GetProfileAsync(int transporterRegId);
        Task<bool> UpdateProfileAsync(UpdateTransporterProfileDto dto);
        Task<bool> UpdatePasswordAsync(int transporterRegId, string currentPassword, string newPassword);

        Task<List<TransporterDBNotifications>> GetUnreadNotificationsAsync(int transporterId);
        Task MarkAsReadAsync(int transporterId);
        Task MarkEachNotificationReadAsync(int notificationId);

        //package delivered

        Task<string> MarkAsDeliveredAsync(int storeOrderId);

        // sender ordres
        Task<List<SenderOrder>> GetTransporterDeliversSendersOrdersAsync(int transporterRegId);

        // update deliverystatus - sender orders
        // Service

        // Service Interface
        Task<bool> UpdateTransporterDeliveryStatusAsync(
            int senderOrderId,
            int transporterRegId,
            string deliveryStatus);
       
    }
}