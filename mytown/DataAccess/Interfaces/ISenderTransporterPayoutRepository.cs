using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface ISenderTransporterPayoutRepository
    {
        Task<TransporterPayout?> GetPayoutBySenderOrderIdAsync(int senderOrderId);
        Task<SenderTransporterPayoutDetailsDto?> GetTransporterPayoutDetailsAsync(int senderOrderId);
        Task AddPayoutAsync(TransporterPayout payout);
        Task SaveAsync();
    }
}
