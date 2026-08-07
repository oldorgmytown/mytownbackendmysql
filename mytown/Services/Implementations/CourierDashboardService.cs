using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{


    public class CourierDashboardService : ICourierDashboardService
    {
        private readonly ICourierDashboardRepository _repository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;

        private static readonly HashSet<string> ValidStatuses =
       new() { "Pending","New Order", "Ready to Ship", "In Progress", "Delivered" };
        public CourierDashboardService(ICourierDashboardRepository repository, IOrderRepository orderRepository, IEmailService emailService
            )
        {
            _repository = repository;
            _orderRepository = orderRepository;
            _emailService = emailService;
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

            // allow tracking id allotment only for ready to ship orders
            if (shipment.ShippingStatus != "Ready to Ship")
                throw new Exception("Tracking can be added only after Ready to Ship notification from Store");

            shipment.TrackingId = trackingId;
            shipment.ShippingStatus = "In Progress";
            //make courier profile status as Active after taking firts order

            // Get CourierId
            var courierId = shipment.CourierBranch.CourierId;

            var courier = await _repository.GetCourierByIdAsync(courierId);

            if (courier != null)
            {
                courier.ProfileStatus = "Active";
                await _repository.UpdateCourierAsync(courier);
            }
            await _repository.SaveAsync();

            // we are using same order confirmation for shopper and guest

            var orderConfirmation =
                await _orderRepository.GetOrderConfirmationAsync(shipment.OrderId);
            // here shopper and guest both deatils will come under shopper email and shopper name
            await _emailService.SendGuestNotificationforTracking(
                    orderConfirmation.ShopperEmail,
                    orderConfirmation.ShopperName,
                    orderConfirmation
                
                );

            // await _repository.SaveAsync();
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

        public async Task<CourierProfileSummaryDto> GetProfileSummaryAsync(
     int courierId, CourierDeliveryFilterDto? filter)
        {
            var courier = await _repository.GetCourierWithBranchesAsync(courierId);

            var today = DateTime.UtcNow.Date;

            return new CourierProfileSummaryDto
            {
                CourierName = courier.CourierServiceName,
                Phone = courier.CourierPhone,
                Email = courier.CourierEmail,

                //  Always today's deliveries (fixed)
                TodayDeliveries = await _repository.GetCompletedDeliveriesCountAsync(
                    courierId, today),

                //  Filtered total deliveries
                TotalDeliveries = await _repository.GetTotalCompletedDeliveriesCountAsync(
                    courierId,
                    filter?.Month,
                    filter?.Year,
                    filter?.FromDate,
                    filter?.ToDate),

                //  Pending tasks (based on OrderDate + filter)
                PendingTasks = await _repository.GetPendingTasksCountAsync(
                    courierId,
                    filter?.Month,
                    filter?.Year,
                    filter?.FromDate,
                    filter?.ToDate)
            };
        }
        public async Task<CourierProfileSummaryDto> GetBranchProfileSummaryAsync(int branchId, CourierDeliveryFilterDto? filter)
        {
            var branch = await _repository.GetBranchAsync(branchId);

            var today = DateTime.UtcNow.Date;

            return new CourierProfileSummaryDto
            { 
            //{
                CourierName = branch.CourierBranchName,
               Phone = branch.BranchPhoneNumber,
               Email = branch.BranchEmail,


                TodayDeliveries = await _repository.GetCompletedDeliveriesCountByBranchAsync(
                    branch.BranchId, today),

                TotalDeliveries = await _repository.GetTotalCompletedDeliveriesCountByBranchAsync(
                    branch.BranchId, filter?.Month,
                    filter?.Year,
                    filter?.FromDate,
                    filter?.ToDate),

                PendingTasks = await _repository.GetPendingTasksCountByBranchAsync(
                    branch.BranchId, filter?.Month,
                    filter?.Year,
                    filter?.FromDate,
                    filter?.ToDate)
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

        public async Task<List<BranchBasicDto>> GetBasicBranchesAsync(int courierId)
        {
            if (courierId <= 0)
            {
                throw new ArgumentException("Invalid courierId");
            }

            var branches = await _repository.GetBasicBranches(courierId);

            return branches ?? new List<BranchBasicDto>();
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

        public async Task<int> GetTotalCompletedDeliveriesCountByBranchAsync(
     int branchId, CourierDeliveryFilterDto? filter)
        {
            return await _repository.GetTotalCompletedDeliveriesCountByBranchAsync(
                branchId,
                filter?.Month,
                filter?.Year,
                filter?.FromDate,
                filter?.ToDate);
        }

        public async Task<int> GetPendingTasksCountByBranchAsync(
     int branchId, CourierDeliveryFilterDto? filter)
        {
            return await _repository.GetPendingTasksCountByBranchAsync(
                branchId,
                filter?.Month,
                filter?.Year,
                filter?.FromDate,
                filter?.ToDate);
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

        public async Task<bool> UpdateCourierAccountDetailsAsync(
    int courierId,
    UpdateCourierAccountDetailDto dto)
        {
            return await _repository.UpdateCourierAccountDetailsAsync(courierId, dto);
        }
        public async Task<UpdateCourierAccountDetailDto?> GetCourierAccountDetailsByCourierIdAsync(int courierId)
        {
            return await _repository.GetCourierAccountDetailsByCourierIdAsync(courierId);
        }
    }
}
