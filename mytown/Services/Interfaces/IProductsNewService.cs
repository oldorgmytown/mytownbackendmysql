using mytown.DTOs.ProductsNew;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.Services.Interfaces
{
    public interface IProductsNewService
    {
        Task<long> CreateProductAsync(
            CreateProductNewRequest request);

        Task<string> UploadToBlobAsync(IFormFile file, string imageType);

        Task<ProductMasterNamesDto> GetProductMasterNamesByBusinessAsync(int busRegId);
        Task<List<ProductSearchResultDto>> SearchProductsAsync(
     ProductSearchRequestDto request);

        Task<ProductReview> AddProductReviewAsync(AddProductReviewDto dto);
        Task<ProductReviewSummaryDto> GetProductReviewsAsync(long productId);
    }
}
