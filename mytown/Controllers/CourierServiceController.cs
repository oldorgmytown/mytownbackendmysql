using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services;
using System.Text.Json;

using System.Threading.Tasks;


[ApiController]
[Route("api/courier")]
public class CourierController : ControllerBase
{
    private readonly ICourierServiceRepository _courierrepo;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CourierController> _logger;
    private readonly IVerificationLinkBuildercourier _verificationLinkBuilder;
    private readonly IBusinessRepository _businessRepo;
    private readonly IShopperRepository _shopperRepo;

    public CourierController(ICourierServiceRepository courierrepo,
         IBusinessRepository businessRepo,
        IShopperRepository shopperRepo,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<CourierController> logger,
        IVerificationLinkBuildercourier verificationLinkBuilder)
    {
        _courierrepo = courierrepo;
        _businessRepo = businessRepo;
        _shopperRepo = shopperRepo;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _verificationLinkBuilder = verificationLinkBuilder;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCourier([FromBody] CourierServiceDto courierDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (await _courierrepo.IsCourierEmailTaken(courierDto.CourierEmail))
            {
                return Conflict(new { error = "Email already registered. Try logging in." });
            }

            // Uncomment for email verification
            /*
            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);
            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            string jsonPayload = JsonSerializer.Serialize(courierDto);

            var pending = new PendingCourierVerification
            {
                Email = courierDto.Email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = jsonPayload
            };

            await _repository.SavePendingVerification(pending);
            await _emailService.SendVerificationEmail(courierDto.Email, verificationLink);
            return Ok(new { message = "Verification email sent! Please check your inbox." });
            */

            // SKIP EMAIL FOR NOW AND REGISTER DIRECTLY
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(courierDto.Password);

            var courier = new CourierService
            {
                CourierServiceName = courierDto.CourierServiceName,
                CourierWebsiteName = courierDto.CourierWebsiteName,
                CourierPhone = courierDto.CourierPhone,
                CourierEmail = courierDto.CourierEmail,
                Password = hashedPassword,
                IsEmailVerified = true, // or false if verification email flow is enabled
                RegisteredDate = DateTime.UtcNow,

                // Service types (multi-selection checkboxes)
                IsLocal = courierDto.IsLocal,
                IsState = courierDto.IsState,
                IsNational = courierDto.IsNational,
                IsInternational = courierDto.IsInternational
            };


            var createdCourier = await _courierrepo.RegisterCourier(courier);
            return Ok(new { message = "Verification link sent to email. Please verify and login.", courier = createdCourier });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering courier for {Email}", courierDto.CourierEmail);
            return StatusCode(500, new { error = "Something went wrong. Please try again." });
        }
    }

    [HttpGet("verify-courier-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        try
        {
            var pending = await _courierrepo.FindPendingCourierVerificationByToken(token);
            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
            {
                return BadRequest(new { error = "Invalid or expired verification link." });
            }

            var courierDto = JsonSerializer.Deserialize<CourierServiceDto>(pending.JsonPayload);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(courierDto.Password);

            var courier = new CourierService
            {
                CourierServiceName = courierDto.CourierServiceName,
                CourierWebsiteName = courierDto.CourierWebsiteName,
                CourierPhone = courierDto.CourierPhone,
                CourierEmail = courierDto.CourierEmail,
                Password = hashedPassword,
                IsEmailVerified = true, // or false if verification email flow is enabled
                RegisteredDate = DateTime.UtcNow,

                // Service types (multi-selection checkboxes)
                IsLocal = courierDto.IsLocal,
                IsState = courierDto.IsState,
                IsNational = courierDto.IsNational,
                IsInternational = courierDto.IsInternational
            };

            await _courierrepo.RegisterCourier(courier);
            await _courierrepo.DeletePendingCourierVerification(token);

            return Ok(new { message = "Courier email verified and registration completed!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying courier token: {Token}", token);
            return StatusCode(500, new { error = "Could not verify email. Try again later." });
        }
    }


    //Upload CVS file for branch locations ..chekc for valid data and preview

    [HttpPost("courierBranchUpload-preview")]
    public async Task<IActionResult> UploadCourierCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File missing");

        var preview = await _courierrepo.ParseAndValidateCsv(file);

        return Ok(preview);
    }

    //save to db
    [HttpPost("save-courier-branches")]
    public async Task<IActionResult> SaveCourierBranches([FromBody] List<CourierBranchCsvRowDto> rows)
    {
        if (rows == null || !rows.Any())
            return BadRequest(new { message = "No rows received." });

        // 1. Backend double-checks validity
        var invalidRows = rows
            .Where(r => !r.IsValid)
            .Select(r => r.RowNumber)
            .ToList();

        if (invalidRows.Any())
        {
            // Backend rejects the upload
            return BadRequest(new
            {
                message = "Some rows are invalid. Please upload a file where all rows are valid.",
                invalidRows = invalidRows
            });
        }

        try
        {
            // 2. All rows valid → Save to DB
            await _courierrepo.SaveCourierBranchesAsync(rows);

            return Ok(new
            {
                message = "All rows saved successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    //[HttpGet("GetBestCouriers")]
    //public async Task<IActionResult> GetBestCouriers(int busRegId, int shopperId, decimal productWeightKg)
    //{
    //    var business =  await _businessRepo.GetBusinessByIdAsync(busRegId);
    //    var shopper =   await _shopperRepo.GetShopperByIdAsync(shopperId);

    //    if (business == null || shopper == null)
    //        return NotFound("Invalid Business or Shopper ID.");

    //    var bestCouriers = _repository.GetBestCourierOptions(business, shopper, productWeightKg);
    //    return Ok(bestCouriers);
    //}

    [Authorize]
    [HttpGet("GetBestCourier")]
    public async Task<IActionResult> GetBestCourier(string storeCity, string storeState, string storeCountry, string shopperCity, decimal productWeightKg)
    {
        if (string.IsNullOrWhiteSpace(storeCity) ||
      string.IsNullOrWhiteSpace(storeState) ||
      string.IsNullOrWhiteSpace(storeCountry) ||
      string.IsNullOrWhiteSpace(shopperCity))
        {
            return BadRequest("All location fields are required.");
        }
        var result = await _courierrepo.GetBestCourierOptions(storeCity, storeState, storeCountry, shopperCity, productWeightKg);

        if (result == null || !result.Any())
            return NotFound("No suitable courier options found.");

        return Ok(result);
    }

    [Authorize]
    [HttpGet("AssignedOrdersByCourier")]
    public async Task<IActionResult> GetAssignedOrdersByCourier([FromQuery] int courierId)
    {
        if (courierId <= 0)
            return BadRequest("Invalid courier ID.");

        var orders = await _courierrepo.GetAssignedOrdersByCourierIdAsync(courierId);
        if (orders == null || !orders.Any())
            return NotFound("No assigned orders found for this courier.");

        return Ok(orders);
    }

}


