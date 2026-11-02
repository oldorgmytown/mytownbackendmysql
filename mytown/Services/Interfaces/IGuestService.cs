using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IGuestService
    {
        // check if email already registered as shopper
        Task<(bool success, string message)> CheckEmailAsync(string email);
        Task<(bool success, string message, int? guestRegId)> RegisterGuestAsync(GuestRegisterDto dto);
        Task<(bool success, string message, int? guestRegId)> VerifyEmailAsync(string token);
        Task<(bool success, string message)> ResendVerificationEmailAsync(string email);
        Task<(bool success, string message, string? token, int? guestRegId)> LoginAsync(GuestLoginDto dto);
        Task<GuestDetailsDto> GetGuestDetailsAsync(int guestRegId);
    }
}