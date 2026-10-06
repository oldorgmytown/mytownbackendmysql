using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.Services.Interfaces
{
    public interface IHubService
    {
        Task<List<HubAddressDto>> GetAllHubLocationsAsync();
        Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync();
        Task<List<HubSenderOrderListDto>> GetTransporterSenderOrdersAsync();

        Task<HubStoreVerificationDto?> GetVerificationAsync(int storeOrderId);
        Task<(bool Success, string? Error, HubStoreVerificationDto? Data)> SaveVerificationAsync(
            int storeOrderId, SaveHubVerificationDto dto);
    }
}
