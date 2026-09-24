using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IGuestRepository
    {
        Task<(bool isTaken, string message)> IsEmailTakenAsync(string email);

       
        Task SavePendingVerificationAsync(PendingGuestVerification pending);
        Task<PendingGuestVerification> FindPendingVerificationByTokenAsync(string token);
        Task<PendingGuestVerification> FindPendingVerificationByEmailAsync(string email);
        Task DeletePendingVerificationAsync(string token);

        Task<GuestRegister> RegisterGuestAsync(GuestRegister guest);
        Task<GuestRegister?> GetGuestByEmailAsync(string email);
        Task<GuestRegister?> GetGuestByIdAsync(int guestRegId);
        Task<GuestDetailsDto> GetGuestDetailsByIdAsync(int guestRegId);
    }
}