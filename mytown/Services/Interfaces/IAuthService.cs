using System.Threading.Tasks;
using mytown.Models;

namespace mytown.Services.Interfaces
{
    public interface IAuthService
    {
        bool EmailExists(string email, string role);
        void SendResetEmail(string email, string role);
        PasswordResetRequest GetResetRequestByToken(string token);
        bool ResetPassword(string email, string newPassword, string role);
        Task<bool> LogoutAsync(int userId, string sessionId, string userType);
        Task<bool> RevokeSessionAsync(string sessionId);
    }
}
