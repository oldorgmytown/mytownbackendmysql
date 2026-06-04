using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface ITransporterDashboardRepository
    {
        // ---- Dashboard Summary ----
        Task<TransporterDashboardDto> GetDashboardSummaryAsync(int transporterRegId);

        // ---- Travel Plan ----
        Task<TravelPlanDto?> GetActivePlanAsync(int transporterRegId);
        Task<List<TravelPlanDto>> GetAllPlansAsync(int transporterRegId);
        Task<TravelPlanDto> SaveTravelPlanAsync(TravelPlanDto dto);
        Task<bool> DeactivatePlanAsync(int planId, int transporterRegId);

        // ---- Search available transporters (for shoppers) ----
        Task<List<AvailableTransporterDto>> SearchAvailableTransportersAsync(
      string startTown,
      string startCity,
      string startState,
      string startCountry,
      string destinationTown,
      string destinationCity,
      string destinationState,
      string destinationCountry);

        // ---- Delivery Requests ----
        // Creates request and auto-assigns (status = "Assigned") — no Accept step
        Task<TransporterDeliveryRequest> CreateDeliveryRequestAsync(ShopperDeliveryRequestDto dto);

        // Active deliveries = Assigned + ReachedPickup + PickedUp + InTransit
        Task<List<ActiveDeliveryDto>> GetActiveDeliveryAsync(int transporterRegId);

        // Status flow: ReachedPickup → PickedUp → InTransit → Delivered
        Task<bool> UpdateDeliveryStatusAsync(UpdateDeliveryStatusDto dto);

        Task<List<ActiveDeliveryDto>> GetCompletedDeliveriesAsync(int transporterRegId);

        // ---- Exception Reports ----
        Task<bool> SubmitExceptionReportAsync(ExceptionReportDto dto);

        // ---- KYC ----
        Task<TransporterKYC?> GetKycAsync(int transporterRegId);
        Task<TransporterKYC> SubmitKycAsync(int transporterRegId, string docType, string docNumber, string fileName);

        // ---- Bank Details ----
        Task<TransporterBankDetails?> GetBankDetailsAsync(int transporterRegId);
        Task<TransporterBankDetails> SubmitBankDetailsAsync(TransporterBankDto dto);

        // ---- Profile ----
        Task<TransporterProfileDto?> GetProfileAsync(int transporterRegId);
        Task<bool> UpdateProfileAsync(UpdateTransporterProfileDto dto);
        Task<bool> UpdatePasswordAsync(int transporterRegId, string newHashedPassword);

        // ---- Notifications ----
        Task<List<TransporterDBNotifications>> GetUnreadNotificationsAsync(int transporterId);
        Task MarkAllAsReadAsync(int transporterId);
        Task MarkEachNotificationReadAsync(int notificationId);

        // mark as delivered
        Task<string> MarkAsDeliveredAsync(int storeOrderId);

        //sender orders
        // Interface
        Task<List<SenderOrder>> GetTransporterDeliversSendersOrdersAsync(int transporterRegId);

        // update sender order status to delivered
        Task<bool> UpdateTransporterDeliveryStatusAsync(
          int senderOrderId,
          int transporterRegId,
          string deliveryStatus);
    }
}