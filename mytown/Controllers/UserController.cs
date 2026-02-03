using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Services.Interfaces;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Buffers.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static mytown.DataAccess.Repositories.UserRepository;

namespace mytown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowFrontend")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public UserController(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (response, token, sessionId) =
                await _userService.LoginAsync(request.Email, request.Password);

            if (response == null)
                return Unauthorized(new { message = "Invalid credentials" });

            Response.Headers["Authorization"] = $"Bearer {token}";
            Response.Headers["x-session-id"] = sessionId;
            Response.Headers["Access-Control-Expose-Headers"] = "Authorization, x-session-id";

            return Ok(response);
        }

        [Authorize]
        [HttpPost("upload_profile_image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Service returns string (image name)
            var result = await _fileService.UploadProfileImageAsync(file);

            // return directly 
            return Ok(result);
        }
    }
}










