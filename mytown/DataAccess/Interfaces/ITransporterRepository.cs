using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface ITransporterRepository
    {
        Task SavePendingVerification(PendingTransporterVerification pending);
        Task<PendingTransporterVerification> FindPendingVerificationByToken(string token);
        Task DeletePendingVerification(string token);

        Task<(bool isTaken, string message)> IsEmailTaken(string email);

        Task<TransporterRegister> RegisterTransporter(TransporterRegister transporter);

        Task<PendingTransporterVerification> FindPendingVerificationByEmail(string email);
    }
}