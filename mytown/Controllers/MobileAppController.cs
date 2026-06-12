using Microsoft.AspNetCore.Mvc;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{

    [Route("api/mobileapp")]
    [ApiController]
    public class MobileAppController :ControllerBase
    {
        private readonly ILogger<MobileAppController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMobileAppService _mobileAppService;

        public MobileAppController(ILogger<MobileAppController> logger, IConfiguration configuration, IMobileAppService mobileAppService)
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
    }
}
