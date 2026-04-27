
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IShopperDashboardService
    {
        Task<List<CurrentOrderDto>> GetCurrentOrdersAsync(
     int shopperRegId,
     string? search,
     int pageNumber,
     int pageSize);

        Task<ShopperOrderDetailsDto?> GetShopperOrderDetailsAsync(
            int storeOrderId,
            string? search,
            int pageNumber,
            int pageSize);
        Task<List<BuyAgainProductDto>> GetBuyAgainProductsAsync(
     int shopperRegId,
     string? search,
     int pageNumber,
     int pageSize);

        Task<List<WishlistItemDto>> GetWishlistAsync(
            int shopperId,
            string? search,
            int pageNumber,
            int pageSize);
        // Remove from wishlist

        Task<bool> RemoveFromWishlistAsync(int shopperId, int productId, int skuId);

        Task<ShopperOrderSummaryDto> GetShopperOrderSummaryAsync(int shopperRegId);

        Task<List<ShopperDBOrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId);

        Task<ShopperDetailsDto?> GetShopperDetailsAsync(int shopperRegId);
        Task<bool> UpdateShopperDetailsAsync(UpdateShopperDetailsDto dto);

        // Task<bool> UpdatePasswordAsync(UpdatePasswordDto dto);

        Task<List<ShopperNotificationDto>> GetShopperNotificationsAsync(int shopperId, bool onlyUnread);

        Task MarkAllShopperAsReadAsync(int shopperId);

        Task MarkEachShopperNotificationAsReadAsync(int notificationId);

    }
}
