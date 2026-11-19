using mytown.Models;
using mytown.Models.DTO_s;

public interface ITransporterPayoutRepository
{
    Task<TransporterPayoutDetailsDto?> GetTransporterPayoutDetailsAsync(int storeOrderId);
    Task<TransporterPayout?> GetPayoutByStoreOrderIdAsync(int storeOrderId);
    Task AddPayoutAsync(TransporterPayout payout);
    Task SaveAsync();
}