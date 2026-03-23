using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Text.Json;

namespace mytown.Services.Implementations
{
    public class TransporterService : ITransporterService
    {
        private readonly ITransporterRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TransporterService> _logger;
        private readonly IVerificationLinkBuilder _verificationLinkBuilder;

        public TransporterService(
            ITransporterRepository repo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<TransporterService> logger,
            IVerificationLinkBuilder verificationLinkBuilder)
        {
            _repo = repo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _verificationLinkBuilder = verificationLinkBuilder;
        }

        // ---------------- REGISTER ----------------
        public async Task<(bool success, string message)> RegisterTransporterAsync(TransporterRegisterDto dto)
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
        public async Task<(bool success, string message, int? transporterRegId)> VerifyEmailAsync(string token)
        {
            var pending = await _repo.FindPendingVerificationByToken(token);

            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                return (false, "Invalid or expired verification link.", null);

            var dto = JsonSerializer.Deserialize<TransporterRegisterDto>(pending.JsonPayload);

            var transporter = new TransporterRegister
            {
                TransporterName = dto.TransporterName,
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
                Status = "Active",
                TransporeterRegDate = DateTime.UtcNow
            };

            await _repo.RegisterTransporter(transporter);
            await _repo.DeletePendingVerification(token);

            return (true, "Email verified successfully.", transporter.TransporterRegId);
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
    }
}

