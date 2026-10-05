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
    }
}
