using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.Services.Interfaces
{
    public interface IHubService
    {
        Task<List<HubAddressDto>> GetAllHubLocationsAsync();
        Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync(int? month, int? year);
        Task<List<HubSenderOrderListDto>> GetTransporterSenderOrdersAsync(int? month, int? year);

        Task<HubStoreVerificationDto?> GetVerificationAsync(int storeOrderId);
        Task<(bool Success, string? Error, HubStoreVerificationDto? Data)> SaveVerificationAsync(
            int storeOrderId, SaveHubVerificationDto dto);

        Task<SenderVerificationDto?> GetSenderVerificationAsync(int senderOrderId);
        Task<(bool Success, string? Error, SenderVerificationDto? Data)> SaveSenderVerificationAsync(
            int senderOrderId, SaveHubVerificationDto dto);

        Task<HubStoreOrderDetailsDto?> GetStoreOrderDetailsAsync(int storeOrderId);

        Task<HubSenderOrderDetailsDto?> GetSenderOrderDetailsAsync(int senderOrderId);
    }
}
