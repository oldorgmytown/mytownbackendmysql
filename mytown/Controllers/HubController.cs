using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;
using System.Runtime.CompilerServices;



namespace mytown.Controllers
{
    [Route("api/hub")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubService _hubService;
        private readonly ILogger<HubController> _logger;
        private readonly IEmailService _emailService;
        private readonly ITransporterDashboardService _service;

        public HubController(IHubService hubService,
                             ILogger<HubController> logger, IEmailService emailService, ITransporterDashboardService service)
        {
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // GET: api/hub/locations
        [HttpGet("get_allhub_locations")]
        public async Task<IActionResult> GetAllHubLocations()
        {
            var hubs = await _hubService.GetAllHubLocationsAsync();
            return Ok(hubs);
        }

        // GET: api/hub/store-orders-onhub?month=9&year=2026
        // GET: api/hub/store-orders-onhub?hubId=1&month=9&year=2026
        [HttpGet("store-orders-onhub")]
        public async Task<IActionResult> GetStoreOrders(
            [FromQuery] int hubId, [FromQuery] int? month, [FromQuery] int? year)
        {
            if (hubId <= 0)
                return BadRequest(new { message = "hubId is required." });
            if (month.HasValue && (month < 1 || month > 12))
                return BadRequest(new { message = "Month must be between 1 and 12." });

            var data = await _hubService.GetTransporterStoreOrdersAsync(hubId, month, year);
            return Ok(data);
        }

        // GET: api/hub/sender-orders-onhub?hubId=1&month=9&year=2026
        [HttpGet("sender-orders-onhub")]
        public async Task<IActionResult> GetSenderOrders(
            [FromQuery] int hubId, [FromQuery] int? month, [FromQuery] int? year)
        {
            if (hubId <= 0)
                return BadRequest(new { message = "hubId is required." });
            if (month.HasValue && (month < 1 || month > 12))
                return BadRequest(new { message = "Month must be between 1 and 12." });

            var data = await _hubService.GetTransporterSenderOrdersAsync(hubId, month, year);
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
        // GET: api/hub/store-order-summary-counts?month=9&year=2026&hubId=1
        [HttpGet("store-order-summary-counts")]
        public async Task<IActionResult> GetStoreOrderCounts(
            [FromQuery] int hubId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;

            if (hubId <= 0)
                return BadRequest(new { message = "hubId is required." });
            if (m < 1 || m > 12)
                return BadRequest(new { message = "Month must be between 1 and 12." });
            if (y < 2000 || y > 2100)
                return BadRequest(new { message = "Invalid year." });

            return Ok(await _hubService.GetStoreOrderCountsAsync(m, y, hubId));
        }

        // GET: api/hub/sender-order-summary-counts?month=9&year=2026&hubId=1
        [HttpGet("sender-order-summary-counts")]
        public async Task<IActionResult> GetSenderOrderCounts(
            [FromQuery] int hubId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;

            if (hubId <= 0)
                return BadRequest(new { message = "hubId is required." });
            if (m < 1 || m > 12)
                return BadRequest(new { message = "Month must be between 1 and 12." });
            if (y < 2000 || y > 2100)
                return BadRequest(new { message = "Invalid year." });

            return Ok(await _hubService.GetSenderOrderCountsAsync(m, y, hubId));
        }

        // POST: api/hub/send-pckrdy-transporter-email
        // NOTE: `trasnporteremail` (typo kept so the URL contract stays the same) carries the transporter NAME.
        [HttpPost("send-pckrdy-transporter-email")]
        public async Task<IActionResult> SendTransporterEmail(
    [FromQuery] string? transporteremail,
    [FromQuery] string? transportername,
    [FromQuery] string? packagedimensions,
    [FromQuery] int transporterRegId,
    [FromQuery] int storeOrderId,
    [FromBody] HubLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(transporteremail))
                return BadRequest(new { message = "Transporter email is required." });
            if (transporterRegId <= 0 || storeOrderId <= 0)
                return BadRequest(new { message = "transporterRegId and storeOrderId are required." });

            try
            {
                await _emailService.SendTransporterHubEmailAsync(
                    transporteremail,
                    string.IsNullOrWhiteSpace(transportername) ? "Transporter" : transportername,
                    packagedimensions ?? string.Empty,
                    dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send package-ready email to transporter {Email}", transporteremail);
                return StatusCode(500, new { message = "Failed to send email to transporter." });
            }

            // Email succeeded. A notification failure should not fail the request.
            try
            {
                var transporterNotification = new TransporterDBNotifications
                {
                    TransporterRegId = transporterRegId,
                    Title = "Package Ready at Hub",
                    Message = $"StoreOrder #{storeOrderId} package has reached the hub and is ready for pickup."
                };

                await _service.AddTransporterNotificationAsync(transporterNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email sent but notification failed for transporter {Id}, store order {storeOrderId}",
                    transporterRegId, storeOrderId);
                return Ok(new { message = "Email sent to transporter. Notification could not be saved." });
            }

            return Ok(new { message = "Email sent and notification added for transporter." });
        }
    }
}