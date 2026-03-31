using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IOrderService
    {
        // Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> selections);
        //Task<int> CreateOrderAsync(
        //int shopperRegId,
        //int? selectedAltAddressId,
        //List<StoreShippingSelection> shippingSelections);

        Task<int> CreateOrderAsync(CreateOrderRequestddto request);
        Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId);

        Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections);

        Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId);
        Task<OrderConfirmationDto> GetOrderConfirmationforOrderHistoryAsync(int orderId);




    }
}
