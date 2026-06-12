using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
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
    }
}
