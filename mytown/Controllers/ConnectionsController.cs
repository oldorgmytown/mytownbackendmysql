using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;


namespace mytown.Controllers
{

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

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadReviewImage(
  [FromForm] UploadVariantImageRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { success = false, message = "No file provided." });

            try
            {
                var fileName = await _service.UploadToBlobAsync(request.File, "ReviewImage");

                return Ok(new
                {
                    success = true,
                    fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "Failed to upload image.",
                        error = ex.Message
                    });
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

        [HttpPost("connect-business")]
        public async Task<IActionResult> ConnectBusiness(
    [FromBody] BusinessConnectionDto request)
        {
            var connection = new BusinessConnection
            {
                BusRegId = request.BusRegId,
                ShopperRegId = request.ShopperRegId
            };

            var result = await _service.ConnectBusinessAsync(connection);

            if (!result)
            {
                return Ok(new
                {
                    message = "Shopper is already connected to this business."
                });
            }

            return Ok(new
            {
                message = "Connected to business successfully."
            });
        }

        [HttpGet("Shopper-connection-status-to-business")]
        public async Task<IActionResult> GetBusinessConnectionStatus(int busRegId, int shopperRegId)
        {
            return Ok(new
            {
                isConnected = await _service.IsBusinessConnectedAsync(busRegId, shopperRegId)
            });
        }

        [HttpGet("business-connected-shoppers/{busRegId}")]
        public async Task<IActionResult> GetConnectedShoppers(int busRegId)
        {
            return Ok(await _service.GetConnectedShoppersAsync(busRegId));
        }
    }
}
