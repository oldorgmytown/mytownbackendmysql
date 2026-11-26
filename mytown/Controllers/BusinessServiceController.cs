using Microsoft.AspNetCore.Mvc;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Implementations;
using mytown.Services.Interfaces;
using System.Globalization;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/businessservices")]
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

        [HttpGet("getbusinessservicetypes_by_busregid")]
        public async Task<IActionResult> GetBusinessServiceTypes(
    [FromQuery] int busRegId)
        {
            if (busRegId <= 0)
                return BadRequest("Invalid BusRegId.");

            var result = await _service.GetBusinessServiceTypesAsync(busRegId);

            return Ok(result);
        }

        // get all services
        [HttpGet("getallservicesbybusregid")]
        public async Task<IActionResult> GetServicesByBusRegId([FromQuery] int busRegId)
        {
            if (busRegId <= 0)
                return BadRequest("Invalid BusRegId.");

            var services = await _service.GetServicesByBusRegIdAsync(busRegId);

            return Ok(services);
        }

        //edit service types on dashboard
        [HttpPut("editservicetypes")]
        public async Task<IActionResult> UpdateService(
    [FromBody] UpdateServiceDto dto)
        {
            var updated = await _service.UpdateServiceAsync(dto);

            if (!updated)
                return NotFound("Service not found.");

            return Ok("Service updated successfully.");
        }


        // ---------- Service enquiry ----------

        // Load the enquiry form: header, services, locations, working hours, date range
        // GET api/businessservices/enquiry-form-data/12
        [HttpGet("enquiry-form-data/{serviceProfileId:int}")]
        public async Task<IActionResult> GetEnquiryFormData(int serviceProfileId)
        {
            var result = await _service.GetEnquiryFormDataAsync(serviceProfileId);

            if (result == null)
                return NotFound(new { message = "Service profile not found." });

            return Ok(result);
        }

        // Calendar: only dates that still have a free slot
        // GET api/businessservices/enquiry-available-dates/12?year=2026&month=11
        [HttpGet("enquiry-available-dates/{serviceProfileId:int}")]
        public async Task<IActionResult> GetEnquiryAvailableDates(
            int serviceProfileId, [FromQuery] int year, [FromQuery] int month)
        {
            if (year < 2000 || year > 2100 || month < 1 || month > 12)
                return BadRequest(new { message = "Invalid year or month." });

            var result = await _service.GetEnquiryAvailableDatesAsync(serviceProfileId, year, month);

            if (result == null)
                return NotFound(new { message = "Service profile not found." });

            return Ok(result);
        }

        // Time slots (free + unblocked) for one date
        // GET api/businessservices/enquiry-available-slots/12?date=2026-11-24
        [HttpGet("enquiry-available-slots/{serviceProfileId:int}")]
        public async Task<IActionResult> GetEnquiryAvailableSlots(
            int serviceProfileId, [FromQuery] DateTime date)
        {
            var result = await _service.GetEnquiryAvailableSlotsAsync(serviceProfileId, date);

            if (result == null)
                return NotFound(new { message = "Service profile not found." });

            return Ok(result);
        }

        // Save the enquiry
        // POST api/businessservices/add-service-enquiry
        [HttpPost("add-service-enquiry")]
        public async Task<IActionResult> AddServiceEnquiry([FromBody] CreateServiceEnquiryDto dto)
        {
            var result = await _service.AddServiceEnquiryAsync(dto);

            if (result.Success)
                return Ok(new { message = result.Message, enquiryId = result.EnquiryId });

            return result.Error switch
            {
                EnquiryError.NotFound => NotFound(new { message = result.Message }),
                EnquiryError.Conflict => Conflict(new { message = result.Message }),
                _ => BadRequest(new { message = result.Message })
            };
        }
    }
}