using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text.Json;

namespace mytown.Services.Implementations
{
    public class ShopperService : IShopperService
    {
        private readonly IShopperRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShopperService> _logger;
        private readonly IVerificationLinkBuilder _verificationLinkBuilder;

        public ShopperService(
            IShopperRepository repo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<ShopperService> logger,
            IVerificationLinkBuilder verificationLinkBuilder)
        {
            _repo = repo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _verificationLinkBuilder = verificationLinkBuilder;
        }

        // ---------------- REGISTER ----------------
        public async Task<(bool success, string message)> RegisterShopperAsync(ShopperRegisterDto dto)
        {
            var (isTaken, statusMessage) = await _repo.IsEmailTaken(dto.Email);

            if (statusMessage != null)
                return (false, statusMessage);

            if (isTaken)
                return (false, "This email is already registered.");

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            var pending = new PendingVerification
            {
                Email = dto.Email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = JsonSerializer.Serialize(dto)
            };

            await _repo.SavePendingVerification(pending);
            await _emailService.SendVerificationEmail(dto.Email, link);

            return (true, "Verification email sent.");
        }

        // ---------------- VERIFY EMAIL ----------------
        public async Task<(bool success, string message, int? shopperRegId)> VerifyEmailAsync(string token)
        {
            var pending = await _repo.FindPendingVerificationByToken(token);

            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                return (false, "Invalid or expired verification link.", null);

            var dto = JsonSerializer.Deserialize<ShopperRegisterDto>(pending.JsonPayload);

            var shopper = new ShopperRegister
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
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

            await _repo.RegisterShopper(shopper);
            await _repo.DeletePendingVerification(token);

            return (true, "Email verified successfully.", shopper.ShopperRegId);
        }

        // ---------------- RESEND EMAIL ----------------
        public async Task<(bool success, string message)> ResendVerificationEmailAsync(string email)
        {
            var existing = await _repo.FindPendingVerificationByEmail(email);

            if (existing == null)
                return (false, "No pending verification found.");

            await _repo.RemoveVerification(existing);

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            var pending = new PendingVerification
            {
                Email = email,
                Token = token,
                ExpiryDate = expiry
            };

            await _repo.SavePendingVerification(pending);

            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            await _emailService.SendVerificationEmail(email, link);

            return (true, "Verification email resent.");
        }

        // ---------------- OTHER DATA ----------------
        public Task<IEnumerable<object>> GetTownsWithStoreCountByCountryAsync(string country)
            => _repo.GetTownsWithStoreCountByCountryAsync(country);

        public Task<IEnumerable<ProdcVariantforShopperDto>> GetRecentlyViewedProductsAsync(
            int shopperId, int days, int limit)
            => _repo.GetRecentlyViewedProductsAsync(shopperId, days, limit);

        // ---------------- ALTERNATE ADDRESS ----------------
        public Task<IEnumerable<ShopperAlternateAddressDto>> GetAddressesAsync(int shopperRegId)
            => _repo.GetAddressesByShopperIdAsync(shopperRegId);

        public async Task<ShopperAlternateAddressDto> AddAddressAsync(ShopperAlternateAddressDto dto)
        {
            var entity = new ShopperAlternateAddress
            {
                ShopperRegId = dto.ShopperRegId,
                AltName = dto.AltName,
                AltPhoneNumber = dto.AltPhoneNumber,
                AltAddress = dto.AltAddress,
                AltTown = dto.AltTown,
                AltCity = dto.AltCity,
                AltState = dto.AltState,
                AltCountry = dto.AltCountry,
                AltPostalCode = dto.AltPostalCode,
                DeliveryNotes = dto.DeliveryNotes
            };

            return await _repo.AddAddressAsync(entity);
        }

        public Task<bool> DeleteAddressAsync(int id)
            => _repo.DeleteAddressAsync(id);
    }
}
