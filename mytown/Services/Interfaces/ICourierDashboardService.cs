using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ICourierDashboardService
    {
        Task<List<CourierOrderDto>> GetOrdersAsync(
                int courierId,
                string shippingStatus);

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
    }
}
