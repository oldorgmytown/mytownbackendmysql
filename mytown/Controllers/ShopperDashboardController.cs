using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;
using Stripe.Climate;

namespace mytown.Controllers
{
  //  [Authorize]
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
        public async Task<IActionResult> GetCurrentOrders([FromQuery]int shopperRegId)
        {
            var result = await _shopperdashboardService.GetCurrentOrdersAsync(shopperRegId);
            return Ok(result);
        }

        [HttpGet("storeorderid_details")]
        public async Task<IActionResult> GetShopperOrderDetails([FromQuery] int storeOrderId)
        {
            var result = await _shopperdashboardService.GetShopperOrderDetailsAsync(storeOrderId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("shopperDBbuy-again")]
        public async Task<IActionResult> GetBuyAgainProducts([FromQuery] int shopperRegId)
        {
            var result = await _shopperdashboardService.GetBuyAgainProductsAsync(shopperRegId);
            return Ok(result);
        }

        [HttpGet("shopperDBwishlist")]
        public async Task<IActionResult> GetWishlist(int shopperId)
        {
            var data = await _shopperdashboardService.GetWishlistAsync(shopperId);
            return Ok(data);
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

    }
}
