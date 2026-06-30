using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [Route("api/shoppers")]
    [ApiController]
    public class ShopperController : ControllerBase
    {
        private readonly IShopperService _shopperService;
        private readonly ILogger<ShopperController> _logger;

        public ShopperController(
            IShopperService shopperService,
            ILogger<ShopperController> logger)
        {
            _shopperService = shopperService;
            _logger = logger;
        }

        // ---------------- REGISTER ----------------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ShopperRegisterDto dto)
        {
            try
            {
                var result = await _shopperService.RegisterShopperAsync(dto);

                if (!result.success)
                    return BadRequest(new { error = result.message });

                return Ok(new { message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering shopper {Email}", dto.Email);
                return StatusCode(500, new { error = "Something went wrong. Please try again." });
            }
        }

        // ---------------- VERIFY EMAIL ----------------
        [AllowAnonymous]
        [HttpGet("verify-shopper-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var result = await _shopperService.VerifyEmailAsync(token);

                if (!result.success)
                    return BadRequest(new { error = result.message });

                return Ok(new
                {
                    message = result.message,
                    shopperRegId = result.shopperRegId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email verification failed for token {Token}", token);
                return StatusCode(500, new { error = "Could not verify email." });
            }
        }

        // ---------------- RESEND VERIFICATION ----------------
        [AllowAnonymous]
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendemailVerificationDTO model)
        {
            try
            {
                var result = await _shopperService.ResendVerificationEmailAsync(model.Email);

                if (!result.success)
                    return BadRequest(new { error = result.message });

                return Ok(new { message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend verification failed for {Email}", model.Email);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }

        // ---------------- TOWNS WITH STORE COUNT ----------------
        [HttpGet("GetTownsWithStoreCountByCountry/{country}")]
        public async Task<IActionResult> GetTownsWithStoreCountByCountry(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return BadRequest("Country is required.");

            var result = await _shopperService.GetTownsWithStoreCountByCountryAsync(country);

            if (result == null || !result.Any())
                return NotFound($"No towns found for country '{country}'.");

            return Ok(result);
        }

        // ---------------- RECENTLY VIEWED PRODUCTS ----------------
        [Authorize]
        [HttpGet("productsrecentlyviewedbyshopper/{shopperId}")]
        public async Task<IActionResult> GetRecentlyViewed(
            int shopperId,
            int days = 7,
            int limit = 10)
        {
            var products = await _shopperService
                .GetRecentlyViewedProductsAsync(shopperId, days, limit);

            return Ok(products);
        }

        // ---------------- ALTERNATE ADDRESSES ----------------
        [Authorize]
        [HttpGet("GetShopperAltAddress")]
        public async Task<IActionResult> GetAddresses(int shopperRegId)
        {
            var addresses = await _shopperService.GetAddressesAsync(shopperRegId);
            return Ok(addresses);
        }

        [Authorize]
        [HttpPost("AddAltShopperAddress")]
        public async Task<IActionResult> AddAddress([FromBody] ShopperAlternateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _shopperService.AddAddressAsync(dto);

            return CreatedAtAction(
                nameof(GetAddresses),
                new { shopperRegId = result.ShopperRegId },
                result);
        }

        [Authorize]
        [HttpDelete("DeleteShopperAltAddress/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var deleted = await _shopperService.DeleteAddressAsync(id);

            if (!deleted)
                return NotFound(new { message = $"Alternate address with ID {id} not found." });

            return Ok(new
            {
                message = $"Alternate address with ID {id} has been successfully deleted."
            });
        }

        // ---------------- CHECK EMAIL EXISTS ----------------
        [AllowAnonymous]
        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
            try
            {
                var result = await _shopperService.CheckEmailExistsAsync(email);

                return Ok(new { exists = result.exists, message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email {Email}", email);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }

     
    }
}