using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Diagnostics;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace mytown.Controllers
{
    [Authorize]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;
      //  private readonly string _connectionString;

        public AdminController(IAdminService adminService,
                               ILogger<AdminController> logger)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           // _connectionString = config.GetConnectionString("DefaultConnection");
        }

        [Authorize]
        [HttpGet("getBusinessRegistersPaginated")]
        public async Task<IActionResult> GetBusinessRegistersPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 2, string? search = null)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "Invalid pagination parameters." });
            }

            var (records, totalRecords) = await _adminService.GetBusinessRegistersPaginatedAsync(page, pageSize, search);

            return Ok(new
            {
                data = records,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

        [Authorize]
        [HttpGet("getBusinessesstoresByStatusPaginated")]
        public async Task<IActionResult> GetBusinessesstoresByStatusPaginated(
            [FromQuery] string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { message = "Invalid pagination parameters." });

            var (records, totalRecords) = await _adminService.GetBusinessesstoresByStatusPaginatedAsync(status, page, pageSize, search);

            return Ok(new
            {
                data = records,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

        [Authorize]
        [HttpGet("GetBusinessesservicesByStatusPaginated")]
        public async Task<IActionResult> GetBusinessesservicesByStatusPaginated(
         [FromQuery] string status,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { message = "Invalid pagination parameters." });

            var (records, totalRecords) = await _adminService.GetBusinessesservicesByStatusPaginatedAsync(status, page, pageSize);

            return Ok(new
            {
                data = records,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

        [Authorize]
        [HttpGet("Businessprofilestatuscounts")]
        public async Task<IActionResult> Businessprofilestatuscounts()
        {
            try
            {
                var result = await _adminService.BusinessprofilestatuscountsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("updateprofilestatusbyadmin")]
        public async Task<IActionResult> UpdateProfileStatusByAdmin(
            [FromQuery] int busRegId,
            [FromQuery] string status,
            [FromBody] AdminProfileUpdateDto commentDto)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required.");

            if (busRegId <= 0)
                return BadRequest("Invalid business registration ID.");

            var updated = await _adminService.UpdateProfileStatusbyAdminAsync(busRegId, status, commentDto.Comment);

            if (!updated)
                return NotFound($"No business profile found with BusRegId {busRegId}.");

            return Ok("Profile status updated successfully.");
        }


        [HttpGet("GetDashboardCounts")]
        public async Task<IActionResult> GetDashboardCounts()
        {
            var counts = await _adminService.GetDashboardCountsAsync();
            return Ok(counts);
        }

        [HttpGet("GetUniqueCounts")]
        public async Task<IActionResult> GetUniqueCounts()
        {
            try
            {
                var (uniqueTowns, uniqueCities, uniqueStates, uniqueCountries) = await _adminService.GetUniqueCountsAsync();

                return Ok(new
                {
                    message = "Unique counts retrieved successfully",
                    data = new
                    {
                        uniqueTowns,
                        uniqueCities,
                        uniqueStates,
                        uniqueCountries
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving unique counts", error = ex.Message });
            }
        }

        [HttpGet("getBusinessRegisterCount")]
        public async Task<IActionResult> GetBusinessRegisterCount()
        {
            var count = await _adminService.GetBusinessRegisterCountAsync();
            return Ok(new { count });
        }

        [HttpGet("getShoppersRegisterCount")]
        public async Task<IActionResult> GetShoppersRegisterCount()
        {
            var count = await _adminService.GetShoppersRegisterCountAsync();
            return Ok(new { count });
        }

        [HttpGet("getCourierRegisterCount")]
        public async Task<IActionResult> GetCourierRegisterCount()
        {
            var count = await _adminService.GetCourierRegisterCountAsync();
            return Ok(new { count });
        }

        //[Authorize]
        //[HttpGet("getShopperRegistersPaginated")]
        //public async Task<IActionResult> GetShopperRegistersPaginated(int page = 1, int pageSize = 10)
        //{
        //    if (page <= 0 || pageSize <= 0)
        //        return BadRequest(new { message = "Page and page size must be greater than 0." });

        //    var (shopperRegisters, totalRecords) = await _adminService.GetShopperRegistersPaginatedAsync(page, pageSize);

        //    if (shopperRegisters == null || !shopperRegisters.Any())
        //        return Ok(new { data = new List<object>(), message = "No shopper registers found.", totalRecords = 0 });

        //    return Ok(new
        //    {
        //        data = shopperRegisters,
        //        totalRecords,
        //        currentPage = page,
        //        pageSize
        //    });
        //}

        [HttpGet("shoppersonAdminpanel")]
        public async Task<IActionResult> GetShoppers(
    string status = "active",
    int page = 1,
    int pageSize = 10)
        {
            var (records, totalCount) =
                await _adminService.GetShoppersByStatusAsync(status, page, pageSize);

            return Ok(new
            {
                data = records,
                totalCount
            });
        }

        //Shopper summary on admin panel

        [HttpGet("shopper_statsonAdminPanel")]
        public async Task<IActionResult> GetShopperStats()
        {
            var stats = await _adminService.GetActiveShopperStatsAsync();
            return Ok(stats);
        }


        [Authorize]
        [HttpPost("updateshopperstatusbyadmin")]
        public async Task<IActionResult> UpdateShopperStatusByAdmin([FromQuery] int shopperId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required.");
            if (shopperId <= 0)
                return BadRequest("Invalid Shopper registration ID.");

            var updated = await _adminService.UpdateShopperStatusByAdminAsync(shopperId, status);

            if (!updated)
                return NotFound($"No Shopper found with Id {shopperId}.");

            return Ok("Shopper status updated successfully.");
        }

        [HttpPut("deactivateShopper")]
        public async Task<IActionResult> DeactivateShopper(int shopperRegId)
        {
            var result = await _adminService.DeactivateShopperAsync(shopperRegId);

            if (!result)
                return NotFound("Shopper not found");

            return Ok(new { message = "Shopper account deactivated successfully" });
        }

        [Authorize]
        [HttpGet("getCourierRegistersPaginated")]
        public async Task<IActionResult> GetCourierRegistersPaginated(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { message = "Page and page size must be greater than 0." });

            var (courierRegisters, totalRecords) = await _adminService.GetCourierRegistersPaginatedAsync(page, pageSize);

            if (courierRegisters == null || !courierRegisters.Any())
                return Ok(new { data = new List<object>(), message = "No courier registers found.", totalRecords = 0 });

            return Ok(new
            {
                data = courierRegisters,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

        [HttpGet("business/completed-stores-in-locations")]
        public async Task<IActionResult> GetLocationsWithCompletedStores()
        {
            var data = await _adminService.GetLocationsWithCompletedStoresAsync();
            return Ok(data);
        }


        [HttpGet("locations/dapper")]
        public async Task<IActionResult> GetLocations_Dapper()
        {
            var sw = Stopwatch.StartNew();

            var data = await _adminService.GetLocationsWithCompletedStores_DapperAsync();

            sw.Stop();

            return Ok(new
            {
                executionTimeMs = sw.ElapsedMilliseconds,
                data = data
            });
        }

        [HttpGet("locations/ef")]
        public async Task<IActionResult> GetLocations_EF()
        {
            var sw = Stopwatch.StartNew();

            var data = await _adminService.GetLocationsWithCompletedStores_EFAsync();

            sw.Stop();

            return Ok(new
            {
                executionTimeMs = sw.ElapsedMilliseconds,
                data = data
            });
        }
        [HttpGet("db-test")]
        public async Task<IActionResult> DbTest()
        {
            try
            {
                await _adminService.TestConnectionAsync();
                return Ok("DB Connected");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message}");
            }
        }


    }
}
