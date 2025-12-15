using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> selections);

        Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId);

        Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections);
    }
}
