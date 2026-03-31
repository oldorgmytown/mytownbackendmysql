using mytown.DataAccess.Interfaces;
using mytown.Services.Interfaces;
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

        public bool EmailExists(string email)
            => _authRepo.EmailExists(email);

        public void SendResetEmail(string email)
            => _authRepo.SendResetEmail(email);

        public object GetResetRequestByToken(string token)
            => _authRepo.GetResetRequestByToken(token);

        public bool ResetPassword(string email, string newPassword)
            => _authRepo.ResetPassword(email, newPassword);

        public Task<bool> LogoutAsync(int userId, string sessionId, string userType)
            => _authRepo.LogoutAsync(userId, sessionId, userType);

        public Task<bool> RevokeSessionAsync(string sessionId)
            => _authRepo.RevokeSessionAsync(sessionId);
    }
}
