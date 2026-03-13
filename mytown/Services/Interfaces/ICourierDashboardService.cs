using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ICourierDashboardService
    {
        Task<List<CourierOrderDto>> GetOrdersAsync(
     int courierId,
     string shippingStatus,
     string? search,
     int pageNumber,
     int pageSize);
        //branch orders

        Task<List<CourierOrderDto>> GetOrdersByBranchAsync(
    int branchId,
    string shippingStatus,
    string? search,
    int pageNumber,
    int pageSize);


        Task AssignTrackingAsync(int storeOrderId, string trackingId);
        Task MarkAsDeliveredAsync(int storeOrderId);

        Task<CourierOrderDetailDto> GetCourierOrderDetailAsync(int storeOrderId);
        Task<CourierProfileSummaryDto> GetProfileSummaryAsync(int courierId);

        // 🔹 Completed Deliveries (Today / All)
        Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesAsync(
            int courierId,
            DateTime? date);

        Task<List<CourierDBNotifications>> GetUnreadNotificationsAsync(int courierId);
        Task MarkAsReadAsync(int courierId);

        //for branches 

        Task<CourierBranch> GetBranchAsync(int branchId);
        Task<CourierProfileSummaryDto> GetBranchProfileSummaryAsync(int courierId);
        

        Task<int> GetCompletedDeliveriesCountByBranchAsync(int branchId, DateTime date);

        Task<int> GetTotalCompletedDeliveriesCountByBranchAsync(int branchId);

        Task<int> GetPendingTasksCountByBranchAsync(int branchId);

        Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesByBranchAsync(int branchId, DateTime? date);

        Task<List<CourierDBNotifications>> GetUnreadNotificationsByBranchAsync(int branchId);

        Task MarkEachNotificationReadAsync(int notificationId);
        Task<string> UploadDeliveryProofAsync(int storeOrderId, IFormFile file);
    }
}
