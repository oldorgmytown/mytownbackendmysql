using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ISenderService
    {
        Task<(bool success, string message)> RegisterSenderAsync(SenderRegisterDto dto);
        Task<(bool success, string message, int? senderRegId)> VerifyEmailAsync(string token);
        Task<(bool success, string message)> ResendVerificationEmailAsync(string email);
    }
}