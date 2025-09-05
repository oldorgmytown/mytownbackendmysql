using mytown.Models;
using mytown.Models.DTO_s;
using static mytown.Models.busprofilepreview;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessProfileRepository
    {
        Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId);

        IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId);

        Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile);

        Task<string> UploadToBlobAsync(IFormFile file, string imageType);

        Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath);
        Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath);

        Task<IEnumerable<product_sub_categories>> GetAllSubCategoriesAsync();

        List<product_sub_categories> GetProductSubCategoriesByBusRegId(int busRegId);
        Task<IEnumerable<businessprofile>> GetAllBusinessProfilesAsync();
        Task<IEnumerable<BusinessProfileWithDiscountDto>> GetBusinessProfilesWithDiscountedProductsAsync();
    }
}

