using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IHubRepository
    {

        Task<List<HubAddressDto>> GetAllHubLocationsAsync();
        Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync(int? hubId, int? month, int? year);
        Task<List<HubSenderOrderListDto>> GetTransporterSenderOrdersAsync(int? hubId, int? month, int? year);

        Task<bool> StoreOrderExistsAsync(int storeOrderId);
        Task<HubStoreVerificationDto?> GetVerificationAsync(int storeOrderId);
        Task<HubStoreVerificationDto> SaveVerificationAsync(int storeOrderId, SaveHubVerificationDto dto);

        Task<bool> SenderOrderExistsAsync(int senderOrderId);

        Task<SenderVerificationDto?> GetSenderVerificationAsync(int senderOrderId);
        Task<SenderVerificationDto> SaveSenderVerificationAsync(int senderOrderId, SaveHubVerificationDto dto);
        Task<HubStoreOrderDetailsDto?> GetStoreOrderDetailsAsync(int storeOrderId);

        Task<HubSenderOrderDetailsDto?> GetSenderOrderDetailsAsync(int senderOrderId);

        //summary counts
        Task<HubMonthlyCountsDto> GetStoreOrderCountsAsync(int month, int year, int? hubId);
        Task<HubMonthlyCountsDto> GetSenderOrderCountsAsync(int month, int year, int? hubId);
    }
}
