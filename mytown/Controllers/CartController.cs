using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
   // [Authorize]
    [Route("api/shoppingcart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService,
                              ILogger<CartController> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ---------------------- ADD TO CART ----------------------
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCart cartItem)
        {
            if (cartItem == null)
                return BadRequest(new { code = 400, message = "Invalid request data" });

            if (cartItem.ShopperRegId <= 0)
                return BadRequest(new { code = 400, message = "Shopper ID is required." });

            if (cartItem.ProductId <= 0)
                return BadRequest(new { code = 400, message = "Product ID is required." });

            if (cartItem.BusRegId <= 0)
                return BadRequest(new { code = 400, message = "Store ID is required." });

            try
            {
                var updatedCartItem = await _cartService.AddToCart(cartItem);

                if (updatedCartItem == null)
                    return BadRequest(new { code = 400, message = "Failed to add product to cart." });

                return Ok(new { code = 200, data = updatedCartItem });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddToCart failed");
                return StatusCode(500, new { code = 500, message = $"Internal Server Error: {ex.Message}" });
            }
        }

        // ---------------------- GET CART ITEMS ----------------------
        [HttpGet("GetCartItems/{shopperRegId}")]
        public async Task<IActionResult> GetCartItems(int shopperRegId)
        {
            var cartItems = await _cartService.GetCartItems(shopperRegId);

            return Ok(new
            {
                code = 200,
                data = cartItems ?? new List<CartItemDto>()
            });
        }

        // ---------------------- INCREASE QTY ----------------------
        [HttpPut("IncreaseCartQty/{cartId}")]
        public async Task<IActionResult> IncreaseCartQty(int cartId)
        {
            if (cartId <= 0)
                return BadRequest(new { code = 400, message = "Invalid cart ID." });

            var success = await _cartService.IncreaseCartItemQty(cartId);

            if (!success)
                return NotFound(new { code = 404, message = "Cart item not found." });

            return Ok(new { code = 200, message = "Cart item quantity increased." });
        }

        // ---------------------- DECREASE QTY ----------------------
        [HttpPut("DecreaseCartQty/{cartId}")]
        public async Task<IActionResult> DecreaseCartQty(int cartId)
        {
            if (cartId <= 0)
                return BadRequest(new { code = 400, message = "Invalid cart ID." });

            var success = await _cartService.DecreaseCartItemQty(cartId);

            if (!success)
                return NotFound(new { code = 404, message = "Cart item not found." });

            return Ok(new { code = 200, message = "Cart item quantity decreased." });
        }

        // ---------------------- REMOVE FROM CART ----------------------
        [HttpDelete("RemoveFromCart/{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            if (cartId <= 0)
                return BadRequest(new { code = 400, message = "Invalid cart ID." });

            var success = await _cartService.RemoveFromCart(cartId);

            if (!success)
                return NotFound(new { code = 404, message = "Cart item not found." });

            return Ok(new { code = 200, message = "Cart item removed successfully." });
        }

        // ---------------------- MOVE TO WISHLIST ----------------------
        [HttpPut("MoveToWishlist/{cartId}")]
        public async Task<IActionResult> MoveToWishlist(int cartId)
        {
            if (cartId <= 0)
                return BadRequest(new { code = 400, message = "Invalid cart ID." });

            var result = await _cartService.MoveToWishlist(cartId);

            if (!result)
                return NotFound(new { code = 404, message = "Item not found!" });

            return Ok(new { code = 200, message = "Item moved to wishlist!" });
        }

        // ---------------------- MOVE BACK TO CART ----------------------
        [HttpPut("MoveBackToCart/{cartId}")]
        public async Task<IActionResult> MoveBackToCart(int cartId)
        {
            if (cartId <= 0)
                return BadRequest(new { code = 400, message = "Invalid cart ID." });

            var result = await _cartService.MoveBackToCart(cartId);

            if (!result)
                return NotFound(new { code = 404, message = "Item not found!" });

            return Ok(new { code = 200, message = "Item moved back to cart!" });
        }

        // ---------------------- UPDATE CART STATUS BY ORDER ----------------------
        [HttpPost("update-cart-status/{orderId}")]
        public async Task<IActionResult> UpdateCartStatus(int orderId)
        {
            if (orderId <= 0)
                return BadRequest(new { code = 400, message = "Invalid Order ID." });

            var result = await _cartService.UpdateCartStatusAsync(orderId);

            if (!result)
                return NotFound(new { code = 404, message = "Order or Cart items not found." });

            return Ok(new { code = 200, message = "Cart status updated successfully." });
        }

        // ---------------------- UPDATE CART STATUS BY SHOPPER ----------------------
        [HttpPost("update-cart-status-by-shopper/{shopperRegId}")]
        public async Task<IActionResult> UpdateCartStatusByShopper(int shopperRegId)
        {
            if (shopperRegId <= 0)
                return BadRequest(new { code = 400, message = "Invalid Shopper ID." });

            var result = await _cartService.UpdateCartStatusByShopperAsync(shopperRegId);

            if (!result)
                return NotFound(new { code = 404, message = "No cart items found for this shopper." });

            return Ok(new { code = 200, message = "Cart status updated successfully." });
        }

        // ---------------------- GET SHOPPER DETAILS ----------------------
        [HttpGet("GetShopperDetails/{shopperRegId}")]
        public async Task<IActionResult> GetShopperDetails(int shopperRegId)
        {
            if (shopperRegId <= 0)
                return BadRequest(new { code = 400, message = "Invalid Shopper ID." });

            var shopper = await _cartService.GetShopperDetails(shopperRegId);

            if (shopper == null)
                return Ok(new { code = 200, message = "No Data" });

            return Ok(new { code = 200, data = shopper });
        }

        // ---------------------- PRODUCT + VARIANT DETAILS ----------------------
        [HttpGet("GetProductVariantDetailsforCart/{productId}")]
        public async Task<IActionResult> GetProductVariantDetails(int productId)
        {
            if (productId <= 0)
                return BadRequest(new { code = 400, message = "Invalid Product ID." });

            var productDetails = await _cartService.GetProductAndVariantforCartAsync(productId);

            if (productDetails == null)
                return NotFound(new { code = 404, message = "Product not found" });

            return Ok(new { code = 200, data = productDetails });
        }
    }
}
