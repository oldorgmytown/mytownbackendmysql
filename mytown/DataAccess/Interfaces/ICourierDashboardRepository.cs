using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{

    public interface ICourierDashboardRepository
    {
        Task<List<CourierOrderDto>> GetOrdersAsync(
     int courierId,
     string shippingStatus,
     string? search,
     int pageNumber,
     int pageSize);

        Task<List<CourierOrderDto>> GetOrdersByBranchAsync(
    int branchId,
    string shippingStatus,
    string? search,
    int pageNumber,
    int pageSize);
        Task<ShippingDetails> GetByStoreOrderIdAsync(int storeOrderId);
        Task SaveAsync();

        Task<CourierOrderDetailDto> GetCourierOrderDetailAsync(int storeOrderId);

        Task<CourierService> GetCourierWithBranchesAsync(int courierId);

        Task<int> GetCompletedDeliveriesCountAsync(int courierId, DateTime date);

        Task<int> GetTotalCompletedDeliveriesCountAsync(
     int courierId,
     int? month,
     int? year,
     DateTime? fromDate,
     DateTime? toDate);

        Task<int> GetPendingTasksCountAsync(
      int courierId,
      int? month,
      int? year,
      DateTime? fromDate,
      DateTime? toDate);

        Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesAsync(
   int courierId,
   DateTime? date);

        Task<List<CourierDBNotifications>> GetUnreadNotificationsAsync(int courierId);
        Task MarkNotificationsAsReadAsync(int courierId);

        Task<List<BranchBasicDto>> GetBasicBranches(int courierId);

        // here are apis for courier branch 

        Task<CourierBranchDto> GetBranchAsync(int branchId);
        Task<int> GetCompletedDeliveriesCountByBranchAsync(int branchId, DateTime date);

        Task<int> GetTotalCompletedDeliveriesCountByBranchAsync(
     int branchId,
     int? month,
     int? year,
     DateTime? fromDate,
     DateTime? toDate);
        Task<int> GetPendingTasksCountByBranchAsync(
    int branchId,
    int? month,
    int? year,
    DateTime? fromDate,
    DateTime? toDate);
       

        Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesByBranchAsync(int branchId, DateTime? date);

        Task<List<CourierDBNotifications>> GetUnreadNotificationsByBranchAsync(int branchId);


        Task MarkEachNotificationReadAsync(int notificationId);

        Task<string> UploadDeliveryProofAsync(int storeOrderId, IFormFile file);

        // updating courier profile status
        Task<CourierService?> GetCourierByIdAsync(int courierId);

        Task UpdateCourierAsync(CourierService courier);


    }




    }
