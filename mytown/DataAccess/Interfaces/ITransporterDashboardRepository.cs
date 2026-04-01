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
        Task<TravelPlanDto> SaveTravelPlanAsync(TravelPlanDto dto);
        Task<bool> DeactivatePlanAsync(int planId, int transporterRegId);

        // ---- Search available transporters (for shoppers) ----
        Task<List<AvailableTransporterDto>> SearchAvailableTransportersAsync(
            string fromLocation, string toLocation, DateTime travelDate);

        // ---- Delivery Requests ----
        Task<TransporterDeliveryRequest> CreateDeliveryRequestAsync(ShopperDeliveryRequestDto dto);
        Task<List<DeliveryRequestDto>> GetPendingRequestsAsync(int transporterRegId);
        Task<List<ActiveDeliveryDto>> GetActiveDeliveryAsync(int transporterRegId);
        Task<bool> AcceptDeliveryRequestAsync(int deliveryReqId, int transporterRegId);
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

        // Notifications

        Task<List<TransporterDBNotifications>> GetUnreadNotificationsAsync(int transporterId);
        Task MarkAllAsReadAsync(int transporterId);
        Task MarkEachNotificationReadAsync(int notificationId);
    }
}