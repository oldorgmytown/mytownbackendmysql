using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IMobileAppService
    {
        Task<List<PopularProductDto>> GetPopularProductsAsync();
        Task<List<PopularStoresDto>> GetPopularStoresAsync();

        Task<List<TownStoreCountDto>> GetExploreTownsAsync();
        Task<List<AvailableTransporterDto>> GetAvailableTransportersAsync(
    string startTown,
    string startCity,
    string destinationTown,
    string destinationCity);
    }
}
