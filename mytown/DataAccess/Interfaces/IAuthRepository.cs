using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        string CreatePasswordResetToken(string email);
        Task SendResetEmail(string email);
        bool ResetPassword(string email, string newPassword);
        bool EmailExists(string email);

        PasswordResetRequest GetResetRequestByToken(string token);

        Task<bool> LogoutAsync(int userId, string sessionId, string userType);

        Task<UserSession?> GetSessionBySessionIdAsync(string sessionId);
        Task<bool> RevokeSessionAsync(string sessionId);

    }
}
