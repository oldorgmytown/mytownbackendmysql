using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;
using System.Text.Json;

namespace mytown.Controllers
{

    [ApiController]
    [Route("api/courier")]
    public class CourierDashboardController : ControllerBase
    {
        private readonly ICourierDashboardService _courierService;
        private readonly IEmailService _emailService;                 // still available for store notifications etc.
        private readonly IConfiguration _configuration;
        private readonly ILogger<CourierController> _logger;
        private readonly IBusinessRepository _businessRepo;
        private readonly IShopperRepository _shopperRepo;

        public CourierDashboardController(
            ICourierDashboardService courierService,
            IBusinessRepository businessRepo,
            IShopperRepository shopperRepo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<CourierController> logger)
        {
            _courierService = courierService;
            _businessRepo = businessRepo;
            _shopperRepo = shopperRepo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(
         int courierId,[FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("Shipping status is required");

         //   int courierId = int.Parse(User.FindFirst("CourierId").Value);

            var orders = await _courierService.GetOrdersAsync(courierId, status);
            return Ok(orders);
        }

        [HttpGet("Orderdetail_StoreOrder")]
        public async Task<IActionResult> GetCourierOrderDetail(int storeOrderId)
        {
            var result = await _courierService.GetCourierOrderDetailAsync(storeOrderId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("AssignTrackingId")]
        public async Task<IActionResult> AssignTracking(
    int storeOrderId,
    [FromBody] AssignTrackingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TrackingId))
                return BadRequest("Tracking ID is required");

            try
            {
                await _courierService.AssignTrackingAsync(
                    storeOrderId,
                    dto.TrackingId);

                return Ok("Tracking ID assigned and order moved to In Progress");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("MarkasDelivered")]
        public async Task<IActionResult> MarkAsDelivered(int storeOrderId)
        {
            try
            {
                await _courierService.MarkAsDeliveredAsync(storeOrderId);

                return Ok("Order marked as delivered");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Courierprofilesummary")]
        public async Task<IActionResult> GetProfileSummary()
        {
            int courierId = int.Parse(User.FindFirst("CourierId").Value);

            var summary = await _courierService.GetProfileSummaryAsync(courierId);
            return Ok(summary);
        }

        // 🔹 Today’s Deliveries
        [HttpGet("deliveries/today")]
        public async Task<IActionResult> GetTodayDeliveries()
        {
            int courierId = int.Parse(User.FindFirst("CourierId").Value);

            var result = await _courierService.GetCompletedDeliveriesAsync(
                courierId,
                DateTime.UtcNow);

            return Ok(result);
        }

        // 🔹 All Completed Deliveries
        [HttpGet("deliveries/completed")]
        public async Task<IActionResult> GetAllCompletedDeliveries()
        {
            int courierId = int.Parse(User.FindFirst("CourierId").Value);

            var result = await _courierService.GetCompletedDeliveriesAsync(
                courierId,
                null);

            return Ok(result);
        }

        [HttpGet("Getcouriernotifications")]
        public async Task<IActionResult> GetUnreadNotifications(int courierId)
        {
            var notifications = await _courierService.GetUnreadNotificationsAsync(courierId);
            return Ok(notifications);
        }

        [HttpPost("MarkcouriernotificationsAsread")]
        public async Task<IActionResult> MarkNotificationsAsRead(int courierId)
        {
            await _courierService.MarkAsReadAsync(courierId);
            return Ok(new { message = "Notifications marked as read" });
        }


    }

}
