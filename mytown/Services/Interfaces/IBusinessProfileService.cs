using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Services.Interfaces
{
    public interface IBusinessProfileService
    {
        // Matches exactly your controller
        Task<IActionResult> AddBusinessProfile(
            [FromForm] BusinessProfileCreateDto businessProfileDto,
            IFormFile? bannerFile,
            IFormFile? logoFile);

        Task<string> UploadProfileImageAsync(IFormFile file, string ImageType); //Business logo n banner upload to blob
        Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath);
        Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath);

        Task<IEnumerable<ProductSubCategory>> GetAllSubCategoriesAsync();
        Task<ProductDetailsDto> GetDetailsBySubCategoryAsync(int subcatId);
        List<ProductSubCategory> GetProductSubCategoriesByBusRegId(int busRegId);

        Task<IEnumerable<BusinessProfile>> GetAllBusinessProfilesAsync();
        Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId);

        IEnumerable<Products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId);

        Task<IEnumerable<BusinessProfileWithDiscountDto>> GetBusinessProfilesWithDiscountedProductsAsync();
        Task<List<string>> GetUniqueCountriesAsync();
    }
}
