using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
using mytown.Models;
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
        private readonly mytown.Models.mytown.DataAccess.AppDbContext _context;

        public ConnectionsController(IConnectionsService service,
                             ILogger<ConnectionsController> logger,
                             mytown.Models.mytown.DataAccess.AppDbContext context)
        {
            _service = service;
            _logger = logger;
            _context = context;
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
        public async Task<IActionResult> GetExperiencesByBusiness(int busRegId, [FromQuery] int shopperRegId)
        {
            try
            {
                var result = await _service.GetExperiencesByBusinessAsync(busRegId, shopperRegId);
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

        //pushtoqa

        [HttpGet("total-profile-views/{busRegId}")]
        public async Task<IActionResult> GetTotalProfileViewCount(int busRegId)
        {
            try
            {
                var total = await _service.GetTotalProfileViewCountAsync(busRegId);
                return Ok(new { BusRegId = busRegId, TotalViews = total });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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

        // GET: api/BusinessConnection/unique-shopper-count/5
        [HttpGet("shopper-connected-count/{busRegId}")]
        public async Task<IActionResult> GetUniqueShopperCount(int busRegId)
        {
            try
            {
                var count = await _service.GetUniqueShopperconnectedCountAsync(busRegId);
                return Ok(new { BusRegId = busRegId, UniqueShopperCount = count });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // likes and comments
        [HttpPost("toggle-like")]
        public async Task<IActionResult> ToggleExperienceLike(
    [FromBody] ShopperExperienceLikeDto dto)
        {
            try
            {
                var isLiked =
                    await _service.ToggleExperienceLikeAsync(dto);

                return Ok(new
                {
                    shopperExperienceId = dto.ShopperExperienceId,
                    isLiked = isLiked
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error toggling experience like");

                return StatusCode(
                    500,
                    "An error occurred while updating like.");
            }
        }

        [HttpPost("add-comment")]
        public async Task<IActionResult> AddExperienceComment(
    [FromBody] CreateShopperExperienceCommentDto dto)
        {
            try
            {
                var result =
                    await _service.AddExperienceCommentAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error adding experience comment");

                return StatusCode(
                    500,
                    "An error occurred while adding comment.");
            }
        }

                [HttpGet("chat-history")]
        public async Task<IActionResult> GetChatHistory(
            int userAId, UserType userAType,
            int userBId, UserType userBType)
        {
            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderUserId == userAId && m.SenderType == userAType &&
                     m.ReceiverUserId == userBId && m.ReceiverType == userBType)
                    ||
                    (m.SenderUserId == userBId && m.SenderType == userBType &&
                     m.ReceiverUserId == userAId && m.ReceiverType == userAType))
                .OrderBy(m => m.SentTime)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("getcomments")]
        public async Task<IActionResult> GetExperienceComments(
    int shopperExperienceId)
        {
            try
            {
                var result =
                    await _service.GetExperienceCommentsAsync(
                        shopperExperienceId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting experience comments");

                return StatusCode(
                    500,
                    "An error occurred while getting comments.");
            }
        }
    }
}
