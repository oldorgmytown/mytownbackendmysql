using Microsoft.AspNetCore.Mvc;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessServiceController : ControllerBase
    {
        private readonly IBusinessServiceService _service;
        public BusinessServiceController(IBusinessServiceService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _service.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("subcategories/{busRegId}")]
        public async Task<IActionResult> GetSubCategories(int busRegId)
        {
            var result = await _service.GetSubCategoriesByBusRegIdAsync(busRegId);
            if (result == null)
                return NotFound(new { message = "Business profile not found" });

            return Ok(result);
        }
    }
}