using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IOrderRepository
    {
       

        Task<int> CreateOrderAsync(CreateOrderRequestddto request);
        Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId);
        Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections);

        Task AddShopperNotificationAsync(ShopperDBNotifications notification);
        Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId);

        //stock qty reduce after order placed
        Task ReduceStockAsync(int orderId);
    }
}

