using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;

namespace mytown.Controllers
{
    [Route("api/shoppingcart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepo;

        private readonly ILogger<CartController> _logger;

        public CartController(ICartRepository cartRepo,
                                 ILogger<CartController> logger)
        {
            _cartRepo = cartRepo ?? throw new ArgumentNullException(nameof(cartRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCart cartItem)
        {
            if (cartItem == null)
                return BadRequest("Invalid request data");

            try
            {
                var updatedCartItem = await _cartRepo.AddToCart(cartItem);
                return Ok(updatedCartItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        //[HttpGet("GetCartItems/{shopperRegId}")]
        //public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(int shopperRegId)
        //{
        //    var cartItems = await _cartRepo.GetCartItems(shopperRegId);

        //    if (cartItems == null || !cartItems.Any())
        //    {
        //        return NotFound("No items found in the cart.");
        //    }

        //    return Ok(cartItems);
        //}

        [HttpGet("GetCartItems/{shopperRegId}")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(int shopperRegId)
        {
            var cartItems = await _cartRepo.GetCartItems(shopperRegId);

            // ✅ Always return 200 OK, even if no items found
            return Ok(cartItems ?? new List<CartItemDto>());
        }

        [HttpPut("IncreaseCartQty/{cartId}")]
        public async Task<IActionResult> IncreaseCartQty(int cartId)
        {
            var success = await _cartRepo.IncreaseCartItemQty(cartId);

            if (!success)
            {
                return NotFound("Cart item not found.");
            }

            return Ok("Cart item quantity increased.");
        }
        [HttpPut("DecreaseCartQty/{cartId}")]
        public async Task<IActionResult> DecreaseCartQty(int cartId)
        {
            var success = await _cartRepo.DecreaseCartItemQty(cartId);

            if (!success)
            {
                return NotFound("Cart item not found.");
            }

            return Ok("Cart item quantity decreased.");
        }
        [HttpDelete("RemoveFromCart/{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var success = await _cartRepo.RemoveFromCart(cartId);

            if (!success)
            {
                return NotFound("Cart item not found.");
            }

            return Ok("Cart item removed successfully.");
        }

        [HttpPut("MoveToWishlist/{cartId}")]
        public async Task<IActionResult> MoveToWishlist(int cartId)
        {
            var result = await _cartRepo.MoveToWishlist(cartId);
            if (result) return Ok(new { message = "Item moved to wishlist!" });
            return NotFound(new { message = "Item not found!" });
        }

        [HttpPut("MoveBackToCart/{cartId}")]
        public async Task<IActionResult> MoveBackToCart(int cartId)
        {
            var result = await _cartRepo.MoveBackToCart(cartId);
            if (result) return Ok(new { message = "Item moved back to cart!" });
            return NotFound(new { message = "Item not found!" });
        }

        [HttpPost("update-cart-status/{orderId}")]
        public async Task<IActionResult> UpdateCartStatus(int orderId)
        {
            var result = await _cartRepo.UpdateCartStatusAsync(orderId);
            if (!result)
            {
                return NotFound(new { message = "Order or Cart items not found." });
            }

            return Ok(new { message = "Cart status updated successfully." });
        }

        [HttpPost("update-cart-status-by-shopper/{shopperRegId}")]
        public async Task<IActionResult> UpdateCartStatusByShopper(int shopperRegId)
        {
            var result = await _cartRepo.UpdateCartStatusByShopperAsync(shopperRegId);
            if (!result)
            {
                return NotFound(new { message = "No cart items found for this shopper." });
            }

            return Ok(new { message = "Cart status updated successfully." });
        }

        [HttpGet("GetShopperDetails/{shopperRegId}")]
        public async Task<IActionResult> GetShopperDetails(int shopperRegId)
        {
            var shopper = await _cartRepo.GetShopperDetails(shopperRegId);

            if (shopper == null)
            {
                return Ok(new { message = "No Data" }); // Return message instead of 404
            }

            return Ok(shopper);
        }
        // get product and varinat details for cart

        [HttpGet("GetProductVariantDetailsforCart/{productId}")]
        public async Task<IActionResult> GetProductVariantDetails(int productId)
        {
            var productDetails = await _cartRepo.GetProductAndVariantforCartAsync(productId);
            if (productDetails == null)
                return NotFound(new { Message = "Product not found" });

            return Ok(productDetails);
        }
    }
}
