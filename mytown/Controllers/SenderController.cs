using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DTOs;
using mytown.Models.DTO_s;
using mytown.Models.DTOs;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/senders")]
    public class SenderController : ControllerBase
    {
        private readonly ISenderService _senderService;
        private readonly ILogger<SenderController> _logger;

        public SenderController(
            ISenderService senderService,
            ILogger<SenderController> logger)
        {
            _senderService = senderService;
            _logger = logger;
        }

        // ---------------- REGISTER ----------------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] SenderRegisterDto dto)
        {
            try
            {
                var senderRegId = await _senderService.RegisterSenderAsync(dto);

                return Ok(new
                {
                    message = "Email verification sent successfully",
                    senderRegId = senderRegId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering sender {Email}", dto.Email);

                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        // ---------------- VERIFY EMAIL ----------------
        [AllowAnonymous]
        [HttpGet("verify-sender-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var senderRegId = await _senderService.VerifyEmailAsync(token);

                return Ok(new
                {
                    message = "Email verified successfully",
                    senderRegId = senderRegId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email verification failed for token {Token}", token);

                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        // ---------------- RESEND VERIFICATION ----------------
        [AllowAnonymous]
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendemailVerificationDTO model)
        {
            try
            {
                await _senderService.ResendVerificationEmailAsync(model.Email);

                return Ok(new
                {
                    message = "Verification email resent successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend verification failed for {Email}", model.Email);

                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        // ---------------- CREATE SENDER ORDER ----------------

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> Create(
       [FromBody] CreateSenderOrderDto dto)
        {
            var senderOrderId =
                await _senderService.CreateSenderOrderAsync(dto);

            return Ok(new
            {
                SenderOrderId = senderOrderId,
                Message = "Sender order created successfully"
            });
        }

        // ---------------- GET MATCHING TRANSPORTERS ----------------
        [HttpGet("matching-transporters/{senderOrderId}")]
        public async Task<IActionResult>
    GetMatchingTransporters(int senderOrderId)
        {
            var result =
                await _senderService
                .GetMatchingTransportersAsync(senderOrderId);

            return Ok(result);
        }

        [HttpPost("summary")]
        public async Task<IActionResult>
    GetSummary(
        [FromBody]
        SenderOrderSummaryRequestDto dto)
        {
            var result =
                await _senderService
                .GetOrderSummaryAsync(dto);

            return Ok(result);
        }

        [HttpPost("create-payment-intent/{senderOrderId}")]
        public async Task<IActionResult>
    CreatePaymentIntent(int senderOrderId)
        {
            var result =
                await _senderService
                .CreatePaymentIntentAsync(
                    senderOrderId);

            return Ok(result);
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult>
    ConfirmPayment(
        [FromBody]
        ConfirmSenderPaymentDto dto)
        {
            await _senderService
                .ConfirmPaymentAsync(dto);

            return Ok(new
            {
                Message = "Payment successful"
            });
        }

        [HttpGet("senderorderconfirmation/{senderOrderId}")]
        public async Task<IActionResult>
    GetConfirmation(
        int senderOrderId)
        {
            var result =
                await _senderService
                .GetOrderConfirmationAsync(
                    senderOrderId);

            return Ok(result);
        }

        [HttpPut("update-delivery-status")]
        public async Task<IActionResult>
    UpdateSenderPackageDeliveryStatus(
        [FromBody]
        UpdateSenderPackageDeliveryStatusDto dto)
        {
            await _senderService
                .UpdateSenderPackageDeliveryStatusAsync(dto);

            return Ok(new
            {
                Message = "Status updated"
            });
        }

        [HttpGet("sender/orders")]
        public async Task<IActionResult>
GetSenderOrders(
    int senderId,
    string orderType)
        {
            var result =
                await _senderService.GetSenderOrdersAsync(
                    senderId,
                    orderType);

            return Ok(result);
        }

        [HttpGet("sender-profile/{senderRegId}")]
        public async Task<IActionResult> GetSenderProfile(int senderRegId)
        {
            var result = await _senderService
                .GetSenderProfileAsync(senderRegId);

            if (result == null)
                return NotFound("Sender not found");

            return Ok(result);
        }

        [HttpPut("update-sender-profile/{senderRegId}")]
        public async Task<IActionResult> UpdateSenderProfile(
    int senderRegId,
    [FromBody] UpdateSenderProfileDto dto)
        {
            var updated =
                await _senderService.UpdateSenderProfileAsync(
                    senderRegId,
                    dto);

            if (!updated)
                return NotFound("Sender not found");

            return Ok("Profile updated successfully");
        }

        // =========================================================
        // NOTIFICATIONS
        // =========================================================

        [HttpGet("notifications/{senderId}")]
        public async Task<IActionResult> GetUnreadNotifications(int senderId)
        {
            var notifications =
                await _senderService.GetUnreadNotificationsAsync(senderId);

            return Ok(notifications);
        }

        [HttpPut("notifications/mark-all-read/{senderId}")]
        public async Task<IActionResult> MarkAllAsRead(int senderId)
        {
            await _senderService.MarkAsReadAsync(senderId);

            return Ok(new
            {
                message = "All notifications marked as read."
            });
        }

        [HttpPut("notifications/mark-read/{notificationId}")]
        public async Task<IActionResult> MarkNotificationRead(int notificationId)
        {
            await _senderService.MarkEachNotificationReadAsync(notificationId);

            return Ok(new
            {
                message = "Notification marked as read."
            });
        }

        // ---------------- ALTERNATE ADDRESSES ----------------

        
        [HttpGet("GetSenderAltAddress")]
        public async Task<IActionResult> GetAddresses(int senderRegId)
        {
            var addresses =
                await _senderService.GetAddressesAsync(senderRegId);

            return Ok(addresses);
        }

        //[Authorize]
        [HttpPost("AddSenderAltAddress")]
        public async Task<IActionResult> AddAddress(
            [FromBody] SenderAlternateAddressDto dto)
        {
            var result =
                await _senderService.AddAddressAsync(dto);

            return Ok(result);
        }

       // [Authorize]
        [HttpDelete("DeleteSenderAltAddress/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var deleted =
                await _senderService.DeleteAddressAsync(id);

            if (!deleted)
                return NotFound();

            return Ok(new
            {
                message = "Address deleted successfully"
            });
        }
    }
}