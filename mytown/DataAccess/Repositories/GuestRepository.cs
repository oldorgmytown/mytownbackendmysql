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
            // checking email is already registered as shopper or not
            var shopper = await _context.ShopperRegisters
                .FirstOrDefaultAsync(g => g.Email.ToLower() == email.ToLower());

            if (shopper == null || shopper.Status == "Deactivated")
                return (false, null);

            if (shopper.Status == "Blocked")
                return (true, "This email is blocked. Please contact support.");

            return (true, null);
        }

        public async Task SavePendingVerificationAsync(PendingGuestVerification pending)
        {
            _context.PendingGuestVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingGuestVerification> FindPendingVerificationByTokenAsync(string token)
        {
            return await _context.PendingGuestVerifications
                .FirstOrDefaultAsync(p => p.Token == token);
        }

        public async Task<PendingGuestVerification> FindPendingVerificationByEmailAsync(string email)
        {
            return await _context.PendingGuestVerifications
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower()
                                       && p.ExpiryDate > DateTime.UtcNow);
        }

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
            //getting latest guest registration by email
            return await _context.GuestRegisters
                .Where(g => g.Email == email)
                .OrderByDescending(g => g.GuestRegId)
                .FirstOrDefaultAsync();
        }

        public async Task<GuestRegister?> GetGuestByIdAsync(int guestRegId)
        {
            return await _context.GuestRegisters
                .FirstOrDefaultAsync(g => g.GuestRegId == guestRegId);
        }

        //  New method - Get guest details by ID
        public async Task<GuestDetailsDto> GetGuestDetailsByIdAsync(int guestRegId)
        {
            return await _context.GuestRegisters
                .Where(g => g.GuestRegId == guestRegId)
                .Select(g => new GuestDetailsDto
                {
                    GuestRegId = g.GuestRegId,
                    Username = g.Username,
                    Email = g.Email,
                    PhoneNumber = g.PhoneNumber,
                    Address = g.Address,
                    Town = g.Town,
                    City = g.City,
                    State = g.State,
                    Country = g.Country,
                    PostalCode = g.PostalCode
                })
                .FirstOrDefaultAsync();
        }
    }
}