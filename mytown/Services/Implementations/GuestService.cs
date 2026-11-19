using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text.Json;

namespace mytown.Services.Implementations
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GuestService> _logger;
        private readonly IVerificationLinkBuilderGuest _verificationLinkBuilder;
        private readonly ITokenService _tokenService;

        public GuestService(
            IGuestRepository repo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<GuestService> logger,
            IVerificationLinkBuilderGuest verificationLinkBuilder,
            ITokenService tokenService)
        {
            _repo = repo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _verificationLinkBuilder = verificationLinkBuilder;
            _tokenService = tokenService;
        }

        // ---------------- REGISTER ----------------

        public async Task<(bool success, string message)> CheckEmailAsync(string email)
        {
            var (isTaken, statusMessage) = await _repo.IsEmailTakenAsync(email);

            if (statusMessage != null)
                return (false, statusMessage);

            if (isTaken)
                return (false,
                    "This email is already registered as Shopper with ItIsMyTown. Please Login and continue as Shopper");

            return (true, "Email is available");
        }

        // returns guestRegId as well
        public async Task<(bool success, string message, int? guestRegId)> RegisterGuestAsync(GuestRegisterDto dto)
        {
            var (isTaken, statusMessage) = await _repo.IsEmailTakenAsync(dto.Email);

            if (statusMessage != null)
                return (false, statusMessage, null);

            if (isTaken)
                return (false, "This email is already registered as Shopper with ItIsMyTown. Please Login and continue as Shopper", null);

            var guest = new GuestRegister
            {
                Username = dto.Username,
                Email = dto.Email,
                Address = dto.Address,
                Town = dto.Town,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                PhoneNumber = dto.PhoneNumber,
                PhotoName = dto.PhotoName,
                IsEmailVerified = true,
                Status = "Active"
            };

            await _repo.RegisterGuestAsync(guest);

            return (true, "Guest registered successfully.", guest.GuestRegId);
        }

        // ---------------- VERIFY EMAIL (not needed, kept for interface) ----------------
        public async Task<(bool success, string message, int? guestRegId)> VerifyEmailAsync(string token)
        {
            return (true, "Email verification not required.", null);
        }

        // ---------------- RESEND EMAIL (not needed, kept for interface) ----------------
        public async Task<(bool success, string message)> ResendVerificationEmailAsync(string email)
        {
            return (true, "Email verification not required.");
        }

        // ---------------- LOGIN ----------------
        public async Task<(bool success, string message, string? token, int? guestRegId)> LoginAsync(GuestLoginDto dto)
        {
            var guest = await _repo.GetGuestByEmailAsync(dto.Email);

            if (guest == null)
                return (false, "Email not found.", null, null);

            if (guest.Status == "Blocked")
                return (false, "Your account is blocked. Please contact support.", null, null);

            if (!guest.IsEmailVerified)
                return (false, "Please verify your email before logging in.", null, null);

            string sessionId = Guid.NewGuid().ToString();
            var token = _tokenService.GenerateToken(guest.GuestRegId, guest.Email, "Guest", sessionId);

            return (true, "Login successful.", token, guest.GuestRegId);
        }

        // ---------------- GET GUEST DETAILS ----------------
        public async Task<GuestDetailsDto> GetGuestDetailsAsync(int guestRegId)
        {
            return await _repo.GetGuestDetailsByIdAsync(guestRegId);
        }
    }
}