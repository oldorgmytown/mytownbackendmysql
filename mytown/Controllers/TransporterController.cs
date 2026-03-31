using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Diagnostics;

namespace mytown.Controllers
{

    [ApiController]
    [Route("api/transporters")]
    public class TransporterController : ControllerBase
    {
        private readonly ITransporterService _transporterService;
        private readonly ILogger<TransporterController> _logger;

        public TransporterController(
            ITransporterService transporterService,
            ILogger<TransporterController> logger)
        {
            _transporterService = transporterService;
            _logger = logger;
        }

        // ---------------- REGISTER ----------------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] TransporterRegisterDto dto)
        {
            try
            {
                var transporterId = await _transporterService.RegisterTransporterAsync(dto);

                return Ok(new
                {
                    message = "Email Veriifcation sent successfully",
                    transporterRegId = transporterId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering transporter {Email}", dto.Email);

                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        // ---------------- VERIFY EMAIL ----------------
        [AllowAnonymous]
        [HttpGet("verify-transporter-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var transporterId = await _transporterService.VerifyEmailAsync(token);

                return Ok(new
                {
                    message = "Email verified successfully",
                    transporterRegId = transporterId
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
                await _transporterService.ResendVerificationEmailAsync(model.Email);

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
