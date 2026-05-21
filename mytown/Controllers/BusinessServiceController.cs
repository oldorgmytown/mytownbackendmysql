using Microsoft.AspNetCore.Mvc;
using mytown.DTOs;
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

        [HttpGet("GetBusinessServices")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _service.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("Service_Subcategories/{busServId}")]
        public async Task<IActionResult> GetByBusServId(int busServId)
        {
            var result = await _service.GetByBusServIdAsync(busServId);

            if (result == null || !result.Any())
            {
                return NotFound(new
                {
                    message = "No service subcategories found"
                });
            }

            return Ok(result);
        }

        [HttpPost("add-service-profile")]
        public async Task<IActionResult> AddServiceProfile(CreateServiceProfileDto dto)
        {
            var result = await _service.AddServiceProfileAsync(dto);

            if (result)
            {
                return Ok("Service profile added successfully");
            }

            return BadRequest();
        }

        [HttpGet("GetBusinessServiceDetails/{busRegId}")]
        public async Task<IActionResult> GetByBusRegId(int busRegId)
        {
            var result = await _service.GetByBusRegIdAsync(busRegId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Business not found"
                });
            }

            return Ok(result);
        }

        [HttpGet("service-profile-details/{busRegId}")]
        public async Task<IActionResult> GetServiceProfileDetails(int busRegId)
        {
            var result = await _service.GetServiceProfileDetailsAsync(busRegId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Data not found"
                });
            }

            return Ok(result);
        }
    }
}