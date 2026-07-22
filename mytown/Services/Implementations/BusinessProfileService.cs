using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mytown.Services;

using mytown.Services.Interfaces;   

namespace mytown.Services.Implementations
{
    public class BusinessProfileService : IBusinessProfileService
    {
        private readonly IBusinessProfileRepository _repo;

        public BusinessProfileService(IBusinessProfileRepository repo)
        {
            _repo = repo;
        }

        // ------------------- ADD BUSINESS PROFILE -------------------
        public async Task<IActionResult> AddBusinessProfile(
            BusinessProfileCreateDto businessProfileDto,
            IFormFile? bannerFile,
            IFormFile? logoFile)
        {
            // Upload images first
            string? bannerPath = null;
            string? logoPath = null;

            if (bannerFile != null)
                bannerPath = await _repo.UploadToBlobAsync(bannerFile, "banner");

            if (logoFile != null)
                logoPath = await _repo.UploadToBlobAsync(logoFile, "logo");

            // Map DTO → Entity
            var entity = new BusinessProfile
            {
                BusRegId = businessProfileDto.BusRegId,
                BusinessName = businessProfileDto.Businessname,
                BusinessLocation = businessProfileDto.BusinessLocation,
                BusinessAbout = businessProfileDto.BusinessAbout,
                ProfileStatus = businessProfileDto.ProfileStatus,
                BusCatId = businessProfileDto.Buscatid ?? 0,
                BannerPath = bannerPath,
                LogoPath = logoPath
            };

            var savedProfile = await _repo.AddBusinessProfileAsync(entity);

            return new OkObjectResult(new
            {
                message = savedProfile.BusinessProfileId == 0
                            ? "Business profile added successfully"
                            : "Business profile updated successfully",
                data = savedProfile
            });
        }

        // ------------------- UPDATE LOGO / BANNER -------------------

        public async Task<string> UploadProfileImageAsync(IFormFile file, string ImageType)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            // image type is decided here
           // const string imageType = "profile";

            return await _repo.UploadToBlobAsync(file, ImageType);
        }
        public Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath)
            => _repo.UpdateBannerPathAsync(busRegId, bannerPath);

        public Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath)
            => _repo.UpdateLogoPathAsync(busRegId, logoPath);

        // ------------------- SUBCATEGORIES -------------------
        public Task<IEnumerable<ProductSubCategory>> GetAllSubCategoriesAsync()
            => _repo.GetAllSubCategoriesAsync();

        public Task<ProductDetailsDto> GetDetailsBySubCategoryAsync(int subcatId)
            => _repo.GetDetailsBySubCategoryAsync(subcatId);

        public List<ProductSubCategory> GetProductSubCategoriesByBusRegId(int busRegId)
            => _repo.GetProductSubCategoriesByBusRegId(busRegId);

        public List<ProductSubCategory> GetProductSubCategoriesByBusCatId(int busCatId)
        {
            return _repo.GetProductSubCategoriesByBusCatId(busCatId);
        }
        // ------------------- BUSINESS PROFILES -------------------
        public Task<IEnumerable<BusinessProfile>> GetAllBusinessProfilesAsync()
            => _repo.GetAllBusinessProfilesAsync();

        public Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId)
            => _repo.GetBusinessProfilesByBusRegIdAsync(busRegId);

        // ------------------- PRODUCTS -------------------
        public IEnumerable<Products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
            => _repo.GetProductsByBusRegIdAndSubcatId(busRegId, prodSubcatId);

        // ------------------- DISCOUNT PROFILES -------------------
        public Task<IEnumerable<BusinessProfileWithDiscountDto>> GetBusinessProfilesWithDiscountedProductsAsync()
            => _repo.GetBusinessProfilesWithDiscountedProductsAsync();

        // ------------------- UNIQUE COUNTRIES -------------------
        public Task<List<string>> GetUniqueCountriesAsync()
            => _repo.GetUniqueCountriesAsync();
    }
}
