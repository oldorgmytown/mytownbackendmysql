using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mytown.Models;
using mytown.Services;
using BCrypt.Net;
using System.Text.Json;
using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
using mytown.Controllers.Helpers;


namespace mytown.Controllers
{
    [Route("api/shoppers")]
    [ApiController]
    public class ShopperController : ControllerBase
    {
        private readonly IShopperRepository _shopperRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShopperController> _logger;
        private readonly IShopperRegistrationValidator _registrationValidator;
        private readonly IVerificationLinkBuilder _verificationLinkBuilder;

        public ShopperController(
            IShopperRepository shopperRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<ShopperController> logger,
            IShopperRegistrationValidator registrationValidator,
            IVerificationLinkBuilder verificationLinkBuilder)
        {
            _shopperRepository = shopperRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _registrationValidator = registrationValidator;
            _verificationLinkBuilder = verificationLinkBuilder;
        }

        /// <summary>
        /// Registers a new shopper.
        /// </summary>
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] ShopperRegisterDto shopperRegisterDto)
        //{
        //    // Validate the registration model using the custom validator
        //    List<string> validationErrors = _registrationValidator.Validate(shopperRegisterDto);
        //    if (validationErrors.Count > 0)
        //    {
        //        _logger.LogWarning("Registration validation errors for {Email}: {Errors}", shopperRegisterDto.Email, validationErrors);
        //        return BadRequest(new { errors = validationErrors });
        //    }

            //try
            //{
            //    // Hash the password from the DTO
            //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(shopperRegisterDto.Password);

            //    // Create a new shopper registration instance with the hashed password
            //    var newShopper = new ShopperRegister
            //    {
            //        Username = shopperRegisterDto.Username,
            //        Email = shopperRegisterDto.Email,
            //        Password = hashedPassword,
            //        Address = shopperRegisterDto.Address,
            //        Town = shopperRegisterDto.Town,
            //        City = shopperRegisterDto.City,
            //        State = shopperRegisterDto.State,
            //        Country = shopperRegisterDto.Country,
            //        PostalCode = shopperRegisterDto.PostalCode,
            //        PhoneNumber = shopperRegisterDto.PhoneNumber,
            //        PhotoName = shopperRegisterDto.PhotoName,
            //        IsEmailVerified = false
            //    };

            //    // Attempt to register the shopper
            //    var registeredShopper = await _shopperRepository.RegisterShopper(newShopper);
            //    if (registeredShopper == null)
            //    {
            //        _logger.LogWarning("Registration failed: Email {Email} is already in use.", shopperRegisterDto.Email);
            //        return Conflict(new { error = "This email address is already registered. Please try logging in or use a different email." });
            //    }

            //    // Generate an email verification token and retrieve the frontend base URL
            //    var verificationRecord = await _shopperRepository.GenerateEmailVerification(shopperRegisterDto.Email);
            //    string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            //    if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            //    {
            //        _logger.LogError("Frontend base URL is not configured.");
            //        return StatusCode(500, new { message = "The application configuration is missing the frontend URL. Please contact support." });
            //    }

            //    // Build the verification link using the provided builder service
            //    string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, verificationRecord.VerificationToken);
            //    _logger.LogInformation("Generated verification link for {Email}: {VerificationLink}", shopperRegisterDto.Email, verificationLink);

            //    // Send the verification email
            //    await _emailService.SendVerificationEmail(shopperRegisterDto.Email, verificationLink);
            //    _logger.LogInformation("Registration successful for {Email}. Verification email sent.", shopperRegisterDto.Email);

            //    return Ok(new { message = "Registration successful! Please check your email for the verification link. Once verified, you can log in." });
            //}

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] ShopperRegisterDto shopperRegisterDto)
            {
                List<string> validationErrors = _registrationValidator.Validate(shopperRegisterDto);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation failed for {Email}: {Errors}", shopperRegisterDto.Email, validationErrors);
                    return BadRequest(new { errors = validationErrors });
                }

                try
                {
                (bool isTaken, string statusMessage) = await _shopperRepository.IsEmailTaken(shopperRegisterDto.Email);

                if (statusMessage != null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { error = statusMessage });
                }

                if (isTaken)
                {
                    return Conflict(new { error = "This email is already registered. Try logging in instead." });
                }


                // Generate verification token and save to pending table
                string verificationToken = Guid.NewGuid().ToString();
                DateTime expiry = DateTime.UtcNow.AddHours(24);
                string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, verificationToken);

                // Serialize the registration DTO
                string jsonPayload = JsonSerializer.Serialize(shopperRegisterDto);

                // Save to PendingVerification table
                var pending = new PendingVerification
                {
                    Email = shopperRegisterDto.Email,
                    Token = verificationToken,
                    ExpiryDate = expiry,
                    JsonPayload = jsonPayload
                };
                await _shopperRepository.SavePendingVerification(pending);

                // Send verification email
                await _emailService.SendVerificationEmail(shopperRegisterDto.Email, verificationLink);

                //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(shopperRegisterDto.Password);

                //var newShopper = new ShopperRegister
                //{
                //    Username = shopperRegisterDto.Username,
                //    Email = shopperRegisterDto.Email,
                //    Password = hashedPassword,
                //    Address = shopperRegisterDto.Address,
                //    Town = shopperRegisterDto.Town,
                //    City = shopperRegisterDto.City,
                //    State = shopperRegisterDto.State,
                //    Country = shopperRegisterDto.Country,
                //    PostalCode = shopperRegisterDto.PostalCode,
                //    PhoneNumber = shopperRegisterDto.PhoneNumber,
                //    PhotoName = shopperRegisterDto.PhotoName,
                //    IsEmailVerified = true,
                //     ShopperRegDate = DateTime.UtcNow
                //};

                //await _shopperRepository.RegisterShopper(newShopper);

                _logger.LogInformation("Verification email sent to {Email}", shopperRegisterDto.Email);
                    return Ok(new { message = "Verification email sent! Please check your inbox to complete registration." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during registration for {Email}", shopperRegisterDto.Email);
                    return StatusCode(500, new { error = "Something went wrong. Please try again later." });
                }
            }

            [HttpGet("verify-shopper-email")]
            public async Task<IActionResult> VerifyEmail([FromQuery] string token)
            {
                try
                {
                    var pending = await _shopperRepository.FindPendingVerificationByToken(token);
                    if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                    {
                        return BadRequest(new { error = "Invalid or expired verification link." });
                    }

                    // Deserialize DTO
                    var shopperDto = JsonSerializer.Deserialize<ShopperRegisterDto>(pending.JsonPayload);
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(shopperDto.Password);

                    var newShopper = new ShopperRegister
                    {
                        Username = shopperDto.Username,
                        Email = shopperDto.Email,
                        Password = hashedPassword,
                        Address = shopperDto.Address,
                        Town = shopperDto.Town,
                        City = shopperDto.City,
                        State = shopperDto.State,
                        Country = shopperDto.Country,
                        PostalCode = shopperDto.PostalCode,
                        PhoneNumber = shopperDto.PhoneNumber,
                        PhotoName = shopperDto.PhotoName,
                        IsEmailVerified = true,
                        status = "Active"
                    };

                    await _shopperRepository.RegisterShopper(newShopper);
                    await _shopperRepository.DeletePendingVerification(token); // clean up

                    _logger.LogInformation("Email verified and shopper registered for {Email}", newShopper.Email);
                    return Ok(new { message = "Your email is verified and your account has been created!", shopperRegId = newShopper.ShopperRegId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error verifying token: {Token}", token);
                    return StatusCode(500, new { error = "Could not verify email. Please try again later." });
                }
        }


            /// <summary>
            //Resends the email verification link.
                /// </summary>
                [HttpPost("resend-verification")]
                public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendemailVerificationDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    _logger.LogWarning("Resend verification requested without email.");
                    return BadRequest(new { error = "Email is required." });
                }

                var existingVerification = await _shopperRepository.FindPendingVerificationByEmail(model.Email);
                if (existingVerification == null)
                {
                    return NotFound(new { error = "No pending verification found. Please register again." });
                }


                //string email = existingVerification.Email;
                //_logger.LogInformation("Resend verification requested for {Email} (old token: {Token})", email, token);

                // Remove old and create new
                await _shopperRepository.RemoveVerification(existingVerification);

                string token = Guid.NewGuid().ToString();
                DateTime expiry = DateTime.UtcNow.AddHours(24);
                var newVerification = new PendingVerification
                {
                    Email = model.Email,
                    Token = token,
                    ExpiryDate = expiry,
                    // JsonPayload = existingVerification.JsonPayload
                };
                await _shopperRepository.SavePendingVerification(newVerification);

                string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);
                await _emailService.SendVerificationEmail(model.Email, link);

                return Ok(new { message = $"New verification email sent to {model.Email}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend verification failed for {Email}", model.Email);
                return StatusCode(500, new { error = "Something went wrong. Please try again." });
            }
        }



        [HttpGet("GetTownsWithStoreCountByCountry/{country}")]
        public async Task<IActionResult> GetTownsWithStoreCountByCountry(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return BadRequest("Country is required.");
            }

            var result = await _shopperRepository.GetTownsWithStoreCountByCountryAsync(country);

            if (result == null || !result.Any())
            {
                return NotFound($"No towns found for country '{country}'.");
            }

            return Ok(result);
        }

        [HttpGet("productsrecentlyviewedbyshopper/{shopperId}")]
        public async Task<IActionResult> GetRecentlyViewed(int shopperId, int days = 7, int limit = 10)
        {
            var products = await _shopperRepository.GetRecentlyViewedProductsAsync(shopperId, days, limit);
            return Ok(products);
        }


        // Shopper Alternate Address

        [HttpGet("GetShopperAltAddress")]
        public async Task<IActionResult> GetAddresses(int shopperRegId)
        {
            var addresses = await _shopperRepository.GetAddressesByShopperIdAsync(shopperRegId);
            return Ok(addresses);
        }

        // POST: api/shopper/AddAltShopperAddress
        [HttpPost("AddAltShopperAddress")]
        public async Task<IActionResult> AddAddress([FromBody] ShopperAlternateAddressDto addressDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to entity
            var address = new ShopperAlternateAddress
            {
                ShopperRegId = addressDto.ShopperRegId,
                AltName = addressDto.AltName,
                AltPhoneNumber = addressDto.AltPhoneNumber,
                AltAddress = addressDto.AltAddress,
                AltTown = addressDto.AltTown,
                AltCity = addressDto.AltCity,
                AltState = addressDto.AltState,
                AltCountry = addressDto.AltCountry,
                AltPostalCode = addressDto.AltPostalCode,
                DeliveryNotes = addressDto.DeliveryNotes
            };

            var newAddress = await _shopperRepository.AddAddressAsync(address);

            // Map entity back to DTO for response
            var resultDto = new ShopperAlternateAddressDto
            {
                AltAddressId = newAddress.AltAddressId,
                ShopperRegId = address.ShopperRegId,
                AltName = newAddress.AltName,
                AltPhoneNumber = newAddress.AltPhoneNumber,
                AltAddress = newAddress.AltAddress,
                AltTown = newAddress.AltTown,
                AltCity = newAddress.AltCity,
                AltState = newAddress.AltState,
                AltCountry = newAddress.AltCountry,
                AltPostalCode = newAddress.AltPostalCode,
                DeliveryNotes = newAddress.DeliveryNotes
            };

            return CreatedAtAction(nameof(GetAddresses), new { shopperRegId = resultDto.AltAddressId }, resultDto);
        }

        // DELETE: api/shopper/DeleteShopperAltAddress/5
        [HttpDelete("DeleteShopperAltAddress/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var deleted = await _shopperRepository.DeleteAddressAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Alternate address with ID {id} not found." });

            return Ok(new { message = $"Alternate address with ID {id} has been successfully deleted." });
        }
    
    }
    }
