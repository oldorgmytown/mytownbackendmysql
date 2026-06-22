using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class MobileAppService : IMobileAppService
    {
        private readonly ILogger<MobileAppService> _logger;
        private readonly IMobileAppRepository _mobileAppRepository;

        public MobileAppService(
            ILogger<MobileAppService> logger,
            IMobileAppRepository mobileAppRepository)
        {
            _logger = logger;
            _mobileAppRepository = mobileAppRepository;
        }

        public async Task<List<PopularProductDto>> GetPopularProductsAsync()
        {
            return await _mobileAppRepository.GetPopularProductsAsync();
        }

        public async Task<List<PopularStoresDto>> GetPopularStoresAsync()
        {
            return await _mobileAppRepository.GetPopularStoresAsync();
        }

        public async Task<List<TownStoreCountDto>> GetExploreTownsAsync()
        {
            return await _mobileAppRepository.GetExploreTownsAsync();
        }

        // New Popular Cities Method
        public async Task<List<PopularCityDto>> GetPopularCitiesAsync()
        {
            return await _mobileAppRepository.GetPopularCitiesAsync();
        }

        public async Task<List<TownListDto>> GetTownListByCityAsync(string city)
       {
    return await _mobileAppRepository.GetTownListByCityAsync(city);
       }

        public async Task<List<AvailableTransporterDto>> GetAvailableTransportersAsync(
    string startTown,
    string startCity,
    string destinationTown,
    string destinationCity)
        {
            return await _mobileAppRepository.GetAvailableTransportersAsync(
                startTown,
                startCity,
                destinationTown,
                destinationCity);
        }
        public async Task<List<AllProductsDto>> GetAllProductsAsync()
        {
            return await _mobileAppRepository.GetAllProductsAsync();
        }

        public async Task<List<AllProductsDto>> GetProductsBySubCategoryAsync(int subCategoryId)
        {
           return await _mobileAppRepository.GetProductsBySubCategoryAsync(subCategoryId);
        }

        public async Task<List<StoreBySubCategoryDto>> GetStoresBySubCategoryAsync(int prodSubcatId)
        {
          return await _mobileAppRepository.GetStoresBySubCategoryAsync(prodSubcatId);
        }

    }
}