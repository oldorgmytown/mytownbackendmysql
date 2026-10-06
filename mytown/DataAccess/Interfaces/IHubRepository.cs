using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IHubRepository
    {

        Task<List<HubAddressDto>> GetAllHubLocationsAsync();
        Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync();
        Task<List<HubSenderOrderListDto>> GetTransporterSenderOrdersAsync();

        Task<bool> StoreOrderExistsAsync(int storeOrderId);
        Task<HubStoreVerificationDto?> GetVerificationAsync(int storeOrderId);
        Task<HubStoreVerificationDto> SaveVerificationAsync(int storeOrderId, SaveHubVerificationDto dto);

        Task<bool> SenderOrderExistsAsync(int senderOrderId);

        Task<SenderVerificationDto?> GetSenderVerificationAsync(int senderOrderId);
        Task<SenderVerificationDto> SaveSenderVerificationAsync(int senderOrderId, SaveHubVerificationDto dto);
        Task<HubStoreOrderDetailsDto?> GetStoreOrderDetailsAsync(int storeOrderId);
    }
}
