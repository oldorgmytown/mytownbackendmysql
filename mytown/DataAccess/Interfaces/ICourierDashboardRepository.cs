using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{

    public interface ICourierDashboardRepository
    {
        Task<List<CourierOrderDto>> GetOrdersAsync(
    int courierId,
    string shippingStatus);

        Task<ShippingDetails> GetByStoreOrderIdAsync(int storeOrderId);
        Task SaveAsync();

        Task<CourierOrderDetailDto> GetCourierOrderDetailAsync(int storeOrderId);

        Task<CourierService> GetCourierWithBranchesAsync(int courierId);

        Task<int> GetCompletedDeliveriesCountAsync(int courierId, DateTime date);

        Task<int> GetTotalCompletedDeliveriesCountAsync(int courierId);

        Task<int> GetPendingTasksCountAsync(int courierId);

        Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesAsync(
   int courierId,
   DateTime? date);

        Task<List<CourierDBNotifications>> GetUnreadNotificationsAsync(int courierId);
        Task MarkNotificationsAsReadAsync(int courierId);


    }




    }
