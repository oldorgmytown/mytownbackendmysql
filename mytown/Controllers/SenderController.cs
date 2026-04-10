using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}