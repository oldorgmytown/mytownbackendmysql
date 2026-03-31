using mytown.Models;
using mytown.Models.DTO_s;
using System.Threading.Tasks;

namespace mytown.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        //latest - add only prod main details----------------------------------------------//
        Task<Products> AddProductAsync(Products product);
        Task<Sku_ProductVariant> AddProductVariantAsync(Sku_CreateVariantDto dto);

        Task<ProdVariantdetailsDto?> GetProductandVariantAsync(int productId);

        //update product main details
        Task<Products> UpdateProductAsync(int productId, ProductCreateDto dto);

        //Update productvariant
        Task<Sku_ProductVariant?> UpdateVariantAsync(Sku_ProductVariantDto dto, List<IFormFile> imageFiles);

        Task<ProductSizeMeasurementDto?> GetMeasurementBySizeIdAsync(int sizeId);
        //---------------------------------------------------------------------------------//


        //Task<products> CreateProductAsync(products product, List<IFormFile> imageFiles);
        //Task<products> UpdateProductAsync(products updatedProduct, List<IFormFile> imageFiles);

        Task<string> UploadToBlobAsync(IFormFile file, string imageType);
        Task DeleteFromBlobAsync(string fileName);
       
        //Delete product and related variants
        Task DeleteProductAsync(int productId);
        //Delete variant and images
        Task DeleteProductVariantAsync(int productId, int skuId);
       
      //  Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProdVariantdetailsDto>> GetAllProductsAsync(int busRegId);
        Task<IEnumerable<ProdcVariantforShopperDto>> GetDiscountedProductsAsync();

        Task<IEnumerable<ProdcVariantforShopperDto>> GetProductsBySubCategoryAsync(int subCategoryId);

        Task SaveProductViewAsync(int shopperId, int productId);

        //get top purchased prodcuts in that location
        Task<IEnumerable<ProdcVariantforShopperDto>> GetTopPurchasedProductsByLocation(string location, int minOrders = 5);
    }
}
