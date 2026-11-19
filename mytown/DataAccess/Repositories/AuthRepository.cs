using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using static mytown.DataAccess.Repositories.AuthRepository;

namespace mytown.DataAccess.Repositories
{

    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthRepository(AppDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        public bool EmailExists(string email, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
                return false;

            email = email.Trim().ToLower();
            role = role.Trim().ToLower();

            return role switch
            {
                "shopper" => _context.ShopperRegisters
                    .Any(u => u.Email.ToLower() == email),

                "business" => _context.BusinessRegisters
                    .Any(u => u.BusEmail.ToLower() == email),

                "courier" => _context.CourierService
                    .Any(u => u.CourierEmail.ToLower() == email),

                "transporter" => _context.TransporterRegisters
                    .Any(u => u.Email.ToLower() == email),

                "sender" => _context.SenderRegisters
                    .Any(u => u.Email.ToLower() == email),

                _ => false
            };
        }


        public string CreatePasswordResetToken(string email, string role)
        {
            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddHours(1);

            var request = new PasswordResetRequest
            {
                Email = email,
                Token = token,
                Expiry = expiry,
                Role = role
            };

            _context.PasswordResetRequests.Add(request);
            _context.SaveChanges();

            return token;
        }

        public async Task SendResetEmail(string email,string role)
        {
            //if (!EmailExists(email))
            //    throw new Exception("Email not found.");

            var token = CreatePasswordResetToken(email,role);
            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            var resetLink = $"{frontendBaseUrl}?reset-password&token={token}";
            // var resetLink = $"{frontendBaseUrl}?reset=1&email={email}&token={token}";
            // var resetLink = $"{frontendBaseUrl}/reset-password?token={token}";
            // var resetLink = $"https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/reset-password?token={token}";

            await _emailService.SendPasswordResetEmail(email, resetLink);
        }


        public PasswordResetRequest GetResetRequestByToken(string token)
        {
            return _context.PasswordResetRequests
                           .FirstOrDefault(r => r.Token == token && r.Expiry > DateTime.UtcNow);
        }


        public bool ResetPassword(string email, string newPassword, string role)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            email = email.Trim().ToLower();
            role = role.Trim().ToLower();

            var hashedPassword = HashPassword(newPassword);

            switch (role)
            {
                case "shopper":
                    {
                        var shopper = _context.ShopperRegisters
                            .FirstOrDefault(u =>
                                u.Email.ToLower() == email);

                        if (shopper == null)
                            return false;

                        shopper.Password = hashedPassword;
                        break;
                    }

                case "business":
                    {
                        var business = _context.BusinessRegisters
                            .FirstOrDefault(u =>
                                u.BusEmail.ToLower() == email);

                        if (business == null)
                            return false;

                        business.Password = hashedPassword;
                        break;
                    }

                case "courier":
                    {
                        var courier = _context.CourierService
                            .FirstOrDefault(u =>
                                u.CourierEmail.ToLower() == email);

                        if (courier == null)
                            return false;

                        courier.Password = hashedPassword;
                        break;
                    }

                case "transporter":
                    {
                        var transporter = _context.TransporterRegisters
                            .FirstOrDefault(u =>
                                u.Email.ToLower() == email);

                        if (transporter == null)
                            return false;

                        transporter.Password = hashedPassword;
                        break;
                    }

                case "sender":
                    {
                        var sender = _context.SenderRegisters
                            .FirstOrDefault(u =>
                                u.Email.ToLower() == email);

                        if (sender == null)
                            return false;

                        sender.Password = hashedPassword;
                        break;
                    }

                default:
                    return false;
            }

            _context.SaveChanges();

            return true;
        }
        

        private string HashPassword(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return hashedPassword;
        }


        public async Task<bool> LogoutAsync(int userId, string sessionId, string userType)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionGuid == sessionId
                                          && s.UserId == userId
                                          && s.UserType == userType
                                          && s.IsActive);

            if (session == null)
                return false;

            session.IsActive = false;
            _context.UserSessions.Update(session);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserSession?> GetSessionBySessionIdAsync(string sessionId)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(x => x.SessionGuid == sessionId);
        }

        public async Task<bool> RevokeSessionAsync(string sessionId)
        {
            var session = await GetSessionBySessionIdAsync(sessionId);
            if (session == null) return false;

            session.IsActive = false;
            _context.UserSessions.Update(session);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
