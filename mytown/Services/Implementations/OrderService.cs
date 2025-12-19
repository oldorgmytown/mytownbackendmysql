using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        //public Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> selections)
        //    => _repo.CreateOrderAsync(shopperRegId, selections);


        public async Task<int> CreateOrderAsync(
       int shopperRegId,
       int? selectedAltAddressId,
       List<StoreShippingSelection> shippingSelections)
        {
            return await _repo.CreateOrderAsync(
                shopperRegId,
                selectedAltAddressId,
                shippingSelections
            );
        }
        public Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId)
            => _repo.CreateOrderAndOrderDetailsAsync(shopperRegId);

        public Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections)
            => _repo.SaveShippingSelectionsAsync(orderId, selections);


        public async Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId)
        {
            return await _repo.GetOrderConfirmationAsync(orderId);
        }

       
    }
}

