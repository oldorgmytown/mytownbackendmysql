using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IHubRepository
    {

        Task<List<HubAddressDto>> GetAllHubLocationsAsync();
        Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync();
    }
}
