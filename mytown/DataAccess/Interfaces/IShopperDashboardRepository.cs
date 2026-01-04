using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
   

    public interface IShopperDashboardRepository
    {
        Task<List<CurrentOrderDto>> GetCurrentOrdersByShopperAsync(int shopperRegId);
        Task<ShopperOrderDetailsDto> GetShopperOrderDetailsAsync(int storeOrderId);

        Task<List<BuyAgainProductDto>> GetBuyAgainProductsAsync(int shopperRegId);

        Task<List<WishlistItemDto>> GetWishlistAsync(int shopperId);

        Task<int> GetWishlistCountAsync(int shopperRegId);
        Task<int> GetCurrentOrdersCountAsync(int shopperRegId);
        Task<int> GetTotalOrdersCountAsync(int shopperRegId);

       // Task<List<OrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId);
        Task<List<ShopperDBOrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId);

        Task<ShopperDetailsDto?> GetShopperDetailsAsync(int shopperRegId);
    }

}
