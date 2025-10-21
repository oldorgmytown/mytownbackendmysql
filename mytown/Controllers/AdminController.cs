using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;

namespace mytown.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminRepository _adminRepo;

        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminRepository adminRepo,
                                 ILogger<AdminController> logger)
        {
            _adminRepo = adminRepo ?? throw new ArgumentNullException(nameof(adminRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("getBusinessRegistersPaginated")]
        public async Task<IActionResult> GetBusinessRegistersPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 2, string? search = null)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "Invalid pagination parameters." });
            }

            var (records, totalRecords) = await _adminRepo.GetBusinessRegistersPaginatedAsync(page, pageSize,search);

            return Ok(new
            {
                data = records,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

        // for stores with all profil status types
        [HttpGet("getBusinessesstoresByStatusPaginated")]
        public async Task<IActionResult> GetBusinessesstoresByStatusPaginated(
            [FromQuery] string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { message = "Invalid pagination parameters." });

            var (records, totalRecords) = await _adminRepo.GetBusinessesstoresByStatusPaginatedAsync(status, page, pageSize, search);

            return Ok(new
            {
                data = records,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

        // for services with all profile status
        [HttpGet("GetBusinessesservicesByStatusPaginated")]
        public async Task<IActionResult> GetBusinessesservicesByStatusPaginated(
         [FromQuery] string status,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 10)
            {
                if (page < 1 || pageSize < 1)
                    return BadRequest(new { message = "Invalid pagination parameters." });

                var (records, totalRecords) = await _adminRepo.GetBusinessesservicesByStatusPaginated(status, page, pageSize);

                return Ok(new
                {
                    data = records,
                    totalRecords,
                    currentPage = page,
                    pageSize
                });
            }


        //Business Summary count for all profile status
        [HttpGet("Businessprofilestatuscounts")]
        public async Task<IActionResult> Businessprofilestatuscounts()
        {
            try
            {
                var result = await _adminRepo.Businessprofilestatuscounts();
                return Ok(result); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("updateprofilestatusbyadmin")]
        public async Task<IActionResult> updateprofilestatusbyadmin([FromQuery] int busRegId, [FromQuery] string status, [FromQuery] string? comments = null)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required.");

            var updated = await _adminRepo.UpdateProfileStatusbyAdminAsync(busRegId, status, comments);

            if (!updated)
                return NotFound($"No business profile found with BusRegId {busRegId}.");

            return Ok("Profile status updated successfully.");
        }


        [HttpGet("GetDashboardCounts")]
        public async Task<IActionResult> GetDashboardCounts()
        {
            var counts = await _adminRepo.GetDashboardCountsAsync();
            return Ok(counts);
        }

        // Add API to get unique counts for cities, states, and countries
        [HttpGet("GetUniqueCounts")]
        public async Task<IActionResult> GetUniqueCounts()
        {
            try
            {
                // Get unique counts from the repository
                var (uniqueTowns,uniqueCities, uniqueStates, uniqueCountries) = await _adminRepo.GetUniqueCountsAsync();

                // Return the result
                return Ok(new
                {
                    message = "Unique counts retrieved successfully",
                    data = new
                    {
                        uniqueTowns = uniqueTowns,
                        uniqueCities = uniqueCities,
                        uniqueStates = uniqueStates,
                        uniqueCountries = uniqueCountries
                    }
                });
            }
            catch (Exception ex)
            {
                // If there's an error, return a 500 status code
                return StatusCode(500, new { message = "Error retrieving unique counts", error = ex.Message });
            }
        }

        [HttpGet("getBusinessRegisterCount")]
        public async Task<IActionResult> GetBusinessRegisterCount()
        {
            var count = await _adminRepo.GetBusinessRegisterCountAsync();
            return Ok(new { count });
        }

        [HttpGet("getShoppersRegisterCount")]
        public async Task<IActionResult> getShoppersRegisterCount()
        {
            var count = await _adminRepo.GetShoppersRegisterCountAsync();
            return Ok(new { count });
        }

        [HttpGet("getCourierRegisterCount")]
        public async Task<IActionResult> getCourierRegisterCount()
        {
            var count = await _adminRepo.GetCourierserviceCountAsync();
            return Ok(new { count });
        }

        // Shoppers Tab
        [HttpGet("getShopperRegistersPaginated")]
        public async Task<IActionResult> GetShopperRegistersPaginated(int page = 1, int pageSize = 10)
        {
            // Validate page and pageSize
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page and page size must be greater than 0." });
            }

            // Call the repository method to fetch paginated shopper registers
            var (shopperRegisters, totalRecords) = await _adminRepo.GetShopperRegistersPaginatedAsync(page, pageSize);

            // Check if there are any records
            if (shopperRegisters == null || !shopperRegisters.Any())
            {
                return Ok(new { data = new List<object>(), message = "No shopper registers found.", totalRecords = 0 });
            }

            // Return the paginated list of shoppers as JSON
            return Ok(new
            {
                data = shopperRegisters,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

        [HttpPost("updateshopperstatusbyadmin")]
        public async Task<IActionResult> updateshopperstatusbyadmin([FromQuery] int shopperId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status is required.");

            var updated = await _adminRepo.UpdateProfileStatusbyAdminAsync(shopperId, status);

            if (!updated)
                return NotFound($"No Shopper found with Id {shopperId}.");
            // Fetch shopper details (e.g., email)
            var shopper = await _adminRepo.GetShopperByIdAsync(shopperId);
            if (shopper != null && !string.IsNullOrEmpty(shopper.Email))
            {
                // Send status update email
               // await _emailService.SendStatusUpdateEmail(shopper.Email, shopper.FullName, status);
            }

            return Ok("Shopper status updated successfully.");
        }

        [HttpGet("getCourierRegistersPaginated")]
        public async Task<IActionResult> GetCourierRegistersPaginated(int page = 1, int pageSize = 10)
        {
            // Validate page and pageSize
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page and page size must be greater than 0." });
            }

            // Call the repository method to fetch paginated shopper registers
            var (courierRegisters, totalRecords) = await _adminRepo.GetCourierRegistersPaginatedAsync(page, pageSize);

            // Check if there are any records
            if (courierRegisters == null || !courierRegisters.Any())
            {
                return Ok(new { data = new List<object>(), message = "No courier registers found.", totalRecords = 0 });
            }

            // Return the paginated list of shoppers as JSON
            return Ok(new
            {
                data = courierRegisters,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }


        //landing page

        [HttpGet("business/completed-stores-in-locations")]
        public async Task<IActionResult> GetLocationsWithCompletedStores()
        {
            var data = await _adminRepo.GetLocationsWithCompletedStoresAsync();
            return Ok(data);
        }


    }
}
