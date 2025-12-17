using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IProductService
    {
        // Product main
        Task<Products> AddProductAsync(ProductCreateDto dto);
        Task<Products?> UpdateProductAsync(ProductCreateDto dto);
        Task DeleteProductAsync(int productId);

        // Variants
        Task<Sku_ProductVariant> AddProductVariantAsync(Sku_CreateVariantDto dto);
        Task<Sku_ProductVariant?> UpdateVariantAsync(Sku_ProductVariantDto dto, List<IFormFile> images);
        Task DeleteVariantAsync(int productId, int skuId);

        // Fetch
        Task<ProdVariantdetailsDto?> GetProductAndVariantAsync(int productId);
        Task<ProductSizeMeasurementDto?> GetMeasurementBySizeIdAsync(int sizeId);
        Task<IEnumerable<ProdVariantdetailsDto>> GetAllProductsAsync(int busRegId);

        // Shopper / Public
        Task<IEnumerable<ProdcVariantforShopperDto>> GetDiscountedProductsAsync();
        Task<IEnumerable<ProdcVariantforShopperDto>> GetProductsBySubCategoryAsync(int subCategoryId);
        Task SaveProductViewAsync(int shopperId, int productId);
        Task<IEnumerable<ProdcVariantforShopperDto>> GetTopPurchasedProductsByLocation(string location, int minOrders);
    }
}
