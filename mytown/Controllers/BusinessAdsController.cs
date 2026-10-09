using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [Authorize]   // for local testing only you can comment this line out
    [ApiController]
    [Route("api/business/ads")]
    public class BusinessAdsController : ControllerBase
    {
        private readonly IAdsService _service;
        private readonly ILogger<BusinessAdsController> _logger;

        public BusinessAdsController(IAdsService service, ILogger<BusinessAdsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private async Task<IActionResult> Run(Func<Task<object?>> action)
        {
            try
            {
                var data = await action();
                return Ok(new { success = true, data });
            }
            catch (AdsException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ads API error");
                return StatusCode(500, new { success = false, message = "Something went wrong. Please try again." });
            }
        }

        [HttpGet("getPromotions")]
        public Task<IActionResult> GetPromotions([FromQuery] int busRegId)
            => Run(() => _service.GetPromotionsAsync(busRegId));

        [HttpPost("createPromotion")]
        public Task<IActionResult> CreatePromotion([FromBody] AdsCreatePromotionDto dto)
            => Run(() => _service.CreatePromotionAsync(dto));

        [HttpPost("uploadMedia")]
        [RequestSizeLimit(60_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 60_000_000)]
        public Task<IActionResult> UploadMedia(IFormFile file)
            => Run(() => _service.UploadMediaAsync(file));

        [HttpPost("createPaymentOrder")]
        public Task<IActionResult> CreatePaymentOrder([FromBody] AdsPaymentOrderRequestDto dto)
            => Run(() => _service.CreatePaymentOrderAsync(dto));

        [HttpPost("confirmPayment")]
        public Task<IActionResult> ConfirmPayment([FromBody] AdsPaymentConfirmDto dto)
            => Run(() => _service.ConfirmPaymentAsync(dto));

        [HttpPost("createStripeOrder")]
        public Task<IActionResult> CreateStripeOrder([FromBody] AdsPaymentOrderRequestDto dto)
            => Run(() => _service.CreateStripeOrderAsync(dto));

        [HttpPost("confirmStripePayment")]
        public Task<IActionResult> ConfirmStripePayment([FromBody] AdsStripeConfirmDto dto)
            => Run(() => _service.ConfirmStripePaymentAsync(dto));

        [HttpGet("getProducts")]
        public Task<IActionResult> GetProducts([FromQuery] int busRegId)
            => Run(() => _service.GetProductsAsync(busRegId));

        [HttpGet("getStoreProfile")]
        public Task<IActionResult> GetStoreProfile([FromQuery] int busRegId)
            => Run(() => _service.GetStoreInfoAsync(busRegId));
    }
}