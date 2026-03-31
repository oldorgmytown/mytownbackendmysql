using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class TransporterRepository : ITransporterRepository
    {
        private readonly AppDbContext _context;

        public TransporterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SavePendingVerification(PendingTransporterVerification pending)
        {
            _context.PendingTransporterVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingTransporterVerification> FindPendingVerificationByToken(string token)
        {
            return await _context.PendingTransporterVerifications
                .FirstOrDefaultAsync(p => p.Token == token);
        }

        public async Task DeletePendingVerification(string token)
        {
            var pending = await _context.PendingTransporterVerifications
                .FirstOrDefaultAsync(p => p.Token == token);

            if (pending != null)
            {
                _context.PendingTransporterVerifications.Remove(pending);
                await _context.SaveChangesAsync();
            }
        }

        // ---------------- EMAIL CHECK ----------------

        public async Task<(bool isTaken, string message)> IsEmailTaken(string email)
        {
            var transporter = await _context.TransporterRegisters
                .FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower());

            if (transporter == null || transporter.Status == "Deactivated")
                return (false, null);

            if (transporter.Status == "Blocked")
                return (true, "This email is blocked. Please contact support.");

            return (true, null);
        }

        // ---------------- REGISTER ----------------

        public async Task<TransporterRegister> RegisterTransporter(TransporterRegister transporter)
        {
            try
            {
                _context.TransporterRegisters.Add(transporter);
                await _context.SaveChangesAsync();
                return transporter;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database Update Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);

                throw new Exception("There was an error saving the transporter registration to the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: " + ex.Message);
                throw new Exception("An unexpected error occurred during transporter registration.");
            }
        }

        // ---------------- RESEND EMAIL ----------------

        public async Task<TransporterVerification> FindPendingVerificationByEmail(string email)
        {
            return await _context.TransporterVerification
                .Include(tv => tv.Transporter)
                .Where(tv => tv.Transporter.Email == email
                             && !tv.IsUsed
                             && tv.ExpiryDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveVerification(TransporterVerification verification)
        {
            _context.TransporterVerification.Remove(verification);
            await _context.SaveChangesAsync();
        }
    }
}
