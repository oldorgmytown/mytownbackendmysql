using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.Services.Interfaces
{
    public interface IHubService
    {
        Task<List<HubAddressDto>> GetAllHubLocationsAsync();
    }
}
