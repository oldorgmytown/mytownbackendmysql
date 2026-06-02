using Microsoft.AspNetCore.Mvc;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceSubCategoryController : ControllerBase
    {
        private readonly IServiceSubCategoryService _service;

        public ServiceSubCategoryController(IServiceSubCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}