using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;


namespace mytown.Controllers
{
    [Authorize]
    [Route("api/connections")]
    [ApiController]
    public class ConnectionsController :ControllerBase
    {
        private readonly IConnectionsService _service;
        private readonly ILogger<ConnectionsController> _logger;

        public ConnectionsController(IConnectionsService service,
                             ILogger<ConnectionsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        //------------ Shopper Experinece/ Reviews-----------------------

        [HttpPost("createexperience")]
        public async Task<IActionResult>
CreateExperience(
    [FromBody] CreateShopperExperienceDto dto)
        {
            try
            {
                var result = await _service.CreateExperienceAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating experiences");
                return StatusCode(500, "An error occurred while creating experiences.");
            }
        }

        [HttpGet("getexperiencesbybusiness/{busRegId}")]
        public async Task<IActionResult> GetExperiencesByBusiness(int busRegId)
        {
            try
            {
                var result = await _service.GetExperiencesByBusinessAsync(busRegId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching experiences");
                return StatusCode(500, "An error occurred while fetching experiences.");
            }
        }


        [HttpPost("capture-business-profile-view")]
        public async Task<IActionResult> CaptureBusinessProfileView(
    [FromBody] CaptureBusinessProfileViewDto request)
        {
            await _service.CaptureBusinessProfileViewAsync(request);

            return Ok(new
            {
                message = "View captured successfully."
            });
        }

        [HttpGet("current-business-profile-viewers")]
        public async Task<IActionResult> GetCurrentBusinessProfileViewers(int busRegId, int shopperRegId)
        {
            return Ok(await _service.GetCurrentBusinessProfileViewersAsync(busRegId, shopperRegId));
        }
    }
}
