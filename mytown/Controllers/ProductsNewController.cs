using Microsoft.AspNetCore.Mvc;
using mytown.DTOs.ProductsNew;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/products-new")]
    public class ProductsNewController : ControllerBase
    {
        private readonly IProductsNewService _service;

        public ProductsNewController(
            IProductsNewService service)
        {
            _service = service;
        }


        // =========================================
        // CREATE PRODUCT
        // =========================================

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductNewRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid product data.",
                    errors = ModelState
                });
            }


            try
            {
                var productId = await _service.CreateProductAsync(request);

                return Ok(new
                {
                    success = true,

                    message = "Product and variants saved successfully.",
                    productId = productId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,

                        message = "Failed to save product.",
                        error = ex.Message
                    });
            }
        }
    }
}