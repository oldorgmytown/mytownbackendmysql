using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;

public interface IStorePayoutRepository
{
    Task<StorePayoutDetailsDto?> GetStorePayoutDetailsAsync(
        int storeOrderId);

    Task<StorePayout?> GetPayoutByStoreOrderIdAsync(
        int storeOrderId);

    Task AddPayoutAsync(StorePayout payout);

    Task SaveAsync();
    Task<CourierPayoutDetailsDto?> GetCourierPayoutDetailsAsync(int storeOrderId);
    Task<CourierPayout?> GetPayouttoCourierByStoreOrderIdAsync(int storeOrderId);
    Task AddPayoutAsync(CourierPayout payout);
    // Task SaveAsync();

    Task<TransporterPayout?> GetPayouttransporterByStoreOrderIdAsync(int storeOrderId);

    Task<TransporterPayoutDetailsDto?> GetTransporterPayoutDetailsAsync(int storeOrderId);

    Task AddPayoutAsync(TransporterPayout payout);

   // Task SaveAsync();
}