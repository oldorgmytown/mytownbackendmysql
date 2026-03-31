using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ITransporterService
    {
      
            Task<(bool success, string message)> RegisterTransporterAsync(TransporterRegisterDto dto);

            Task<(bool success, string message, int? transporterRegId)> VerifyEmailAsync(string token);

            Task<(bool success, string message)> ResendVerificationEmailAsync(string email);
        
    }
}
