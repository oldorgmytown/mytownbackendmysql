using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
   
        public interface ITransporterRepository
        {
            // ---------------- REGISTER ----------------
            Task<TransporterRegister> RegisterTransporter(TransporterRegister transporter);

            Task<(bool isTaken, string message)> IsEmailTaken(string email);

            // ---------------- RESEND EMAIL ----------------
            Task<TransporterVerification> FindPendingVerificationByEmail(string email);

            Task RemoveVerification(TransporterVerification verification);

            // ---------------- COMMON VERIFICATION ----------------
            Task SavePendingVerification(PendingVerification pending);

            Task<PendingVerification> FindPendingVerificationByToken(string token);

            Task DeletePendingVerification(string token);
        }
    
}
