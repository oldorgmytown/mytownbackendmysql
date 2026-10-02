using Microsoft.EntityFrameworkCore;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

public class StorePayoutRepository : IStorePayoutRepository
{
    private readonly AppDbContext _context;

    public StorePayoutRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StorePayoutDetailsDto?>
        GetStorePayoutDetailsAsync(int storeOrderId)
    {
        var result = await _context.StoreOrders
            .Where(x => x.StoreOrderId == storeOrderId)
            .Select(x => new StorePayoutDetailsDto
            {
                StoreOrderId = x.StoreOrderId,

                StoreId = x.StoreId,

                Amount = x.StoreTotalAmount,

                BeneficiaryId = _context.BusinessAccountDetails
                    .Where(b => b.BusRegId == x.StoreId)
                    .Select(b => b.CashfreeBeneficiaryId)
                    .FirstOrDefault(),

                BeneficiaryStatus = _context.BusinessAccountDetails
                    .Where(b => b.BusRegId == x.StoreId)
                    .Select(b => b.CashfreeBeneficiaryStatus)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        return result;
    }


    public async Task<StorePayout?>
        GetPayoutByStoreOrderIdAsync(int storeOrderId)
    {
        return await _context.StorePayouts
            .FirstOrDefaultAsync(
                x => x.StoreOrderId == storeOrderId);
    }


    public async Task AddPayoutAsync(StorePayout payout)
    {
        await _context.StorePayouts.AddAsync(payout);
    }


    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
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

    public async Task<CourierPayout?> GetPayouttoCourierByStoreOrderIdAsync(int storeOrderId)
    {
        return await _context.CourierPayouts
            .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);
    }

    public async Task AddPayoutAsync(CourierPayout payout)
    {
        await _context.CourierPayouts.AddAsync(payout);
    }

    //public async Task SaveAsync()
    //{
    //    await _context.SaveChangesAsync();
    //}

    public async Task<TransporterPayout?> GetPayouttransporterByStoreOrderIdAsync(int storeOrderId)
    {
        return await _context.TransporterPayouts
            .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);
    }

    public async Task<TransporterPayoutDetailsDto?> GetTransporterPayoutDetailsAsync(int storeOrderId)
    {
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

    public async Task AddPayoutAsync(TransporterPayout payout)
    {
        await _context.TransporterPayouts.AddAsync(payout);
    }
}