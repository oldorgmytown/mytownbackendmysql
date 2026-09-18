using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using mytown.DataAccess.Interfaces;
using mytown.Services.Interfaces;
using mytown.Models;
using System.Threading.Tasks;

namespace mytown.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;

        public AuthService(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public bool EmailExists(string email, string role)
            => _authRepo.EmailExists(email,role);

        public void SendResetEmail(string email,string role)
            => _authRepo.SendResetEmail(email,role);

        //push
        public PasswordResetRequest GetResetRequestByToken(string token)
            => _authRepo.GetResetRequestByToken(token);

        public bool ResetPassword(string email, string newPassword, String role)
            => _authRepo.ResetPassword(email, newPassword,role);

        public Task<bool> LogoutAsync(int userId, string sessionId, string userType)
            => _authRepo.LogoutAsync(userId, sessionId, userType);

        public Task<bool> RevokeSessionAsync(string sessionId)
            => _authRepo.RevokeSessionAsync(sessionId);
    }
}
