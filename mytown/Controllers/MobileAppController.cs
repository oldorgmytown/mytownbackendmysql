using Microsoft.AspNetCore.Mvc;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [Route("api/mobileapp")]
    [ApiController]
    public class MobileAppController : ControllerBase
    {
        private readonly ILogger<MobileAppController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMobileAppService _mobileAppService;

        public MobileAppController(
            ILogger<MobileAppController> logger,
            IConfiguration configuration,
            IMobileAppService mobileAppService)
        {
            _logger = logger;
            _configuration = configuration;
            _mobileAppService = mobileAppService;
        }

        [HttpGet("popularproducts")]
        public async Task<IActionResult> GetPopularProducts()
        {
            var result = await _mobileAppService.GetPopularProductsAsync();
            return Ok(result);
        }

        [HttpGet("popularstores")]
        public async Task<IActionResult> GetPopularStores()
        {
            var result = await _mobileAppService.GetPopularStoresAsync();
            return Ok(result);
        }

        [HttpGet("exploretowns")]
        public async Task<IActionResult> GetExploreTowns()
        {
            var result = await _mobileAppService.GetExploreTownsAsync();
            return Ok(result);
        }

        [HttpGet("find-transporters")]
        public async Task<IActionResult> GetAvailableTransporters(
     string startTown,
     string startCity,
     string destinationTown,
     string destinationCity)
        {
            var result = await _mobileAppService.GetAvailableTransportersAsync(
                startTown,
                startCity,
                destinationTown,
                destinationCity);

            return Ok(result);
        }

        [HttpGet("popularcities")]
        public async Task<IActionResult> GetPopularCities()
        {
            var result = await _mobileAppService.GetPopularCitiesAsync();
            return Ok(result);
        }

        [HttpGet("city/{city}/towns")]
        public async Task<IActionResult> GetTownListByCity(string city)
        {
            var result = await _mobileAppService.GetTownListByCityAsync(city);

            return Ok(result);
        }

        [HttpGet("allproducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _mobileAppService.GetAllProductsAsync();
            return Ok(result);
        }

        [HttpGet("productsbysubcategory/{subCategoryId}")]
        public async Task<IActionResult> GetProductsBySubCategory(int subCategoryId)
        {
            var result = await _mobileAppService.GetProductsBySubCategoryAsync(subCategoryId);

            return Ok(result);
        }

        [HttpGet("storesbysubcategory/{prodSubcatId}")]
        public async Task<IActionResult> GetStoresBySubCategory(int prodSubcatId)
        {
            var result =
                await _mobileAppService.GetStoresBySubCategoryAsync(prodSubcatId);

            return Ok(result);
        }


    }
}