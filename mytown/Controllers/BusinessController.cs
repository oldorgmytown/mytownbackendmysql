using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;
//using mytown.Controllers.Helpers;
using mytown.Services.Implementations;
using mytown.DataAccess.Interfaces;
using MyTown.Models;

namespace mytown.Controllers
{
    [Route("api/business")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BusinessController> _logger;
        private readonly IBusinessRegistrationValidator _registrationValidator;
        private readonly IVerificationLinkBuilderbusiness _verificationLinkBuilderbusiness;
        private readonly ITokenService _tokenService;

        public BusinessController(
            IBusinessService businessService,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<BusinessController> logger,
            IBusinessRegistrationValidator registrationValidator,
            IVerificationLinkBuilderbusiness verificationLinkBuilderbusiness,
            ITokenService tokenService)
        {
            _businessService = businessService;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _registrationValidator = registrationValidator;
            _verificationLinkBuilderbusiness = verificationLinkBuilderbusiness;
            _tokenService = tokenService;
        }

        // ================== REGISTER BUSINESS ===================
        [AllowAnonymous]
        [HttpPost("businessregister")]
        public async Task<IActionResult> RegisterBusiness([FromBody] BusinessRegisterDto businessRegisterDto)
        {
            var validationErrors = _registrationValidator.Validate(businessRegisterDto);
            if (validationErrors.Count > 0)
            {
                _logger.LogWarning("Validation failed for {Email}: {Errors}", businessRegisterDto.BusEmail, validationErrors);
                return BadRequest(new { errors = validationErrors });
            }

            try
            {
                if (await _businessService.IsEmailTaken(businessRegisterDto.BusEmail))
                {
                    return Conflict(new { error = "This email is already registered. Try logging in instead." });
                }

                string token = Guid.NewGuid().ToString();
                DateTime expiry = DateTime.UtcNow.AddHours(24);
                string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                string verificationLink = _verificationLinkBuilderbusiness.BuildLink(frontendBaseUrl, token);

                string jsonPayload = JsonSerializer.Serialize(businessRegisterDto);

                var pending = new PendingBusinessVerification
                {
                    Email = businessRegisterDto.BusEmail,
                    Token = token,
                    ExpiryDate = expiry,
                    JsonPayload = jsonPayload
                };

                await _businessService.SavePendingVerification(pending);
                await _emailService.SendVerificationEmail(businessRegisterDto.BusEmail, verificationLink);

                return Ok(new { message = "Verification email sent! Please check your inbox." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", businessRegisterDto.BusEmail);
                return StatusCode(500, new { error = "Something went wrong. Please try with new email." });
            }
        }

        // ================== VERIFY EMAIL ===================
        [AllowAnonymous]
        [HttpGet("verify-business-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var pending = await _businessService.FindPendingVerificationByToken(token);
                if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                {
                    return BadRequest(new { error = "Invalid or expired verification link." });
                }

                var businessDto = JsonSerializer.Deserialize<BusinessRegisterDto>(pending.JsonPayload);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(businessDto.Password);

                var newBusiness = new BusinessRegister
                {
                    BusinessUsername = businessDto.BusinessUsername,
                    BusinessName = businessDto.Businessname,
                    LicenseType = businessDto.LicenseType,
                    Gstin = businessDto.Gstin,
                    BusServId = businessDto.BusservId,
                    BusCatId = businessDto.BuscatId,
                    Town = businessDto.Town,
                    BusMobileNo = businessDto.BusMobileNo,
                    BusEmail = businessDto.BusEmail,
                    Address1 = businessDto.Address1,
                    Address2 = businessDto.Address2,
                    BusinessCity = businessDto.businessCity,
                    BusinessState = businessDto.businessState,
                    BusinessCountry = businessDto.businessCountry,
                    PostalCode = businessDto.postalCode,
                    Password = hashedPassword,
                    IsEmailVerified = true,
                    Currency = GetCurrencyByCountry(businessDto.businessCountry)
                };

                await _businessService.RegisterBusiness(newBusiness);

                //add bank account details to that table

                var bankDetails = new BusinessAccountDetail
                {
                    BusRegId = newBusiness.BusRegId,
                    AccountHolderName = businessDto.AccountHolderName,
                    BankName = businessDto.BankName,
                    AccountNumber = businessDto.AccountNumber,
                    IFSCCode = businessDto.IFSCCode,
                    CreatedDate = DateTime.UtcNow
                };

                await _businessService.SaveBusinessAccountDetails(bankDetails);

                var newProfile = new BusinessProfile
                {
                    BusRegId = newBusiness.BusRegId,
                    ProfileStatus = "Incomplete",
                    BusinessName = newBusiness.BusinessName,
                    BusinessLocation = $"{newBusiness.Town}, {newBusiness.BusinessCity}, {newBusiness.BusinessState}, {newBusiness.BusinessCountry}"
                };

                await _businessService.CreateProfile(newProfile);
                await _businessService.DeletePendingVerification(token);

                var sessionId = Guid.NewGuid().ToString();
                var jwtToken = _tokenService.GenerateToken(
                    newBusiness.BusRegId,
                    newBusiness.BusEmail,
                    "Business",
                    sessionId
                );

                return Ok(new
                {
                    message = "Your email is verified and your business account is created!",
                    busRegId = newBusiness.BusRegId,
                    token = jwtToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token: {Token}", token);
                return StatusCode(500, new { error = "Could not verify email. Please try again later." });
            }
        }

        private string GetCurrencyByCountry(string country)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "India", "INR" },
                { "United States", "USD" },
                { "UK", "GBP" },
                { "Germany", "EUR" },
                { "France", "EUR" },
                { "Australia", "AUD" },
                { "Canada", "CAD" }
            };

            return map.ContainsKey(country) ? map[country] : "INR";
        }

        // ================== CHECK EMAIL ===================
        [AllowAnonymous]
        [HttpPost("check-email")]
        public async Task<IActionResult> CheckBusinessEmail([FromBody] EmailCheckRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { error = "Email is required." });

            const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                return BadRequest(new { errors = new[] { "Please enter a valid email address." } });

            bool isTaken = await _businessService.IsEmailTaken(request.Email);

            if (isTaken)
                return Conflict(new { error = "This email is already registered. Try logging in instead." });

            return Ok(new { message = "Email is valid and available." });
        }

        // ================== RESEND VERIFICATION ===================
        [AllowAnonymous]
        [HttpPost("resend-business-verification")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendemailVerificationDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                    return BadRequest(new { error = "Email is required." });

                var pending = await _businessService.FindPendingVerificationByEmail(model.Email);
                if (pending == null)
                    return NotFound(new { error = "No pending verification found. Please register again." });

                await _businessService.DeletePendingVerification(pending.Token);

                string token = Guid.NewGuid().ToString();
                DateTime expiry = DateTime.UtcNow.AddHours(24);

                var newPending = new PendingBusinessVerification
                {
                    Email = model.Email,
                    Token = token,
                    ExpiryDate = expiry,
                    JsonPayload = pending.JsonPayload  // ← uncommented, carry forward
                };

                await _businessService.SavePendingVerification(newPending);

                string link = _verificationLinkBuilderbusiness.BuildLink(
                    _configuration["FrontendBaseUrl"],
                    token
                );

                await _emailService.SendVerificationEmail(model.Email, link);

                return Ok(new { message = $"New verification email sent to {model.Email}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend verification failed for {Email}", model.Email);
                return StatusCode(500, new { error = "Something went wrong. Please try again." });
            }
        }

        // ================== GET BUSINESS BY ID ===================
        [Authorize]
        [HttpGet("businessregister/{busRegId}")]
        public async Task<IActionResult> GetBusinessById(int busRegId)
        {
            try
            {
                var business = await _businessService.GetBusinessByIdAsync(busRegId);

                if (business == null)
                    return NotFound(new { error = "Business not found with the given BusRegId." });

                return Ok(business);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business with BusRegId {BusRegId}", busRegId);
                return StatusCode(500, new { error = "An error occurred while retrieving the business details." });
            }
        }

        // ================== CATEGORIES ===================
        //[Authorize]
        [HttpGet("BusinessCategories")]
        public async Task<ActionResult> GetBusinessCategories()
        {
            var categories = await _businessService.GetBusinessCategories();

            return Ok(new { value = categories });
        }


        [Authorize]
        [HttpGet("BusinessSubCategoriesforStores")]
        public async Task<ActionResult<IEnumerable<BusinessCategory>>> BusinessSubCategoriesforStores(int buscatid)
        {
            return Ok(await _businessService.BusinessSubCategoriesforStores(buscatid));
        }


        // ================== UPLOAD IMAGE ===================
        [Authorize]
        [HttpPost("upload_image")]
        public async Task<IActionResult> UploadImage(IFormFile file, string imageType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var newFileName = $"{imageType}_{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{Path.GetExtension(file.FileName)}";

            var blobClient = containerClient.GetBlobClient(newFileName);

using (var stream = file.OpenReadStream())
{
    var contentType = Path.GetExtension(file.FileName).ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".webp" => "image/webp",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        _ => "application/octet-stream"
    };

    await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
    {
        HttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
        {
            ContentType = contentType
        }
    });
}

            return Ok(new { FileName = newFileName, Url = blobClient.Uri.AbsoluteUri });
        }
    }
}
