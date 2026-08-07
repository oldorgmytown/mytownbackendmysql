using System.Threading.Tasks;

namespace mytown.Services.Interfaces
{
    public interface IAuthService
    {
        bool EmailExists(string email, string role);
        void SendResetEmail(string email);
        object GetResetRequestByToken(string token);
        bool ResetPassword(string email, string newPassword);
        Task<bool> LogoutAsync(int userId, string sessionId, string userType);
        Task<bool> RevokeSessionAsync(string sessionId);
    }
}
