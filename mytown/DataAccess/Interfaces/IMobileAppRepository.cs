using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IMobileAppRepository
    {
        Task<List<PopularProductDto>> GetPopularProductsAsync();
        Task<List<PopularStoresDto>> GetPopularStoresAsync();
        Task<List<TownStoreCountDto>> GetExploreTownsAsync();
        Task<List<AvailableTransporterDto>> GetAvailableTransportersAsync(
    string startTown,
    string startCity,
    string destinationTown,
    string destinationCity);
        Task<List<PopularCityDto>> GetPopularCitiesAsync();
        Task<List<TownListDto>> GetTownListByCityAsync(string city);
    }
}