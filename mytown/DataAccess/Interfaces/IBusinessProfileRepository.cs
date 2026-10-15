using mytown.Models;
using mytown.Models.DTO_s;
using static mytown.Models.busprofilepreview;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessProfileRepository
    {
        Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId);

        IEnumerable<Products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId);

        Task<BusinessProfile> AddBusinessProfileAsync(BusinessProfile businessProfile);

        Task<string> UploadToBlobAsync(IFormFile file, string imageType);

        Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath);
        Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath);

        Task<IEnumerable<ProductSubCategory>> GetAllSubCategoriesAsync();

        //get type,fabric,design on add product form
        Task<ProductDetailsDto> GetDetailsBySubCategoryAsync(int prodSubcatId);

        List<ProductSubCategory> GetProductSubCategoriesByBusRegId(int busRegId);

        List<ProductSubCategory> GetProductSubCategoriesByBusCatId(int busCatId);
        Task<IEnumerable<BusinessProfile>> GetAllBusinessProfilesAsync();
        Task<IEnumerable<BusinessProfileWithDiscountDto>> GetBusinessProfilesWithDiscountedProductsAsync();

        Task<List<string>> GetUniqueCountriesAsync();
    }
}

