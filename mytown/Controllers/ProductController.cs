using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace MyTown.Controllers
{
   // [Authorize]
    [Route("api/business/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // ---------------- Add Product ----------------

        [HttpPost("Add_Product")]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto dto)
        {
            var product = await _service.AddProductAsync(dto);
            return Ok(new { productId = product.ProductId, message = "Product added successfully" });
        }

        // ---------------- Add Variant ----------------

        [HttpPost("Add_SKU_ProductVariant")]
        public async Task<IActionResult> AddProductVariant([FromForm] Sku_CreateVariantDto dto)
        {
            var variant = await _service.AddProductVariantAsync(dto);
            return Ok(variant);
        }

        // ---------------- Fetch ----------------

        [HttpGet("GetSizeMeasurements")]
        public async Task<IActionResult> GetMeasurementBySizeId(int sizeId)
        {
            var result = await _service.GetMeasurementBySizeIdAsync(sizeId);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("GetProductandVariantDetails/{productId}")]
        public async Task<IActionResult> GetProductandVariantDetails(int productId)
        {
            var result = await _service.GetProductAndVariantAsync(productId);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("GetAllProductsforbusid/{busRegId}")]
        public async Task<IActionResult> GetAllProducts(int busRegId)
        {
            var products = await _service.GetAllProductsAsync(busRegId);
            return Ok(products);
        }

        // ---------------- Update ----------------

        [HttpPut("Update_Productdetails")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductCreateDto dto)
        {
            var result = await _service.UpdateProductAsync(dto);
            return result == null ? NotFound() : Ok(new { message = "Product updated successfully" });
        }

        [HttpPut("UpdateProductVariants")]
        public async Task<IActionResult> UpdateVariant(
            [FromForm] Sku_ProductVariantDto dto,
            [FromForm] List<IFormFile>? images)
        {
            var result = await _service.UpdateVariantAsync(dto, images ?? new());
            return result == null ? NotFound() : Ok(result);
        }

        // ---------------- Delete ----------------

        [HttpDelete("deleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _service.DeleteProductAsync(productId);
            return Ok(new { message = "Product deleted successfully" });
        }

        [HttpDelete("delete_ProductVariant")]
        public async Task<IActionResult> DeleteVariant(int productId, int sku_VariantId)
        {
            await _service.DeleteVariantAsync(productId, sku_VariantId);
            return Ok(new { message = "Variant deleted successfully" });
        }

        // ---------------- Shopper APIs ----------------

        [HttpGet("GetDiscountedProductsAsync")]
        public async Task<IActionResult> GetDiscountedProductsAsync()
        {
            return Ok(await _service.GetDiscountedProductsAsync());
        }

        [HttpGet("GetProductsBySubCategory/{subCategoryId}")]
        public async Task<IActionResult> GetProductsBySubCategory(int subCategoryId)
        {
            return Ok(await _service.GetProductsBySubCategoryAsync(subCategoryId));
        }

        [HttpPost("ShopperRecentViewProduct")]
        public async Task<IActionResult> ShopperRecentViewProduct(int shopperId, int productId)
        {
            await _service.SaveProductViewAsync(shopperId, productId);
            return Ok(new { message = "Product view recorded" });
        }

        [HttpGet("TopPurchasedProductsByTown")]
        public async Task<IActionResult> GetTopPurchasedProductsByLocation(
            string location,
            int minOrders = 5)
        {
            return Ok(await _service.GetTopPurchasedProductsByLocation(location, minOrders));
        }
    }
}
