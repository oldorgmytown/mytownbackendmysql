using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

public class TransporterPayoutRepository : ITransporterPayoutRepository
{
    private readonly AppDbContext _context;

    public TransporterPayoutRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TransporterPayoutDetailsDto?> GetTransporterPayoutDetailsAsync(int storeOrderId)
    {
        // ADJUST: confirm which table links a StoreOrderId to a TransporterId
        // in your schema - mirrored here from ShippingDetails/CourierBranch,
        // change to whatever your actual Transporter order relationship is.
        return await _context.ShippingDetails
           .Where(x => x.StoreOrderId == storeOrderId && x.TransporterRegId.HasValue)
           .Select(x => new TransporterPayoutDetailsDto
           {
               StoreOrderId = x.StoreOrderId,
               TransporterRegId = x.TransporterRegId!.Value,

               // Currently using shipping cost as transporter payout
               Amount = x.Cost,

               BeneficiaryId = _context.TransporterAccountDetails
                   .Where(a => a.TransporterRegId == x.TransporterRegId.Value)
                   .Select(a => a.CashfreeBeneficiaryId)
                   .FirstOrDefault()
           })
           .FirstOrDefaultAsync();
    }

    public async Task<TransporterPayout?> GetPayoutByStoreOrderIdAsync(int storeOrderId)
    {
        return await _context.TransporterPayouts
            .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);
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