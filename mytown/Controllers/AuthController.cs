using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Services.Interfaces;
using System.Security.Claims;
using mytown.Services.Implementations;  

namespace mytown.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("CheckEmail")]
        public IActionResult CheckEmail([FromBody] string email, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            if (_authService.EmailExists(email,role))
                return Ok(new { success = true });

            return NotFound("Email not registered.");
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(
     [FromBody] string email,
     [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(role))
                return BadRequest("Role is required.");

            if (!_authService.EmailExists(email, role))
                return NotFound("Email not found for the selected role.");

            _authService.SendResetEmail(email);

            return Ok("Reset link sent.");
        }

        [HttpGet("verify-reset-token")]
        public IActionResult VerifyResetToken([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required.");

            var request = _authService.GetResetRequestByToken(token);

            if (request == null)
                return BadRequest(new { error = "Invalid or expired token." });

            dynamic record = request;
            return Ok(new { message = "Valid token", email = record.Email });

        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromForm] string email, [FromForm] string newPassword, [FromForm] string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                return BadRequest("Password and Confirm Password are required.");

            if (newPassword != confirmPassword)
                return BadRequest("Passwords do not match.");

            var result = _authService.ResetPassword(email, newPassword);

            if (!result)
                return BadRequest("Invalid or expired token.");

            return Ok("Password reset successful.");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var sessionId = User.FindFirst("SessionGuid")!.Value;
            var userType = User.FindFirst(ClaimTypes.Role)!.Value;

            var result = await _authService.LogoutAsync(userId, sessionId, userType);

            if (!result)
                return BadRequest(new { message = "Logout failed or session not found." });

            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("sessions/revoke")]
        public async Task<IActionResult> RevokeSession([FromBody] string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return BadRequest("SessionId is required");

            var result = await _authService.RevokeSessionAsync(sessionId);

            if (!result)
                return NotFound(new { message = "Session not found" });

            return Ok(new { message = "Session revoked successfully" });
        }
    }
}
