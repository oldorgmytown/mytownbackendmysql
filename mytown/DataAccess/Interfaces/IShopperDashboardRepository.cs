using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
   

    public interface IShopperDashboardRepository
    {
        Task<List<CurrentOrderDto>> GetCurrentOrdersByShopperAsync(
      int shopperRegId,
      string? search,
      int pageNumber,
      int pageSize);

        //Task<ShopperOrderDetailsDto?> GetShopperOrderDetailsAsync(
        //    int storeOrderId,
        //    string? search,
        //    int pageNumber,
        //    int pageSize);
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
        //Remove from wishliast
        Task<bool> RemoveFromWishlistAsync(int shopperId, int productId, int skuId);

        Task<int> GetWishlistCountAsync(int shopperRegId);
    
        Task<int> GetCurrentOrdersCountAsync(int shopperRegId);
        Task<int> GetTotalOrdersCountAsync(int shopperRegId);

       // Task<List<OrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId);
        Task<List<ShopperDBOrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId);

        Task<ShopperDetailsDto?> GetShopperDetailsAsync(int shopperRegId);

        Task<bool> UpdateShopperDetailsAsync(UpdateShopperDetailsDto dto);
        //  Task<bool> UpdatePasswordAsync(ShopperRegister shopper);

       Task<List<ShopperDBNotifications>> GetShopperNotificationsAsync(int shopperId, bool onlyUnread);

            Task MarkAllShopperAsReadAsync(int shopperId);

            Task MarkEachShopperNotificationAsReadAsync(int notificationId);
      
    }

}
