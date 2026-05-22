using Microsoft.AspNetCore.Mvc;
using mytown.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

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

        // GET all business services
        [HttpGet("all")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _service.GetAllServicesAsync();
            return Ok(services);
        }

        // GET sub categories by busServId
        [HttpGet("subcategories/{busServId}")]
        public async Task<IActionResult> GetSubCategories(int busServId)
        {
            var result = await _service.GetSubCategoriesByBusServIdAsync(busServId);

            if (result == null || !result.Any())
                return NotFound(new { message = "No sub categories found" });

            return Ok(result);
        }
    }
}