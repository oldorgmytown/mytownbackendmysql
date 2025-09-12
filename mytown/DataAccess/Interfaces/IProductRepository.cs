using mytown.Models;
using mytown.Models.DTO_s;
using System.Threading.Tasks;

namespace mytown.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<products> CreateProductAsync(products product, List<IFormFile> imageFiles);



        Task<products> UpdateProductAsync(products updatedProduct, List<IFormFile> imageFiles);

        Task<string> UploadToBlobAsync(IFormFile file, string imageType);
        Task DeleteFromBlobAsync(string fileName);
        //    Task<products> GetProductByIdAsync(int productId);
        //  Task<products> UpdateProductAsync(products product);

        Task DeleteProductAsync(int productId);
        // bool UpdateProduct(products product);
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(int busRegId);
        Task<IEnumerable<ProductDto>> GetDiscountedProductsAsync();

        Task<IEnumerable<ProductDto>> GetProductsBySubCategoryAsync(int subCategoryId);

        Task SaveProductViewAsync(int shopperId, int productId);

        //get top purchased prodcuts in that location
        List<ProductDto> GetTopPurchasedProductsByLocation(string location, int minOrders = 5);
    }
}
