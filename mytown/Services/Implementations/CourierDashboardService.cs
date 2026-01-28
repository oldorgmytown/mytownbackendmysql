using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{


    public class CourierDashboardService : ICourierDashboardService
    {
        private readonly ICourierDashboardRepository _repository;

        private static readonly HashSet<string> ValidStatuses =
       new() { "Need to be shipped", "In Progress", "Complete" };
        public CourierDashboardService(ICourierDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CourierOrderDto>> GetOrdersAsync(
         int courierId,
         string shippingStatus)
        {
            if (!ValidStatuses.Contains(shippingStatus))
                throw new Exception("Invalid shipping status");

            return await _repository.GetOrdersAsync(
                courierId, shippingStatus);
        }

        public async Task AssignTrackingAsync(
            int storeOrderId,
            string trackingId)
        {
            var shipment = await _repository.GetByStoreOrderIdAsync(storeOrderId);

            if (shipment.ShippingStatus != "Need to be shipped")
                throw new Exception("Tracking can be added only for new orders");

            shipment.TrackingId = trackingId;
            shipment.ShippingStatus = "In Progress";

            await _repository.SaveAsync();
        }

        public async Task MarkAsDeliveredAsync(int storeOrderId)
        {
            var shipment = await _repository.GetByStoreOrderIdAsync(storeOrderId);

            if (shipment.ShippingStatus != "In Progress")
                throw new Exception("Only in-progress orders can be completed");

            shipment.ShippingStatus = "Complete";
            shipment.DeliveredDate = DateTime.UtcNow;

            await _repository.SaveAsync();
        }

        public async Task<CourierOrderDetailDto> GetCourierOrderDetailAsync(int storeOrderId)
        {
            return await _repository.GetCourierOrderDetailAsync(storeOrderId);
        }

        public async Task<CourierProfileSummaryDto> GetProfileSummaryAsync(int courierId)
        {
            var courier = await _repository.GetCourierWithBranchesAsync(courierId);

            var today = DateTime.UtcNow.Date;

            return new CourierProfileSummaryDto
            {
                CourierName = courier.CourierServiceName,
                Phone = courier.CourierPhone,
                Email = courier.CourierEmail,

                //Branches = courier.CourierBranches.Select(b => new CourierBranchDto
                //{
                //    BranchId = b.BranchId,
                //    City = b.City,
                //    Town = b.Town
                //}).ToList(),

                TodayDeliveries = await _repository.GetCompletedDeliveriesCountAsync(
                    courierId, today),

                TotalDeliveries = await _repository.GetTotalCompletedDeliveriesCountAsync(
                    courierId),

                PendingTasks = await _repository.GetPendingTasksCountAsync(
                    courierId)
            };
        }

        public async Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesAsync(
    int courierId,
    DateTime? date)
        {
            return await _repository.GetCompletedDeliveriesAsync(courierId, date);
        }

        public async Task<List<CourierDBNotifications>> GetUnreadNotificationsAsync(int courierId)
        {
            return await _repository.GetUnreadNotificationsAsync(courierId);
        }

        public async Task MarkAsReadAsync(int courierId)
        {
            await _repository.MarkNotificationsAsReadAsync(courierId);
        }

    }
}
