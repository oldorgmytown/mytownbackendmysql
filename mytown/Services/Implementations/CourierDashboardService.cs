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
       new() { "Pending","New Order", "In Progress", "Delivered" };
        public CourierDashboardService(ICourierDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CourierOrderDto>> GetOrdersAsync(
      int courierId,
      string shippingStatus,
      string? search,
      int pageNumber,
      int pageSize)
        {
            if (!ValidStatuses.Contains(shippingStatus))
                throw new Exception("Invalid shipping status");

            return await _repository.GetOrdersAsync(
                courierId,
                shippingStatus,
                search,
                pageNumber,
                pageSize);
        }

        public async Task<List<CourierOrderDto>> GetOrdersByBranchAsync(
    int branchId,
    string shippingStatus,
    string? search,
    int pageNumber,
    int pageSize)
        {
            if (!ValidStatuses.Contains(shippingStatus))
                throw new Exception("Invalid shipping status");

            return await _repository.GetOrdersByBranchAsync(
                branchId,
                shippingStatus,
                search,
                pageNumber,
                pageSize);
        }


        public async Task AssignTrackingAsync(
            int storeOrderId,
            string trackingId)
        {
            var shipment = await _repository.GetByStoreOrderIdAsync(storeOrderId);

            if (shipment.ShippingStatus != "Pending")
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

              
                TodayDeliveries = await _repository.GetCompletedDeliveriesCountAsync(
                    courierId, today),

                TotalDeliveries = await _repository.GetTotalCompletedDeliveriesCountAsync(
                    courierId),

                PendingTasks = await _repository.GetPendingTasksCountAsync(
                    courierId)
            };
        }

        public async Task<CourierProfileSummaryDto> GetBranchProfileSummaryAsync(int courierId)
        {
            var courier = await _repository.GetCourierWithBranchesAsync(courierId);

            var today = DateTime.UtcNow.Date;

            return new CourierProfileSummaryDto
            {
                CourierName = courier.CourierServiceName,
                Phone = courier.CourierPhone,
                Email = courier.CourierEmail,


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

        // branches 

        public async Task<CourierBranchDto> GetBranchAsync(int branchId)
        {
            return await _repository.GetBranchAsync(branchId);
        }

        public async Task<int> GetCompletedDeliveriesCountByBranchAsync(int branchId, DateTime date)
        {
            return await _repository.GetCompletedDeliveriesCountByBranchAsync(branchId, date);
        }

        public async Task<int> GetTotalCompletedDeliveriesCountByBranchAsync(int branchId)
        {
            return await _repository.GetTotalCompletedDeliveriesCountByBranchAsync(branchId);
        }

        public async Task<int> GetPendingTasksCountByBranchAsync(int branchId)
        {
            return await _repository.GetPendingTasksCountByBranchAsync(branchId);
        }

        public async Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesByBranchAsync(int branchId, DateTime? date)
        {
            return await _repository.GetCompletedDeliveriesByBranchAsync(branchId, date);
        }

        public async Task<List<CourierDBNotifications>> GetUnreadNotificationsByBranchAsync(int branchId)
        {
            return await _repository.GetUnreadNotificationsByBranchAsync(branchId);
        }

        public async Task MarkEachNotificationReadAsync(int notificationId)
        {
            await _repository.MarkEachNotificationReadAsync(notificationId);
        }

        public async Task<string> UploadDeliveryProofAsync(int storeOrderId, IFormFile file)
        {
            return await _repository.UploadDeliveryProofAsync(storeOrderId, file);
        }
    }
}
