using Microsoft.AspNetCore.Mvc;
using mytown.DTOs.ProductsNew;
using mytown.Models.DTO_s;
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
                        error = ex.Message,
                        inner = ex.InnerException?.Message,
                        innerInner = ex.InnerException?.InnerException?.Message
                    });
            }
        }

        // =========================================
        // UPLOAD VARIANT IMAGE
        // =========================================

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadVariantImage(
     [FromForm] UploadVariantImageRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { success = false, message = "No file provided." });

            try
            {
                var fileName = await _service.UploadToBlobAsync(request.File, "product");

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

        [HttpGet("product-filter-names/{busRegId}")]
        public async Task<IActionResult> GetProductMasterNames(int busRegId)
        {
            try
            {
                var result = await _service
                    .GetProductMasterNamesByBusinessAsync(busRegId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "Failed to get product master names.",
                        error = ex.Message
                    });
            }
        }
    }
}