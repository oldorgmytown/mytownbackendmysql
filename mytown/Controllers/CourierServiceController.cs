using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text.Json;

[ApiController]
[Route("api/courier")]
public class CourierController : ControllerBase
{
    private readonly ICourierServiceHandler _courierService;
    private readonly IEmailService _emailService;                 // still available for store notifications etc.
    private readonly IConfiguration _configuration;
    private readonly ILogger<CourierController> _logger;
    private readonly IBusinessRepository _businessRepo;
    private readonly IShopperRepository _shopperRepo;

    public CourierController(
        ICourierServiceHandler courierService,
        IBusinessRepository businessRepo,
        IShopperRepository shopperRepo,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<CourierController> logger)
    {
        _courierService = courierService;
        _businessRepo = businessRepo;
        _shopperRepo = shopperRepo;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCourier([FromBody] CourierServiceDto courierDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Register directly (no verification) — if you want verification, pass true.
            var created = await _courierService.RegisterCourierAsync(courierDto, sendVerification: false);
            if (created == null)
            {
                // null indicates email taken (or verification flow triggered)
                var taken = await _courierService.IsCourierEmailTakenAsync(courierDto.CourierEmail);
                if (taken)
                    return Conflict(new { error = "Email already registered. Try logging in." });

                return StatusCode(500, new { error = "Registration failed." });
            }

            return Ok(new { message = "Courier registered successfully.", courier = created });
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
            var created = await _courierService.VerifyCourierEmailAsync(token);

            if (created == null)
                return BadRequest(new { error = "Invalid or expired verification link." });

            return Ok(new { message = "Courier email verified and registration completed!", courier = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying courier token: {Token}", token);
            return StatusCode(500, new { error = "Could not verify email. Try again later." });
        }
    }

    // preview CSV
    [HttpPost("courierBranchUpload-preview")]
    public async Task<IActionResult> UploadCourierCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File missing");

        var preview = await _courierService.ParseAndValidateCsvAsync(file);
        return Ok(preview);
    }

    // save branches
    [HttpPost("save-courier-branches")]
    public async Task<IActionResult> SaveCourierBranches([FromBody] List<CourierBranchCsvRowDto> rows)
    {
        if (rows == null || !rows.Any())
            return BadRequest(new { message = "No rows received." });

        var invalidRows = rows.Where(r => !r.IsValid).Select(r => r.RowNumber).ToList();
        if (invalidRows.Any())
        {
            return BadRequest(new
            {
                message = "Some rows are invalid. Please upload a file where all rows are valid.",
                invalidRows = invalidRows
            });
        }

        try
        {
            var saved = await _courierService.SaveCourierBranchesAsync(rows);
            if (!saved)
                return BadRequest(new { message = "Failed to save branches." });

            return Ok(new { message = "All rows saved successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving courier branches");
            return BadRequest(new { message = ex.Message });
        }
    }

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

        var result = await _courierService.GetBestCourierOptionsAsync(storeCity, storeState, storeCountry, shopperCity, productWeightKg);

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

        var orders = await _courierService.GetAssignedOrdersByCourierIdAsync(courierId);
        if (orders == null || !orders.Any())
            return NotFound("No assigned orders found for this courier.");

        return Ok(orders);
    }
}
