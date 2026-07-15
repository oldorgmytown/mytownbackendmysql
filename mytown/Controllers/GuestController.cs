using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [Route("api/guests")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _guestService;
        private readonly ILogger<GuestController> _logger;

        public GuestController(IGuestService guestService, ILogger<GuestController> logger)
        {
            _guestService = guestService;
            _logger = logger;
        }

        // ---------------- REGISTER ----------------

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var result = await _guestService.CheckEmailAsync(email);

            if (!result.success)
                return BadRequest(new { error = result.message });

            return Ok(new { message = result.message });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] GuestRegisterDto dto)
        {
            try
            {
                var result = await _guestService.RegisterGuestAsync(dto);

                if (!result.success)
                    return BadRequest(new { error = result.message });

                //  Now returns guestRegId
                return Ok(new { message = result.message, guestRegId = result.guestRegId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering guest {Email}", dto.Email);
                return StatusCode(500, new { error = "Something went wrong. Please try again." });
            }
        }

        // ---------------- VERIFY EMAIL ----------------
        [AllowAnonymous]
        [HttpGet("verify-guest-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var result = await _guestService.VerifyEmailAsync(token);

                if (!result.success)
                    return BadRequest(new { error = result.message });

                return Ok(new { message = result.message, guestRegId = result.guestRegId });
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
                var result = await _guestService.ResendVerificationEmailAsync(model.Email);

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

        // ---------------- LOGIN ----------------
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] GuestLoginDto dto)
        {
            try
            {
                var result = await _guestService.LoginAsync(dto);

                if (!result.success)
                    return BadRequest(new { error = result.message });

                return Ok(new
                {
                    message = result.message,
                    token = result.token,
                    guestRegId = result.guestRegId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for guest {Email}", dto.Email);
                return StatusCode(500, new { error = "Something went wrong. Please try again." });
            }
        }

        // ---------------- GET GUEST DETAILS ----------------
        [Authorize]
        [HttpGet("getGuestdetails/{guestRegId}")]
        public async Task<IActionResult> GetGuestDetails(int guestRegId)
        {
            try
            {
                var result = await _guestService.GetGuestDetailsAsync(guestRegId);

                if (result == null)
                    return NotFound(new { error = "Guest not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching guest details for {GuestRegId}", guestRegId);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }
    }
}