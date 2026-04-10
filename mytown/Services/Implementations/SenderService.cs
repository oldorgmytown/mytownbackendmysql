using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text.Json;

namespace mytown.Services.Implementations
{
    public class SenderService : ISenderService
    {
        private readonly ISenderRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SenderService> _logger;
        private readonly IVerficationLinkBuildersender _verificationLinkBuilder;

        public SenderService(
            ISenderRepository repo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<SenderService> logger,
            IVerficationLinkBuildersender verificationLinkBuilder)
        {
            _repo = repo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _verificationLinkBuilder = verificationLinkBuilder;
        }

        // ---------------- REGISTER ----------------
        public async Task<(bool success, string message)> RegisterSenderAsync(SenderRegisterDto dto)
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

            var pending = new PendingSenderVerification
            {
                Email = dto.Email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = JsonSerializer.Serialize(dto)
            };

            await _repo.SavePendingSenderVerification(pending);
            await _emailService.SendVerificationEmail(dto.Email, link);

            return (true, "Verification email sent.");
        }

        // ---------------- VERIFY EMAIL ----------------
        public async Task<(bool success, string message, int? senderRegId)> VerifyEmailAsync(string token)
        {
            var pending = await _repo.FindPendingSenderVerificationByToken(token);

            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                return (false, "Invalid or expired verification link.", null);

            var dto = JsonSerializer.Deserialize<SenderRegisterDto>(pending.JsonPayload);

            var sender = new SenderRegister
            {
                SenderName = dto.SenderName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Address = dto.Address,
                Town = dto.Town,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                PhoneNumber = dto.PhoneNumber,
                IsEmailVerified = true,
                Status = "Active"
            };

            await _repo.RegisterSender(sender);
            await _repo.DeletePendingSenderVerification(token);

            return (true, "Email verified successfully.", sender.SenderRegId);
        }

        // ---------------- RESEND EMAIL ----------------
        public async Task<(bool success, string message)> ResendVerificationEmailAsync(string email)
        {
            var existing = await _repo.FindPendingSenderVerificationByEmail(email);

            if (existing == null)
                return (false, "No pending verification found.");

            await _repo.DeletePendingSenderVerification(existing.Token);

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            var pending = new PendingSenderVerification
            {
                Email = email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = existing.JsonPayload
            };

            await _repo.SavePendingSenderVerification(pending);

            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            await _emailService.SendVerificationEmail(email, link);

            return (true, "Verification email resent.");
        }
    }
}