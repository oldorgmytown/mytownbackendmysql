using mytown.Models.DTO_s;

namespace mytown.DataAccess.Repositories
{
    public interface IMobileAuthRepository
    {
        Task<(bool success, string message)> SignupAsync(MobileSignupDto dto);
        Task<(bool success, string message)> SendOtpAsync(string email, string role);
        Task<(bool success, string message)> VerifyOtpAsync(string email, string otp, string role);
    }
}