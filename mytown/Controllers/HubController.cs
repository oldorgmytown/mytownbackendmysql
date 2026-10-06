using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;


namespace mytown.Controllers
{
    [Route("api/hub")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubService _hubService;
        private readonly ILogger<HubController> _logger;

        public HubController(IHubService hubService,
                             ILogger<HubController> logger)
        {
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/hub/locations
        [HttpGet("get_allhub_locations")]
        public async Task<IActionResult> GetAllHubLocations()
        {
            var hubs = await _hubService.GetAllHubLocationsAsync();
            return Ok(hubs);
        }
        [HttpGet("store-orders-onhub")]
        public async Task<IActionResult> GetStoreOrders()
        {
            var data = await _hubService.GetTransporterStoreOrdersAsync();
            return Ok(data);
        }

        // GET: api/hub/sender-orders
        [HttpGet("sender-orders-onhub")]
        public async Task<IActionResult> GetSenderOrders()
        {
            var data = await _hubService.GetTransporterSenderOrdersAsync();
            return Ok(data);
        }

        
        [HttpGet("get_store-orders_verified")]
        public async Task<IActionResult> GetVerification(int storeOrderId)
        {
            var data = await _hubService.GetVerificationAsync(storeOrderId);
            return Ok(data);   // null if the hub hasn't started verification yet
        }

        // PUT: api/hub/store-orders/490/verification
        [HttpPut("save_store-orders_verification")]
        public async Task<IActionResult> SaveVerification(
            int storeOrderId, [FromBody] SaveHubVerificationDto dto)
        {
            var result = await _hubService.SaveVerificationAsync(storeOrderId, dto);

            if (!result.Success)
                return result.Error == "Store order not found."
                    ? NotFound(new { message = result.Error })
                    : BadRequest(new { message = result.Error });

            return Ok(result.Data);
        }
    }
}
