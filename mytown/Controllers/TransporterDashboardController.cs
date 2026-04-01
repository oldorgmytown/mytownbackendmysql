using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/transporter-dashboard")]
    public class TransporterDashboardController : ControllerBase
    {
        private readonly ITransporterDashboardService _service;
        private readonly ILogger<TransporterDashboardController> _logger;

        public TransporterDashboardController(
            ITransporterDashboardService service,
            ILogger<TransporterDashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // =================== DASHBOARD ===================

        [HttpGet("summary/{transporterRegId}")]
        public async Task<IActionResult> GetDashboardSummary(int transporterRegId)
        {
            var result = await _service.GetDashboardSummaryAsync(transporterRegId);
            if (result == null) return NotFound("Transporter not found.");
            return Ok(result);
        }

        // =================== TRAVEL PLAN ===================

        [HttpGet("travel-plan/{transporterRegId}")]
        public async Task<IActionResult> GetActivePlan(int transporterRegId)
        {
            var plan = await _service.GetActivePlanAsync(transporterRegId);
            return Ok(plan); // returns null if no active plan (frontend handles it)
        }

        [HttpPost("travel-plan/save")]
        public async Task<IActionResult> SaveTravelPlan([FromBody] TravelPlanDto dto)
        {
            try
            {
                var result = await _service.SaveTravelPlanAsync(dto);
                return Ok(new { message = "Travel plan saved successfully.", plan = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving travel plan");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("travel-plan/deactivate")]
        public async Task<IActionResult> DeactivatePlan([FromQuery] int planId, [FromQuery] int transporterRegId)
        {
            var result = await _service.DeactivatePlanAsync(planId, transporterRegId);
            if (!result) return NotFound("Plan not found.");
            return Ok(new { message = "Plan deactivated." });
        }

        // =================== SEARCH TRANSPORTERS (for Shoppers) ===================

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> SearchTransporters(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime date)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                return BadRequest("From and To locations are required.");

            var results = await _service.SearchAvailableTransportersAsync(from, to, date);
            return Ok(results);
        }

        // =================== DELIVERY REQUESTS ===================

        // Shopper sends delivery request to a transporter's plan
        [HttpPost("delivery-request/create")]
        public async Task<IActionResult> CreateDeliveryRequest([FromBody] ShopperDeliveryRequestDto dto)
        {
            var (success, message, reqId) = await _service.CreateDeliveryRequestAsync(dto);
            if (!success) return BadRequest(new { error = message });
            return Ok(new { message, deliveryReqId = reqId });
        }

        // Transporter sees pending requests
        [HttpGet("delivery-request/pending/{transporterRegId}")]
        public async Task<IActionResult> GetPendingRequests(int transporterRegId)
        {
            var list = await _service.GetPendingRequestsAsync(transporterRegId);
            return Ok(list);
        }

        // Transporter accepts a request
        [HttpPatch("delivery-request/accept")]
        public async Task<IActionResult> AcceptRequest(
            [FromQuery] int deliveryReqId,
            [FromQuery] int transporterRegId)
        {
            var result = await _service.AcceptDeliveryRequestAsync(deliveryReqId, transporterRegId);
            if (!result) return BadRequest(new { error = "Request not found or already processed." });
            return Ok(new { message = "Delivery request accepted." });
        }

        // =================== ACTIVE DELIVERY ===================

        [HttpGet("active-delivery/{transporterRegId}")]
        public async Task<IActionResult> GetActiveDelivery(int transporterRegId)
        {
            var deliveries = await _service.GetActiveDeliveryAsync(transporterRegId);
            return Ok(deliveries);  // ✅ always returns array — frontend handles 1 or many
        }

        // Update status (ReachedPickup, PickedUp, InTransit, Delivered)
        [HttpPatch("active-delivery/update-status")]
        public async Task<IActionResult> UpdateDeliveryStatus([FromBody] UpdateDeliveryStatusDto dto)
        {
            var result = await _service.UpdateDeliveryStatusAsync(dto);
            if (!result) return BadRequest(new { error = "Status update failed." });
            return Ok(new { message = $"Status updated to {dto.NewStatus}." });
        }

        // Completed deliveries history
        [HttpGet("completed-deliveries/{transporterRegId}")]
        public async Task<IActionResult> GetCompletedDeliveries(int transporterRegId)
        {
            var list = await _service.GetCompletedDeliveriesAsync(transporterRegId);
            return Ok(list);
        }

        // =================== EXCEPTION REPORTS ===================

        [HttpPost("exception-report")]
        public async Task<IActionResult> SubmitExceptionReport([FromBody] ExceptionReportDto dto)
        {
            var result = await _service.SubmitExceptionReportAsync(dto);
            if (!result) return BadRequest(new { error = "Failed to submit report." });
            return Ok(new { message = "Exception report submitted." });
        }

        // =================== VERIFICATION ===================

        [HttpPost("verify/kyc")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubmitKyc([FromForm] TransporterKycDto dto)
        {
            var (success, message) = await _service.SubmitKycAsync(dto);
            if (!success) return BadRequest(new { error = message });
            return Ok(new { message });
        }

        [HttpPost("verify/bank")]
        public async Task<IActionResult> SubmitBankDetails([FromBody] TransporterBankDto dto)
        {
            var (success, message) = await _service.SubmitBankDetailsAsync(dto);
            if (!success) return BadRequest(new { error = message });
            return Ok(new { message });
        }

        // =================== PROFILE ===================

        [HttpGet("profile/{transporterRegId}")]
        public async Task<IActionResult> GetProfile(int transporterRegId)
        {
            var profile = await _service.GetProfileAsync(transporterRegId);
            if (profile == null) return NotFound("Transporter not found.");
            return Ok(profile);
        }

        [HttpPut("profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateTransporterProfileDto dto)
        {
            var result = await _service.UpdateProfileAsync(dto);
            if (!result) return NotFound("Transporter not found.");
            return Ok(new { message = "Profile updated successfully." });
        }

        [HttpPatch("profile/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeTransporterPasswordDto dto)
        {
            var result = await _service.UpdatePasswordAsync(
                dto.TransporterRegId, dto.CurrentPassword, dto.NewPassword);
            if (!result) return BadRequest(new { error = "Password update failed." });
            return Ok(new { message = "Password changed successfully." });
        }


        // get transporter notifications 

        [HttpGet("Gettransporternotifications")]
        public async Task<IActionResult> GetUnreadNotifications(int trasnporterId)
        {
            var notifications = await _service.GetUnreadNotificationsAsync(trasnporterId);
            return Ok(notifications);
        }

        [HttpPost("MarktransporternotificationsAsread")]
        public async Task<IActionResult> MarkNotificationsAsRead(int trasnporterId)
        {
            await _service.MarkAsReadAsync(trasnporterId);
            return Ok(new { message = "Notifications marked as read" });
        }

        [HttpPut("markeach-notification-read")]
        public async Task<IActionResult> MarkEachNotificationRead(int notificationId)
        {
            await _service.MarkEachNotificationReadAsync(notificationId);
            return Ok(new { message = "Notification marked as read successfully" });
        }

        // Small DTO for password change (local to controller file)
        public class ChangeTransporterPasswordDto
        {
            public int TransporterRegId { get; set; }
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
}