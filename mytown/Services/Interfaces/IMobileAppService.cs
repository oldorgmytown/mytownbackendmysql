using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IMobileAppService
    {
        Task<List<PopularProductDto>> GetPopularProductsAsync();
        Task<List<PopularStoresDto>> GetPopularStoresAsync();
        Task<List<TownStoreCountDto>> GetExploreTownsAsync();
        // New Popular Cities API
        Task<List<PopularCityDto>> GetPopularCitiesAsync();
        Task<List<TownListDto>> GetTownListByCityAsync(string city);
        Task<List<AvailableTransporterDto>> GetAvailableTransportersAsync(
        string startTown,
        string startCity,
        string destinationTown,
        string destinationCity);
       Task<List<AllProductsDto>> GetAllProductsAsync();
       Task<List<AllProductsDto>> GetProductsBySubCategoryAsync(int subCategoryId);
       Task<List<StoreBySubCategoryDto>> GetStoresBySubCategoryAsync(int prodSubcatId);
       Task<List<CountryDto>> GetAllCountriesAsync();  // added
       
    }
}