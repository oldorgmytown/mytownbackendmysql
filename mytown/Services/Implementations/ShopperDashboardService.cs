using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
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

        public async Task<List<CurrentOrderDto>> GetCurrentOrdersAsync(
     int shopperRegId,
     string? search,
     int pageNumber,
     int pageSize)
        {
            return await _shopperdashboardRepository
                .GetCurrentOrdersByShopperAsync(shopperRegId, search, pageNumber, pageSize);
        }

        public async Task<ShopperOrderDetailsDto?> GetShopperOrderDetailsAsync(
            int storeOrderId,
            string? search,
            int pageNumber,
            int pageSize)
        {
            return await _shopperdashboardRepository
                .GetShopperOrderDetailsAsync(storeOrderId, search, pageNumber, pageSize);
        }
        public async Task<List<BuyAgainProductDto>> GetBuyAgainProductsAsync(
       int shopperRegId,
       string? search,
       int pageNumber,
       int pageSize)
        {
            return await _shopperdashboardRepository
                .GetBuyAgainProductsAsync(shopperRegId, search, pageNumber, pageSize);
        }


        public async Task<List<WishlistItemDto>> GetWishlistAsync(
            int shopperId,
            string? search,
            int pageNumber,
            int pageSize)
        {
            return await _shopperdashboardRepository
                .GetWishlistAsync(shopperId, search, pageNumber, pageSize);
        }
        //remove fom wishlist
        public async Task<bool> RemoveFromWishlistAsync(int shopperId, int productId, int skuId)
        {
            return await _shopperdashboardRepository.RemoveFromWishlistAsync(shopperId,productId, skuId);
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
        public async Task<bool> UpdateShopperDetailsAsync(UpdateShopperDetailsDto dto)
        {
            return await _shopperdashboardRepository.UpdateShopperDetailsAsync(dto);
        }
        //public async Task<bool> UpdatePasswordAsync(ShopperRegister shopper)
        //{
        //    _shopperdashboardRepository.ShopperRegisters.Update(shopper);
        //    return await _shopperdashboardRepository.SaveChangesAsync() > 0;
        //}
        //public async Task<bool> UpdatePasswordAsync(UpdatePasswordDto dto)
        //{
        //    var shopper = await _shopperdashboardRepository.GetByIdAsync(dto.ShopperRegId);
        //    if (shopper == null)
        //        throw new Exception("Shopper not found");

        //    // 🔐 Verify current password
        //    var verifyResult = _passwordHasher.VerifyHashedPassword(
        //        shopper,
        //        shopper.PasswordHash,
        //        dto.CurrentPassword
        //    );

        //    if (verifyResult == PasswordVerificationResult.Failed)
        //        throw new UnauthorizedAccessException("Current password is incorrect");

        //    // 🔐 Hash new password
        //    shopper.PasswordHash = _passwordHasher.HashPassword(shopper, dto.NewPassword);

        //    return await _shopperRepository.UpdatePasswordAsync(shopper);
        //}

        // Get notifications
        public async Task<List<ShopperNotificationDto>> GetShopperNotificationsAsync(
            int shopperId, bool onlyUnread)
        {
            var notifications = await _shopperdashboardRepository.GetShopperNotificationsAsync(shopperId, onlyUnread);

            return notifications.Select(n => new ShopperNotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedDate
            }).ToList();
        }

        // Mark all as read
        public async Task MarkAllShopperAsReadAsync(int shopperId)
        {
            await _shopperdashboardRepository.MarkAllShopperAsReadAsync(shopperId);
        }

        // Mark single as read
        public async Task MarkEachShopperNotificationAsReadAsync(int notificationId)
        {
            await _shopperdashboardRepository.MarkEachShopperNotificationAsReadAsync(notificationId);
        }
    }
}

