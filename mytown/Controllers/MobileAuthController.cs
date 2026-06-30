using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Repositories;
using mytown.Models.DTO_s;

namespace mytown.Controllers
{
    [Route("api/mobileapp/auth")]
    [ApiController]
    public class MobileAuthController : ControllerBase
    {
        private readonly IMobileAuthRepository _authRepo;
        private readonly ILogger<MobileAuthController> _logger;

        public MobileAuthController(IMobileAuthRepository authRepo, ILogger<MobileAuthController> logger)
        {
            _authRepo = authRepo;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] MobileSignupDto dto)
        {
            try
            {
                var result = await _authRepo.SignupAsync(dto);
                if (!result.success)
                    return BadRequest(new { error = result.message });
                return Ok(new { message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Signup error for {Email}", dto.Email);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }

        [AllowAnonymous]
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] MobileSendOtpDto dto)
        {
            try
            {
                var result = await _authRepo.SendOtpAsync(dto.Email, dto.Role);
                if (!result.success)
                    return BadRequest(new { error = result.message });
                return Ok(new { message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send OTP error for {Email}", dto.Email);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }

        [AllowAnonymous]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] MobileSendOtpDto dto)
        {
            try
            {
                var result = await _authRepo.SendOtpAsync(dto.Email, dto.Role);
                if (!result.success)
                    return BadRequest(new { error = result.message });
                return Ok(new { message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend OTP error for {Email}", dto.Email);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] MobileVerifyOtpDto dto)
        {
            try
            {
                var result = await _authRepo.VerifyOtpAsync(dto.Email, dto.Otp, dto.Role);
                if (!result.success)
                    return BadRequest(new { error = result.message });
                return Ok(new { message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verify OTP error for {Email}", dto.Email);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }
    }
}