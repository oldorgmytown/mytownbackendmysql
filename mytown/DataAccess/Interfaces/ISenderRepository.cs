using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface ISenderRepository
    {
        Task<(bool isTaken, string message)> IsEmailTaken(string email);

        Task SavePendingSenderVerification(PendingSenderVerification pending);
        Task<PendingSenderVerification> FindPendingSenderVerificationByToken(string token);
        Task<PendingSenderVerification> FindPendingSenderVerificationByEmail(string email);
        Task DeletePendingSenderVerification(string token);

        Task<SenderRegister> RegisterSender(SenderRegister sender);

        Task<SenderRegister> GetSenderByIdAsync(int senderRegId);
    }
}