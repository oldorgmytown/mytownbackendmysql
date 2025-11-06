using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Services;
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
        private readonly UserRepository _userRepository;

        private readonly IWebHostEnvironment _env;
        private readonly string stripeSecretKey = "sk_test_51QtS7OFMWqb9scCuoOdpdCcEb7WultTBEDZMEF7MsjyvgbbdHsQalQyKXsDQaYKBFg4DAAQkL1VeGp6DfO6FZ0CW00hbxqjakt";
        private readonly IEmailService _emailService;

        public UserController(UserRepository userRepository, IWebHostEnvironment env, IEmailService emailService)
        {
            _userRepository = userRepository; // Inject UserRepository
            _env = env;  // Inject IWebHostEnvironment to get access to WebRootPath
            StripeConfiguration.ApiKey = stripeSecretKey;
            _emailService = emailService;
        }




        //        //[HttpPost("login")]
        //        //public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        //        //{
        //        //    if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
        //        //    {
        //        //        return BadRequest(new { code = 400, message = "Invalid login request" });
        //        //    }

        //        //    var result = await _userRepository.LoginAsync(loginRequest.Email, loginRequest.Password);

        //        //    //if (result is string message)
        //        //    //{
        //        //    //    return message switch
        //        //    //    {
        //        //    //        "EmailNotFound" => NotFound(new { code = 404, message = "Email not registered" }),
        //        //    //        "WrongPassword" => Unauthorized(new { code = 401, message = "Incorrect password" }),
        //        //    //        "EmailNotVerified" => StatusCode(403, new { code = 403, message = "Please verify your email before login" }),
        //        //    //        _ => StatusCode(500, new { code = 500, message = "Unexpected login error" })
        //        //    //    };
        //        //    //}

        //        //    return Ok(result); // success
        //        //}
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { code = 400, message = "Invalid login request" });
            }

            try
            {
                var result = await _userRepository.LoginAsync(loginRequest.Email, loginRequest.Password);

                if (result == null)
                    return Unauthorized(new { code = 401, message = "Invalid credentials" });


                // Extract token from result
                var token = result.GetType().GetProperty("token")?.GetValue(result, null)?.ToString();

                if (string.IsNullOrEmpty(token))
                    return StatusCode(500, "Token not generated");

                //Return exactly same way as earlier, with token included
                return Ok(result);

                
            }
            catch (Exception ex)
            {
                // Log full error for debugging
                Console.WriteLine($"Login error: {ex}");

                return StatusCode(500, new { code = 500, message = "An unexpected error occurred" });
            }
        }


     
        //        #region business Profile

        [Authorize]
        [HttpPost("upload_profile_image")]
public async Task<IActionResult> upload_profile_image(IFormFile file)
{


    //string _targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
    string _targetFilePath = Path.Combine(_env.WebRootPath, "UploadedFiles");


    // Ensure that the folder exists
    if (!Directory.Exists(_targetFilePath))
    {
        Directory.CreateDirectory(_targetFilePath);
    }

    if (file == null || file.Length == 0)
    {
        return BadRequest("No file uploaded.");
    }

    // Generate a unique file name using timestamp
    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
    var fileExtension = Path.GetExtension(file.FileName);

    // Create a new file name with timestamp
    var newFileName = $"{fileNameWithoutExtension}_{timestamp}{fileExtension}";

    var filePath = Path.Combine(_targetFilePath, newFileName);

    // Save the file to the server
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    var publicUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{newFileName}";
    return Ok(new { FileName = newFileName, Url = publicUrl });
    // Return the file path or any other necessary data
    // return Ok(new { FileName = newFileName, FilePath = filePath });

}


    }




}











