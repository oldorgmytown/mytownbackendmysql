using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    // [Authorize]
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

        // =========================================================
        // DASHBOARD SUMMARY
        // =========================================================

        [HttpGet("summary/{transporterRegId}")]
        public async Task<IActionResult> GetDashboardSummary(int transporterRegId)
        {
            try
            {
                var result = await _service.GetDashboardSummaryAsync(transporterRegId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDashboardSummary failed for transporter {Id}", transporterRegId);
                return BadRequest(new { error = ex.Message });
            }
        }

        // =========================================================
        // TRAVEL PLANS
        // =========================================================

        [HttpGet("travel-plan/active/{transporterRegId}")]
        public async Task<IActionResult> GetActivePlan(int transporterRegId)
        {
            var plan = await _service.GetActivePlanAsync(transporterRegId);
            if (plan == null) return NotFound(new { message = "No active travel plan found." });
            return Ok(plan);
        }

        [HttpGet("travel-plan/all/{transporterRegId}")]
        public async Task<IActionResult> GetAllPlans(int transporterRegId)
        {
            var plans = await _service.GetAllPlansAsync(transporterRegId);
            return Ok(plans);
        }

        [HttpPost("travel-plan")]
        public async Task<IActionResult> SaveTravelPlan([FromBody] TravelPlanDto dto)
        {
            try
            {
                var saved = await _service.SaveTravelPlanAsync(dto);
                return Ok(saved);
            }
           
                catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        

        [HttpPut("travel-plan/{planId}/deactivate/{transporterRegId}")]
        public async Task<IActionResult> DeactivatePlan(int planId, int transporterRegId)
        {
            var result = await _service.DeactivatePlanAsync(planId, transporterRegId);
            if (!result) return NotFound(new { message = "Plan not found." });
            return Ok(new { message = "Plan deactivated." });
        }

        // =========================================================
        // SEARCH AVAILABLE TRANSPORTERS (called by shoppers)
        // =========================================================

        [HttpGet("search")]
        public async Task<IActionResult> SearchAvailableTransporters(
     [FromQuery] string startTown,
     [FromQuery] string startCity,
     [FromQuery] string startState,
     [FromQuery] string startCountry,

     [FromQuery] string destinationTown,
     [FromQuery] string destinationCity,
     [FromQuery] string destinationState,
     [FromQuery] string destinationCountry)

           {
            var results =
                await _service.SearchAvailableTransportersAsync(
                    startTown,
                    startCity,
                    startState,
                    startCountry,
                    destinationTown,
                    destinationCity,
                    destinationState,
                    destinationCountry);

            return Ok(results);
        }

        // =========================================================
        // DELIVERY REQUEST — created by shopper, auto-assigned to transporter
        // =========================================================

        /// <summary>
        /// POST /api/transporter-dashboard/delivery-request
        /// Shopper submits request → auto-assigned to the transporter who owns the PlanId.
        /// No accept step. Status starts as "Assigned".
        /// </summary>
        [HttpPost("delivery-request")]
        public async Task<IActionResult> CreateDeliveryRequest([FromBody] ShopperDeliveryRequestDto dto)
        {
            var (success, message, deliveryReqId) = await _service.CreateDeliveryRequestAsync(dto);

            if (!success)
                return BadRequest(new { error = message });

            return Ok(new { message, deliveryReqId });
        }

        // =========================================================
        // ACTIVE DELIVERIES (Assigned + ReachedPickup + PickedUp + InTransit)
        // =========================================================

        [HttpGet("deliveries/active/{transporterRegId}")]
        public async Task<IActionResult> GetActiveDeliveries(int transporterRegId)
        {
            var deliveries = await _service.GetActiveDeliveryAsync(transporterRegId);
            return Ok(deliveries);
        }

        // =========================================================
        // UPDATE DELIVERY STATUS
        // Flow: Assigned → ReachedPickup → PickedUp → InTransit → Delivered
        // =========================================================

        /// <summary>
        /// PUT /api/transporter-dashboard/deliveries/update-status
        /// Body: { deliveryReqId, transporterRegId, newStatus }
        /// Allowed newStatus: ReachedPickup | PickedUp | InTransit | Delivered
        /// </summary>
        [HttpPut("deliveries/update-status")]
        public async Task<IActionResult> UpdateDeliveryStatus([FromBody] UpdateDeliveryStatusDto dto)
        {
            var result = await _service.UpdateDeliveryStatusAsync(dto);
            if (!result)
                return BadRequest(new
                {
                    error = "Status update failed. Check delivery ID or status transition. " +
                            "Allowed flow: Assigned → ReachedPickup → PickedUp → InTransit → Delivered"
                });

            return Ok(new { message = $"Delivery status updated to '{dto.NewStatus}'." });
        }

        // =========================================================
        // COMPLETED DELIVERIES
        // =========================================================

        [HttpGet("deliveries/completed/{transporterRegId}")]
        public async Task<IActionResult> GetCompletedDeliveries(int transporterRegId)
        {
            var deliveries = await _service.GetCompletedDeliveriesAsync(transporterRegId);
            return Ok(deliveries);
        }

        // =========================================================
        // EXCEPTION REPORTS
        // =========================================================

        [HttpPost("exception-report")]
        public async Task<IActionResult> SubmitExceptionReport([FromBody] ExceptionReportDto dto)
        {
            var result = await _service.SubmitExceptionReportAsync(dto);
            if (!result) return BadRequest(new { error = "Failed to submit exception report." });
            return Ok(new { message = "Exception report submitted." });
        }

        // =========================================================
        // KYC
        // =========================================================

        [HttpPost("kyc")]
        public async Task<IActionResult> SubmitKyc([FromForm] TransporterKycDto dto)
        {
            var (success, message) = await _service.SubmitKycAsync(dto);
            if (!success) return BadRequest(new { error = message });
            return Ok(new { message });
        }

        // =========================================================
        // BANK DETAILS
        // =========================================================

        [HttpPost("bank-details")]
        public async Task<IActionResult> SubmitBankDetails([FromBody] TransporterBankDto dto)
        {
            var (success, message) = await _service.SubmitBankDetailsAsync(dto);
            if (!success) return BadRequest(new { error = message });
            return Ok(new { message });
        }

        // =========================================================
        // PROFILE
        // =========================================================

        [HttpGet("profile/{transporterRegId}")]
        public async Task<IActionResult> GetProfile(int transporterRegId)
        {
            var profile = await _service.GetProfileAsync(transporterRegId);
            if (profile == null) return NotFound(new { message = "Profile not found." });
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateTransporterProfileDto dto)
        {
            var result = await _service.UpdateProfileAsync(dto);
            if (!result) return NotFound(new { message = "Transporter not found." });
            return Ok(new { message = "Profile updated successfully." });
        }

        /// <summary>
        /// PUT /api/transporter-dashboard/profile/change-password
        /// Body: { transporterRegId, currentPassword, newPassword }
        /// Uses UpdateTransporterPasswordDto — NOT the shopper UpdatePasswordDto
        /// </summary>
        [HttpPut("profile/change-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdateTransporterPasswordDto dto)
        {
            var result = await _service.UpdatePasswordAsync(
                dto.TransporterRegId, dto.CurrentPassword, dto.NewPassword);  // ✅ TransporterRegId

            if (!result) return BadRequest(new { error = "Password update failed." });
            return Ok(new { message = "Password updated successfully." });
        }

        // =========================================================
        // NOTIFICATIONS
        // =========================================================

        [HttpGet("notifications/{transporterId}")]
        public async Task<IActionResult> GetUnreadNotifications(int transporterId)
        {
            var notifications = await _service.GetUnreadNotificationsAsync(transporterId);
            return Ok(notifications);
        }

        [HttpPut("notifications/mark-all-read/{transporterId}")]
        public async Task<IActionResult> MarkAllAsRead(int transporterId)
        {
            await _service.MarkAsReadAsync(transporterId);
            return Ok(new { message = "All notifications marked as read." });
        }

        [HttpPut("notifications/mark-read/{notificationId}")]
        public async Task<IActionResult> MarkNotificationRead(int notificationId)
        {
            await _service.MarkEachNotificationReadAsync(notificationId);
            return Ok(new { message = "Notification marked as read." });
        }

        // mark package as delivered

        [HttpPost("mark-delivered/{storeOrderId}")]
        public async Task<IActionResult> MarkAsDelivered(int storeOrderId)
        {
            try
            {
                var result = await _service.MarkAsDeliveredAsync(storeOrderId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}