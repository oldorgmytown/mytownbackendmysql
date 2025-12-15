using mytown.DataAccess.Interfaces;
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

        public Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> selections)
            => _repo.CreateOrderAsync(shopperRegId, selections);

        public Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId)
            => _repo.CreateOrderAndOrderDetailsAsync(shopperRegId);

        public Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections)
            => _repo.SaveShippingSelectionsAsync(orderId, selections);
    }
}

