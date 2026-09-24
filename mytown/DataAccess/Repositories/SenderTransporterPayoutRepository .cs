using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace mytown.DataAccess.Repositories
{
    public class SenderTransporterPayoutRepository : ISenderTransporterPayoutRepository
    {
        private readonly AppDbContext _context;

        public SenderTransporterPayoutRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TransporterPayout?> GetPayoutBySenderOrderIdAsync(int senderOrderId)
        {
            return await _context.TransporterPayouts
                .FirstOrDefaultAsync(x => x.SenderOrderId == senderOrderId
                                        && x.OrderType == "SenderOrder");
        }

        public async Task<SenderTransporterPayoutDetailsDto?> GetTransporterPayoutDetailsAsync(int senderOrderId)
        {
            return await (
                from so in _context.SenderOrders
                join tad in _context.TransporterAccountDetails
                    on so.TransporterRegId equals tad.TransporterRegId
                where so.SenderOrderId == senderOrderId
                select new SenderTransporterPayoutDetailsDto
                {
                    TransporterRegId = so.TransporterRegId!.Value,
                    BeneficiaryId = tad.CashfreeBeneficiaryId,
                    Amount = so.TransporterCharges ?? 0
                }
            ).FirstOrDefaultAsync();
        }

        public async Task AddPayoutAsync(TransporterPayout payout)
        {
            await _context.TransporterPayouts.AddAsync(payout);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
