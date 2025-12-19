using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IOrderRepository
    {
       // Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> shippingSelections);
        Task<int> CreateOrderAsync(
      int shopperRegId,
      int? selectedAltAddressId,
      List<StoreShippingSelection> shippingSelections
  );
        Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId);
        Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections);


        Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId);
    }
}

