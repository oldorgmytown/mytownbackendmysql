using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

public class CourierPayoutRepository : ICourierPayoutRepository
{
    private readonly AppDbContext _context;

    public CourierPayoutRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CourierPayoutDetailsDto?> GetCourierPayoutDetailsAsync(int storeOrderId)
    {
        return await _context.ShippingDetails
            .Where(x => x.StoreOrderId == storeOrderId)
            .Select(x => new CourierPayoutDetailsDto
            {
                StoreOrderId = x.StoreOrderId,
                CourierId = x.CourierBranch.CourierId,
                Amount = x.Cost,

                BeneficiaryId = _context.CourierAccountDetails
                    .Where(a => a.CourierId == x.CourierBranch.CourierId)
                    .Select(a => a.CashfreeBeneficiaryId)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CourierPayout?> GetPayoutByStoreOrderIdAsync(int storeOrderId)
    {
        return await _context.CourierPayouts
            .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);
    }

    public async Task AddPayoutAsync(CourierPayout payout)
    {
        await _context.CourierPayouts.AddAsync(payout);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}