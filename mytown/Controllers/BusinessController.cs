using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services;
using MyTown.Models;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace mytown.Controllers
{
    [Route("api/business")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BusinessController> _logger;
        private readonly IBusinessRegistrationValidator _registrationValidator;
        private readonly IVerificationLinkBuilderbusiness _verificationLinkBuilderbusiness;

        public BusinessController(IBusinessRepository businessRepository, 
            IEmailService emailService,
          IConfiguration configuration,
          ILogger<BusinessController> logger,
          IBusinessRegistrationValidator registrationValidator,
          IVerificationLinkBuilderbusiness verificationLinkBuilderbusiness)
        {
            _businessRepository = businessRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _registrationValidator = registrationValidator;
            _verificationLinkBuilderbusiness = verificationLinkBuilderbusiness;
        }
        [AllowAnonymous]
        [HttpPost("businessregister")]
        public async Task<IActionResult> RegisterBusiness([FromBody] BusinessRegisterDto businessRegisterDto)
        {
            List<string> validationErrors = _registrationValidator.Validate(businessRegisterDto);
            if (validationErrors.Count > 0)
            {
                _logger.LogWarning("Validation failed for {Email}: {Errors}", businessRegisterDto.BusEmail, validationErrors);
                return BadRequest(new { errors = validationErrors });
            }

            try
            {
                if (await _businessRepository.IsEmailTaken(businessRegisterDto.BusEmail))
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

                await _businessRepository.SavePendingVerification(pending);
                await _emailService.SendVerificationEmail(businessRegisterDto.BusEmail, verificationLink);

                //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(businessRegisterDto.Password);

                //var newBusiness = new BusinessRegister
                //{
                //    BusinessUsername = businessRegisterDto.BusinessUsername,
                //    Businessname = businessRegisterDto.Businessname,
                //    LicenseType = businessRegisterDto.LicenseType,
                //    Gstin = businessRegisterDto.Gstin,
                //    BusservId = businessRegisterDto.BusservId,
                //    BuscatId = businessRegisterDto.BuscatId,
                //    Town = businessRegisterDto.Town,
                //    BusMobileNo = businessRegisterDto.BusMobileNo,
                //    BusEmail = businessRegisterDto.BusEmail,
                //    Address1 = businessRegisterDto.Address1,
                //    Address2 = businessRegisterDto.Address2,
                //    businessCity = businessRegisterDto.businessCity,
                //    businessState = businessRegisterDto.businessState,
                //    businessCountry = businessRegisterDto.businessCountry,
                //    postalCode = businessRegisterDto.postalCode,
                //    Password = hashedPassword,
                //    IsEmailVerified = true,
                //    BusinessRegDate = DateTime.UtcNow
                //};

                //await _businessRepository.RegisterBusiness(newBusiness);

                _logger.LogInformation("Verification email sent to {Email}", businessRegisterDto.BusEmail);
                return Ok(new { message = "Verification email sent! Please check your inbox." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", businessRegisterDto.BusEmail);
                return StatusCode(500, new { error = "Something went wrong. Please try with new email." });
            }
        }
        [AllowAnonymous]
        [HttpGet("verify-business-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var pending = await _businessRepository.FindPendingVerificationByToken(token);
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

                await _businessRepository.RegisterBusiness(newBusiness);

                // adding busprfile id soon after verification
                var newProfile = new BusinessProfile
                {
                    BusRegId = newBusiness.BusRegId,
                    ProfileStatus = "Incomplete",
                    BusinessName = newBusiness.BusinessName,
                    BusinessLocation = $"{newBusiness.Town}, {newBusiness.BusinessCity}, {newBusiness.BusinessState}, {newBusiness.BusinessCountry}",
                    BusServId = newBusiness.BusServId,
                    BusCatId = newBusiness.BusCatId
                    // CreatedAt = DateTime.UtcNow
                };

                await _businessRepository.CreateProfile(newProfile);
                await _businessRepository.DeletePendingVerification(token);

                _logger.LogInformation("Email verified and business registered for {Email}", newBusiness.BusEmail);
                return Ok(new { message = "Your email is verified and your business account is created!", busRegId = newBusiness.BusRegId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token: {Token}", token);
                return StatusCode(500, new { error = "Could not verify email. Please try again later." });
            }
        }

        private string GetCurrencyByCountry(string country)
        {
            var countryCurrencyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "India", "INR" },
        { "United States", "USD" },
        { "UK", "GBP" },
        { "Germany", "EUR" },
        { "France", "EUR" },
        { "Australia", "AUD" },
        { "Canada", "CAD" },
        // add more as needed
    };

            return countryCurrencyMap.TryGetValue(country, out var currency) ? currency : "INR"; // default fallback
        }
        [AllowAnonymous]
        [HttpPost("check-email")]
        public async Task<IActionResult> CheckBusinessEmail([FromBody] EmailCheckRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { error = "Email is required." });
            }

            var errors = new List<string>();

            // Validate email format
            const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
            {
                errors.Add("Please enter a valid email address.");
            }

            if (errors.Count > 0)
            {
                _logger.LogWarning("Email validation failed: {Email}, Errors: {Errors}", request.Email, errors);
                return BadRequest(new { errors });
            }

            bool isTaken = await _businessRepository.IsEmailTaken(request.Email);
            if (isTaken)
            {
                return Conflict(new { error = "This email is already registered. Try logging in instead." });
            }

            return Ok(new { message = "Email is valid and available." });
        }

        [AllowAnonymous]
        [HttpPost("resend-business-verification")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendemailVerificationDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    _logger.LogWarning("Resend verification requested without email.");
                    return BadRequest(new { error = "Email is required." });
                }

                var existingVerification = await _businessRepository.FindPendingVerificationByEmail(model.Email);
                if (existingVerification == null)
                {
                    return NotFound(new { error = "No pending verification found. Please register again." });
                }

                // Remove old and create new
                await _businessRepository.RemoveVerification(existingVerification);

                string token = Guid.NewGuid().ToString();
                DateTime expiry = DateTime.UtcNow.AddHours(24);
                var newVerification = new PendingBusinessVerification
                {
                    Email = model.Email,
                    Token = token,
                    ExpiryDate = expiry,
                   // JsonPayload = existingVerification.JsonPayload
                };
                await _businessRepository.SavePendingVerification(newVerification);

                string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                string link = _verificationLinkBuilderbusiness.BuildLink(frontendBaseUrl, token);
                await _emailService.SendVerificationEmail(model.Email, link);

                return Ok(new { message = $"New verification email sent to {model.Email}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend verification failed for {Email}", model.Email);
                return StatusCode(500, new { error = "Something went wrong. Please try again." });
            }
        }

        //get business owner home page with busregid
        [Authorize]
        [HttpGet("businessregister/{busRegId}")]
        public async Task<IActionResult> GetBusinessById(int busRegId)
        {
            try
            {
                var business = await _businessRepository.GetBusinessByIdAsync(busRegId);

                if (business == null)
                {
                    return NotFound(new { error = "Business not found with the given BusRegId." });
                }

                return Ok(business);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business with BusRegId {BusRegId}", busRegId);
                return StatusCode(500, new { error = "An error occurred while retrieving the business details." });
            }
        }

        [Authorize]
        [HttpGet("BusinessCategories")]
        public async Task<ActionResult<IEnumerable<BusinessCategory>>> GetBusinessCategories()
        {
            var categories = await _businessRepository.GetBusinessCategories();
            return Ok(categories);
        }

        [Authorize]
        [HttpGet("BusinessSubCategoriesforStores")]
        public async Task<ActionResult<IEnumerable<BusinessCategory>>> BusinessSubCategoriesforStores(int buscatid)
        {
            var subcategories = await _businessRepository.BusinessSubCategoriesforStores(buscatid);
            return Ok(subcategories);
        }



        // Upload image to blob
        [Authorize]
        [HttpPost("upload_image")]
        public async Task<IActionResult> UploadImage(IFormFile file, string imageType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            // Create a blob container client
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create container if not exists (optional)
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var newFileName = $"{imageType}_{fileNameWithoutExtension}_{timestamp}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(newFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            var blobUrl = blobClient.Uri.AbsoluteUri;

            return Ok(new { FileName = newFileName, Url = blobUrl });
        }

    }

}
