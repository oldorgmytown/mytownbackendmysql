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


        public string CreatePasswordResetToken(string email)
        {
            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddHours(1);

            var request = new PasswordResetRequest
            {
                Email = email,
                Token = token,
                Expiry = expiry
            };

            _context.PasswordResetRequests.Add(request);
            _context.SaveChanges();

            return token;
        }

        public async Task SendResetEmail(string email)
        {
            //if (!EmailExists(email))
            //    throw new Exception("Email not found.");

            var token = CreatePasswordResetToken(email);
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


        public bool ResetPassword(string email, string newPassword)
        {
            //var request = _context.PasswordResetRequests
            //    .FirstOrDefault(r => r.Token == token && r.Expiry > DateTime.UtcNow);

            //if (request == null) return false;

            var shopper = _context.ShopperRegisters.FirstOrDefault(s => s.Email == email);
            var business = _context.BusinessRegisters.FirstOrDefault(b => b.BusEmail == email);
            var courier = _context.CourierService.FirstOrDefault(b => b.CourierEmail == email);


            if (shopper != null)
            {
                shopper.Password = HashPassword(newPassword); // Replace with your hashing
            }
            else if (business != null)
            {
                business.Password = HashPassword(newPassword);
            }
            else if (courier != null)
            {
                courier.Password = HashPassword(newPassword);
            }
            else
            {
                return false;
            }

            //  _context.PasswordResetRequests.Remove(email);
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
