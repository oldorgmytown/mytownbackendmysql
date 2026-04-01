// ===== ITransporterDashboardService.cs =====
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

        Task<List<AvailableTransporterDto>> SearchAvailableTransportersAsync(
            string fromLocation, string toLocation, DateTime travelDate);

        Task<(bool success, string message, int deliveryReqId)> CreateDeliveryRequestAsync(ShopperDeliveryRequestDto dto);
        Task<List<DeliveryRequestDto>> GetPendingRequestsAsync(int transporterRegId);
        Task<List<ActiveDeliveryDto>> GetActiveDeliveryAsync(int transporterRegId);
        Task<bool> AcceptDeliveryRequestAsync(int deliveryReqId, int transporterRegId);
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
    }
}
