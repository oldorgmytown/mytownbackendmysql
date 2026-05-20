using Microsoft.AspNetCore.Mvc;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessServiceController : ControllerBase
    {
        private readonly IServicesProfile _service;
        public BusinessServiceController(IServicesProfile service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _service.GetAllServicesAsync();
            return Ok(services);
        }
    }
}