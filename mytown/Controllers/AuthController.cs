using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.Services;
using System.Security.Claims;

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
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            if (!_authRepo.EmailExists(email))
                return NotFound("Email not found");

            _authRepo.SendResetEmail(email);

            return Ok("Reset link sent.");
        }

        [HttpGet("verify-reset-token")]
        public IActionResult VerifyResetToken([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Email is required.");
            var request = _authRepo.GetResetRequestByToken(token);

            if (request == null)
                return BadRequest(new { error = "Invalid or expired token." });

            return Ok(new { message = "Valid token", email = request.Email });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(
     [FromForm] string email,
     [FromForm] string newPassword,
     [FromForm] string confirmPassword)
        {
            // 1️⃣ Validate inputs
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required." );

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                return BadRequest("Password and Confirm Password are required." );

            if (newPassword != confirmPassword)
                return BadRequest("Passwords do not match.");

            // 2️⃣ Call repo
            var result = _authRepo.ResetPassword(email, newPassword);

            // 3️⃣ Handle result
            if (!result)
                return BadRequest("Invalid or expired token.");

            return Ok("Password reset successful.");
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var sessionId = User.FindFirst("sessionId")!.Value;
            var userType = User.FindFirst(ClaimTypes.Role)!.Value;

            var result = await _authRepo.LogoutAsync(userId, sessionId, userType);

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

            var result = await _authRepo.RevokeSessionAsync(sessionId);

            if (!result)
                return NotFound(new { message = "Session not found" });

            return Ok(new { message = "Session revoked successfully" });
        }


    }

}

