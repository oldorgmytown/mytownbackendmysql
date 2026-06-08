using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly AppDbContext _context;

        public GuestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool isTaken, string message)> IsEmailTakenAsync(string email)
        {
            var guest = await _context.GuestRegisters
                .FirstOrDefaultAsync(g => g.Email.ToLower() == email.ToLower());

            if (guest == null || guest.Status == "Deactivated")
                return (false, null);

            if (guest.Status == "Blocked")
                return (true, "This email is blocked. Please contact support.");

            return (true, null);
        }

        //  Changed from PendingVerification to PendingGuestVerification
        public async Task SavePendingVerificationAsync(PendingGuestVerification pending)
        {
            _context.PendingGuestVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        //  Changed from PendingVerification to PendingGuestVerification
        public async Task<PendingGuestVerification> FindPendingVerificationByTokenAsync(string token)
        {
            return await _context.PendingGuestVerifications
                .FirstOrDefaultAsync(p => p.Token == token);
        }

        //  Changed from PendingVerification to PendingGuestVerification
        public async Task<PendingGuestVerification> FindPendingVerificationByEmailAsync(string email)
        {
            return await _context.PendingGuestVerifications
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower()
                                       && p.ExpiryDate > DateTime.UtcNow);
        }

        // Changed from PendingVerification to PendingGuestVerification
        public async Task DeletePendingVerificationAsync(string token)
        {
            var pending = await _context.PendingGuestVerifications
                .FirstOrDefaultAsync(p => p.Token == token);

            if (pending != null)
            {
                _context.PendingGuestVerifications.Remove(pending);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GuestRegister> RegisterGuestAsync(GuestRegister guest)
        {
            _context.GuestRegisters.Add(guest);
            await _context.SaveChangesAsync();
            return guest;
        }

        public async Task<GuestRegister?> GetGuestByEmailAsync(string email)
        {
            return await _context.GuestRegisters
                .FirstOrDefaultAsync(g => g.Email.ToLower() == email.ToLower());
        }

        public async Task<GuestRegister?> GetGuestByIdAsync(int guestRegId)
        {
            return await _context.GuestRegisters
                .FirstOrDefaultAsync(g => g.GuestRegId == guestRegId);
        }
    }
}