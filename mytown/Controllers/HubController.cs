using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;


namespace mytown.Controllers
{
    [Route("api/hub")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubService _hubService;
        private readonly ILogger<HubController> _logger;

        public HubController(IHubService hubService,
                             ILogger<HubController> logger)
        {
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/hub/locations
        [HttpGet("get_allhub_locations")]
        public async Task<IActionResult> GetAllHubLocations()
        {
            var hubs = await _hubService.GetAllHubLocationsAsync();
            return Ok(hubs);
        }
       // [HttpGet("store-orders-onhub")]
        // GET: api/hub/store-orders-onhub?month=9&year=2026
        [HttpGet("store-orders-onhub")]
        public async Task<IActionResult> GetStoreOrders([FromQuery] int? month, [FromQuery] int? year)
        {
            if (month.HasValue && (month < 1 || month > 12))
                return BadRequest(new { message = "Month must be between 1 and 12." });

            var data = await _hubService.GetTransporterStoreOrdersAsync(month, year);
            return Ok(data);
        }

        // GET: api/hub/sender-orders-onhub?month=9&year=2026
        [HttpGet("sender-orders-onhub")]
        public async Task<IActionResult> GetSenderOrders([FromQuery] int? month, [FromQuery] int? year)
        {
            if (month.HasValue && (month < 1 || month > 12))
                return BadRequest(new { message = "Month must be between 1 and 12." });

            var data = await _hubService.GetTransporterSenderOrdersAsync(month, year);
            return Ok(data);
        }

        [HttpGet("get_store-orders_verified")]
        public async Task<IActionResult> GetVerification(int storeOrderId)
        {
            var data = await _hubService.GetVerificationAsync(storeOrderId);
            return Ok(data);   // null if the hub hasn't started verification yet
        }

        // PUT: api/hub/store-orders/490/verification
        [HttpPut("save-store-orders_verification")]
        public async Task<IActionResult> SaveVerification(
            int storeOrderId, [FromBody] SaveHubVerificationDto dto)
        {
            var result = await _hubService.SaveVerificationAsync(storeOrderId, dto);

            if (!result.Success)
                return result.Error == "Store order not found."
                    ? NotFound(new { message = result.Error })
                    : BadRequest(new { message = result.Error });

            return Ok(result.Data);
        }

        // GET: api/hub/sender-orders/12/verification
        [HttpGet("get-sender-orders-verified")]
        public async Task<IActionResult> GetSenderVerification(int senderOrderId)
        {
            var data = await _hubService.GetSenderVerificationAsync(senderOrderId);
            return Ok(data);   // null if verification hasn't started
        }

        // PUT: api/hub/sender-orders/12/verification
        [HttpPut("save-sender-orders-verification")]
        public async Task<IActionResult> SaveSenderVerification(
            int senderOrderId, [FromBody] SaveHubVerificationDto dto)
        {
            var result = await _hubService.SaveSenderVerificationAsync(senderOrderId, dto);

            if (!result.Success)
                return result.Error == "Sender order not found."
                    ? NotFound(new { message = result.Error })
                    : BadRequest(new { message = result.Error });

            return Ok(result.Data);
        }

        // GET: api/hub/store-orders/490/details
        [HttpGet("get-store-orders-details")]
        public async Task<IActionResult> GetStoreOrderDetails(int storeOrderId)
        {
            var data = await _hubService.GetStoreOrderDetailsAsync(storeOrderId);

            if (data == null)
                return NotFound(new { message = "Store order not found or no transporter assigned." });

            return Ok(data);
        }

        // GET: api/hub/sender-orders/12/details
        [HttpGet("get-sender-orders-details")]
        public async Task<IActionResult> GetSenderOrderDetails(int senderOrderId)
        {
            var data = await _hubService.GetSenderOrderDetailsAsync(senderOrderId);

            if (data == null)
                return NotFound(new { message = "Sender order not found or no transporter assigned." });

            return Ok(data);
        }

        // GET: api/hub/store-order-counts?month=9&year=2026
        [HttpGet("store-order-summary-counts")]
        public async Task<IActionResult> GetStoreOrderCounts(
            [FromQuery] int? month, [FromQuery] int? year)
        {
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;

            if (m < 1 || m > 12)
                return BadRequest(new { message = "Month must be between 1 and 12." });
            if (y < 2000 || y > 2100)
                return BadRequest(new { message = "Invalid year." });

            return Ok(await _hubService.GetStoreOrderCountsAsync(m, y));
        }

        // GET: api/hub/sender-order-counts?month=9&year=2026
        [HttpGet("sender-order-summary-counts")]
        public async Task<IActionResult> GetSenderOrderCounts(
            [FromQuery] int? month, [FromQuery] int? year)
        {
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;

            if (m < 1 || m > 12)
                return BadRequest(new { message = "Month must be between 1 and 12." });
            if (y < 2000 || y > 2100)
                return BadRequest(new { message = "Invalid year." });

            return Ok(await _hubService.GetSenderOrderCountsAsync(m, y));
        }
    }
}
