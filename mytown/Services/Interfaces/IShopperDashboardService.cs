
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IShopperDashboardService
    {
        Task<List<CurrentOrderDto>> GetCurrentOrdersAsync(int shopperRegId);
        Task<ShopperOrderDetailsDto> GetShopperOrderDetailsAsync(int storeOrderId);
        Task<List<BuyAgainProductDto>> GetBuyAgainProductsAsync(int shopperRegId);
        Task<List<WishlistItemDto>> GetWishlistAsync(int shopperId);

        Task<ShopperOrderSummaryDto> GetShopperOrderSummaryAsync(int shopperRegId);

        Task<List<ShopperDBOrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId);

        Task<ShopperDetailsDto?> GetShopperDetailsAsync(int shopperRegId);
    }
}
