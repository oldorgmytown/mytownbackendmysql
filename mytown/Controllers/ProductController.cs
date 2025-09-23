using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
using Azure.Storage.Blobs;
using mytown.DataAccess.Repositories;
using System;


namespace MyTown.Controllers
{
    [Route("api/business/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<ProductController> _logger;
        private readonly IConfiguration _configuration;

        public ProductController(IProductRepository productRepo, IConfiguration configuration,
                                 ILogger<ProductController> logger)
        {
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration;
        }


        [HttpPost("Add_Product")]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto dto)
        {
            var entity = new products
            {
                BusRegId = dto.BusRegId,
                BuscatId = dto.BuscatId,
                prod_subcat_id = dto.ProdSubcatId,
                product_name = dto.ProductName,               
                product_description = dto.ProductDescription,
                ProductTypeId = dto.ProductTypeId,
                FabricId = dto.FabricId,
                DesignId = dto.DesignId,
                supplier_name = dto.SupplierName
            };

            var result = await _productRepo.AddProductAsync(entity);

            return Ok(new { productId = result.product_id, message = "Product added successfully" });
        }


        [HttpPost("Add_SKU_ProductVariant")]
        public async Task<IActionResult> AddProductVariant([FromForm] Sku_ProductVariantDto dto, [FromForm] List<IFormFile> files)
        {
            {
                if (dto == null)
                    return BadRequest("Variant data is required.");

                var variant = await _productRepo.AddProductVariantAsync(dto, files);

                return Ok(new
                {
                    message = "Variant added successfully",
                    skuId = variant.SkuId
                });
            }
        }

        [HttpGet("GetProductandVariantDetails/{productId}")]
        public async Task<ActionResult<ProdVariantdetailsDto>> GetProductandVariantDetails(int productId)
        {
            var result = await _productRepo.GetProductandVariantAsync(productId);
            if (result == null)
                return NotFound(new { message = "Product not found." });

            return Ok(result);
        }

        [HttpPut("UpdateProductVariants")]
        public async Task<IActionResult> UpdateVariant(
    [FromForm] Sku_ProductVariantDto dto,
    [FromForm] List<IFormFile>? images)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productRepo.UpdateVariantAsync(dto, images ?? new());

            if (result == null)
                return NotFound(new { Message = "Variant not found." });

            return Ok(new
            {
                result.SkuId,
                result.Color,
                result.Size,
                result.Sku_Cost,
                result.Quantity,
                Images = result.Images.Select(i => new
                {
                    i.ImageId,
                    i.FileName,
                    i.SortOrder
                })
            });
        }


        //[HttpPost("Save_Product")]
        //public async Task<IActionResult> SaveProduct([FromForm] ProductDto request, [FromForm] List<IFormFile> files)
        //{
        //    if (request == null)
        //        return BadRequest("Invalid product data.");

        //    var product = new products
        //    {
        //        product_id = request.ProductId,
        //        BusRegId = request.BusRegId,
        //        BuscatId = request.BuscatId,
        //        prod_subcat_id = request.ProdSubcatId,
        //        product_name = request.ProductName,
        //        product_subject = request.ProductSubject,
        //        product_description = request.ProductDescription,
        //        product_cost = request.ProductAmount,
        //        product_length = request.ProductLength,
        //        product_width = request.ProductWidth,
        //        product_weight = request.ProductWeight,
        //        product_quantity = request.Quantity,
        //        product_height = request.ProductHeight,
        //        discount = request.Discount,
        //        discount_price = request.DiscountPrice,
        //        color = request.Color,
        //        size = request.Size,

        //          product_image = "" // for tetsing purpose
        //    };

        //    products savedProduct;

        //    if (request.ProductId == 0)
        //    {
        //        savedProduct = await _productRepo.CreateProductAsync(product, files);
        //    }
        //    else
        //    {
        //        savedProduct = await _productRepo.UpdateProductAsync(product, files);
        //        if (savedProduct == null)
        //            return NotFound(new { message = "Product not found." });
        //    }

        //    return Ok(new
        //    {
        //        message = request.ProductId == 0 ? "Product created successfully" : "Product updated successfully",
        //        productId = savedProduct.product_id
        //    });
        //}



        [HttpDelete("deleteProduct")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            try
            {
                
                await _productRepo.DeleteProductAsync(productId);

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error deleting product: {ex.Message}");                
                return StatusCode(500, new { message = "An error occurred while deleting the product." });
            }
        }
        [HttpDelete("delete_ProductVariant")]
        public async Task<IActionResult> DeleteVariant(int productId, int sku_VariantId)
        {
            try
            {
                await _productRepo.DeleteProductVariantAsync(productId, sku_VariantId);
                return Ok(new { message = "Variant deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting variant: {ex.Message}");
            }
        }

        //[HttpPut("updateProduct")]
        //public IActionResult UpdateProduct([FromBody] products updatedProduct)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new { message = "Invalid product data" });
        //    }

        //    var isUpdated = _productRepo.UpdateProductAsync(updatedProduct);
        //    //if (!isUpdated)
        //    //{
        //    //    return NotFound(new { message = "Product not found" });
        //    //}

        //    return Ok(new { message = "Product updated successfully" });
        //}

        // GET: api/products/{id}
        [HttpGet("GetProductById/{productId}")]
        public async Task<ActionResult<products>> GetProductById(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: api/User/GetAllProducts
        [HttpGet("GetAllProductsforbusid/{BusRegId}")]
        public async Task<ActionResult<ProdVariantdetailsDto>> GetAllProducts(int BusRegId)
        {
            try
            {
                // Fetch all products from the repository
                var products = await _productRepo.GetAllProductsAsync(BusRegId);

                // Check if no products were found
                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                // Return the list of products with a 200 OK status
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Handle any errors and return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetDiscountedProductsAsync")]
        public async Task<ActionResult<products>> GetDiscountedProductsAsync()
        {
            try
            {
                // Fetch all products from the repository
                var products = await _productRepo.GetDiscountedProductsAsync();

                // Check if no products were found
                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                // Return the list of products with a 200 OK status
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Handle any errors and return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetProductsBySubCategory/{subCategoryId}")]
        public async Task<IActionResult> GetProductsBySubCategory(int subCategoryId)
        {
            var products = await _productRepo.GetProductsBySubCategoryAsync(subCategoryId);
            if (products == null || !products.Any())
            {
                return NotFound(new { Message = "No products found for this subcategory" });
            }

            return Ok(products);
        }

        //save shopper recently viewd product
        [HttpPost("ShopperRecentViewProduct")]
        public async Task<IActionResult> ShopperRecentViewProduct(int shopperId, int productId)
        {
            await _productRepo.SaveProductViewAsync(shopperId, productId);
            return Ok(new { message = "Product view recorded" });
        }

        [HttpGet("TopPurchasedProductsByTown")]

        public IActionResult GetTopPurchasedProductsByLocation([FromQuery] string location, [FromQuery] int minOrders = 5)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Location cannot be empty.");
            }

            var products = _productRepo.GetTopPurchasedProductsByLocation(location, minOrders);

            if (products == null || !products.Any())
            {
                return NotFound(new { Message = "No products found for the given location and criteria." });
            }

            return Ok(products);
        }

    }
}