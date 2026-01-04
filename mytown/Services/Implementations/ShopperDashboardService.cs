using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class ShopperDashboardService : IShopperDashboardService
    {
        private readonly IShopperDashboardRepository _shopperdashboardRepository;

        public ShopperDashboardService(IShopperDashboardRepository shopperdashboardRepository)
        {
            _shopperdashboardRepository = shopperdashboardRepository;
        }

        public async Task<List<CurrentOrderDto>> GetCurrentOrdersAsync(int shopperRegId)
        {
            // Business rules can be added here later
            return await _shopperdashboardRepository.GetCurrentOrdersByShopperAsync(shopperRegId);
        }

        public async Task<ShopperOrderDetailsDto> GetShopperOrderDetailsAsync(int storeOrderId)
        {
            return await _shopperdashboardRepository.GetShopperOrderDetailsAsync(storeOrderId);
        }
        public async Task<List<BuyAgainProductDto>> GetBuyAgainProductsAsync(int shopperRegId)
        {
            return await _shopperdashboardRepository.GetBuyAgainProductsAsync(shopperRegId);
        }

        public async Task<List<WishlistItemDto>> GetWishlistAsync(int shopperId)
        {
            return await _shopperdashboardRepository.GetWishlistAsync(shopperId);
        }

        public async Task<ShopperOrderSummaryDto> GetShopperOrderSummaryAsync(int shopperRegId)
        {
            var wishlistCount = await _shopperdashboardRepository.GetWishlistCountAsync(shopperRegId);
            var currentOrdersCount = await _shopperdashboardRepository.GetCurrentOrdersCountAsync(shopperRegId);
            var totalOrdersCount = await _shopperdashboardRepository.GetTotalOrdersCountAsync(shopperRegId);

            return new ShopperOrderSummaryDto
            {
                WishlistCount = wishlistCount,
                CurrentOrdersCount = currentOrdersCount,
                TotalOrdersCount = totalOrdersCount
            };
        }


        public async Task<List<ShopperDBOrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId)
        {
            return await _shopperdashboardRepository.GetOrderHistoryByShopperAsync(shopperRegId);
        }

        //get shopper details for profile
        public async Task<ShopperDetailsDto?> GetShopperDetailsAsync(int shopperRegId)
        {
            return await _shopperdashboardRepository.GetShopperDetailsAsync(shopperRegId);
        }

    }
}

