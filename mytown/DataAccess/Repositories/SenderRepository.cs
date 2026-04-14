using Microsoft.EntityFrameworkCore;

using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Implementations
{
    public class SenderRepository : ISenderRepository
    {
        private readonly AppDbContext _context;

        public SenderRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---------------- EMAIL CHECK ----------------
        public async Task<(bool isTaken, string message)> IsEmailTaken(string email)
        {
            var sender = await _context.SenderRegisters
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());

            if (sender == null || sender.Status == "Deactivated")
                return (false, null);

            if (sender.Status == "Blocked")
                return (true, "This email is blocked. Please contact support.");

            return (true, null);
        }

        // ---------------- PENDING SENDER VERIFICATION ----------------
        public async Task SavePendingSenderVerification(PendingSenderVerification pending)
        {
            _context.PendingSenderVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingSenderVerification> FindPendingSenderVerificationByToken(string token)
        {
            return await _context.PendingSenderVerifications
                .FirstOrDefaultAsync(p => p.Token == token);
        }

        public async Task<PendingSenderVerification> FindPendingSenderVerificationByEmail(string email)
        {
            return await _context.PendingSenderVerifications
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower()
                                       && p.ExpiryDate > DateTime.UtcNow);
        }

        public async Task DeletePendingSenderVerification(string token)
        {
            var pending = await _context.PendingSenderVerifications
                .FirstOrDefaultAsync(p => p.Token == token);

            if (pending != null)
            {
                _context.PendingSenderVerifications.Remove(pending);
                await _context.SaveChangesAsync();
            }
        }

        // ---------------- REGISTER SENDER ----------------
        public async Task<SenderRegister> RegisterSender(SenderRegister sender)
        {
            try
            {
                _context.SenderRegisters.Add(sender);
                await _context.SaveChangesAsync();
                return sender;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database Update Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);

                throw new Exception("There was an error saving the sender registration to the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: " + ex.Message);
                throw new Exception("An unexpected error occurred during sender registration.");
            }
        }

        // ---------------- GET BY ID ----------------
        public async Task<SenderRegister> GetSenderByIdAsync(int senderRegId)
        {
            return await _context.SenderRegisters
                .FirstOrDefaultAsync(s => s.SenderRegId == senderRegId);
        }
    }
}