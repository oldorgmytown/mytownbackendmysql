using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.Services;

namespace mytown.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authService)
        {
            _authRepo = authService;
        }

        [HttpPost("CheckEmail")]
        public IActionResult CheckEmail([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            if (_authRepo.EmailExists(email))
                return Ok(new { success = true });

            return NotFound("Email not registered.");
        }
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] string email)
        {
            if (!_authRepo.EmailExists(email))
                return NotFound(new { success = false, message = "Email not found" });

            _authRepo.SendResetEmail(email);

            return Ok(new { success = true, message = "Reset link sent." });
        }

        [HttpGet("verify-reset-token")]
        public IActionResult VerifyResetToken([FromQuery] string token)
        {
            var request = _authRepo.GetResetRequestByToken(token);

            if (request == null)
                return BadRequest(new { error = "Invalid or expired token." });

            return Ok(new { message = "Valid token", email = request.Email });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromForm] string email, [FromForm] string newPassword, [FromForm] string confirmPassword)
        {
            var result = _authRepo.ResetPassword(email, newPassword);
            if (!result) return BadRequest("Invalid or expired token.");
            return Ok("Password reset successful.");
        }
       
    }

}

