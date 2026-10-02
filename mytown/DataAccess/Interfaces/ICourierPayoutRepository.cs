using mytown.Models;
using mytown.Models.DTO_s;

public interface ICourierPayoutRepository
{
    Task<CourierPayoutDetailsDto?> GetCourierPayoutDetailsAsync(int storeOrderId);

    Task<CourierPayout?> GetPayoutByStoreOrderIdAsync(int storeOrderId);

    Task AddPayoutAsync(CourierPayout payout);

    Task SaveAsync();
}