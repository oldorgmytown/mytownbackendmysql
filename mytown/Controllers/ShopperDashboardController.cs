using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;
using Stripe.Climate;

namespace mytown.Controllers
{
   [Authorize]
    [ApiController]
    [Route("api/shopperdashboard")]
    public class ShopperDashboardController: ControllerBase
    {
        private readonly IShopperDashboardService _shopperdashboardService;

        public ShopperDashboardController(IShopperDashboardService shopperdashboardService)
        {
            _shopperdashboardService = shopperdashboardService;
        }

        [HttpGet("currentordersforshopper")]
        public async Task<IActionResult> GetCurrentOrders(
       [FromQuery] int shopperRegId,
       [FromQuery] string? search,
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 100)
        {
            var result = await _shopperdashboardService.GetCurrentOrdersAsync(
                shopperRegId, search, pageNumber, pageSize);

            return Ok(result);
        }


[HttpGet("storeorderid_details")]
public async Task<IActionResult> GetShopperOrderDetails(
    [FromQuery] int storeOrderId,
    [FromQuery] string? search,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 100)
{
    var result = await _shopperdashboardService.GetShopperOrderDetailsAsync(
        storeOrderId, search, pageNumber, pageSize);

    if (result == null)
        return NotFound();

    return Ok(result);
}

        [HttpGet("shopperDBbuy-again")]
        public async Task<IActionResult> GetBuyAgainProducts(
     [FromQuery] int shopperRegId,
     [FromQuery] string? search,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 100)
        {
            var result = await _shopperdashboardService
                .GetBuyAgainProductsAsync(shopperRegId, search, pageNumber, pageSize);

            return Ok(result);
        }


        [HttpGet("shopperDBwishlist")]
        public async Task<IActionResult> GetWishlist(
            [FromQuery] int shopperId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            var data = await _shopperdashboardService
                .GetWishlistAsync(shopperId, search, pageNumber, pageSize);

            return Ok(data);
        }

        [HttpDelete("removefrom_wishlist")]
        public async Task<IActionResult> RemoveFromWishlist(int shopperId, int productId, int skuId)
        {
            if (shopperId <= 0)
                return BadRequest(new { code = 400, message = "Invalid Shopper id" });

            var removed = await _shopperdashboardService.RemoveFromWishlistAsync(shopperId, productId, skuId);

            if (!removed)
                return NotFound(new { code = 404, message = "Wishlist item not found" });

            return Ok(new { code = 200, message = "Item removed from wishlist" });
        }



        [HttpGet("shopperDBorder-summary")]
        public async Task<IActionResult> GetOrderSummary(int shopperRegId)
        {
            var summary = await _shopperdashboardService.GetShopperOrderSummaryAsync(shopperRegId);

            return Ok(summary);
        }

        [HttpGet("ShopperDBorder-history")]
        public async Task<IActionResult> GetOrderHistory(int shopperRegId)
        {
            var orders = await _shopperdashboardService
                .GetOrderHistoryByShopperAsync(shopperRegId);

            if (orders == null || !orders.Any())
                return Ok(new List<ShopperDBOrderHistoryDto>());

            return Ok(orders);
        }

        /// <summary>
        /// Get shopper profile details
        /// </summary>
        [HttpGet("shopperprofiledeatils")]
        public async Task<IActionResult> GetShopperDetails(int shopperRegId)
        {
            var shopper = await _shopperdashboardService
                .GetShopperDetailsAsync(shopperRegId);

            if (shopper == null)
                return NotFound("Shopper not found");

            return Ok(shopper);
        }

        [HttpPut("Updateshopperprofile")]
        public async Task<IActionResult> UpdateShopperDetails(
    [FromBody] UpdateShopperDetailsDto dto)
        {
            if (dto == null || dto.ShopperRegId <= 0)
                return BadRequest("Invalid data");

            var updated = await _shopperdashboardService
                .UpdateShopperDetailsAsync(dto);

            if (!updated)
                return NotFound("Shopper not found");

            return Ok(new
            {
                code = 200,
                message = "Profile updated successfully"
            });
        }

        //[HttpPut("update_Shopperpassword")]
        //public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        //{
        //    try
        //    {
        //        var result = await _shopperdashboardService.UpdatePasswordAsync(dto);

        //        if (!result)
        //            return BadRequest(new { message = "Password update failed" });

        //        return Ok(new { message = "Password updated successfully" });
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Unauthorized(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}


        //Notifications
        [HttpGet("shopper-notifications")]
        public async Task<IActionResult> GetShopperNotifications(
    int shopperId,
    [FromQuery] bool onlyUnread = false)
        {
            var result = await _shopperdashboardService.GetShopperNotificationsAsync(shopperId, onlyUnread);
            return Ok(result);
        }

        [HttpPut("shopper-notifications_mark-read")]
        public async Task<IActionResult> MarkShopperNotificationsAsRead(int shopperId)
        {
            await _shopperdashboardService.MarkAllShopperAsReadAsync(shopperId);
            return Ok(new { message = "All Notifications marked as read" });
        }

        [HttpPut("shopper-each-notification-read")]
        public async Task<IActionResult> MarkEachShopperNotificationAsRead(int notificationId)
        {
            await _shopperdashboardService.MarkEachShopperNotificationAsReadAsync(notificationId);
            return Ok(new { message = "Notification marked as read" });
        }
    }
}
