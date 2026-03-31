using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using mytown.Services.Implementations;
using System.Threading.Tasks;



namespace mytown.Controllers
{
   // [Authorize]
    [Route("api/business/profile")]
    [ApiController]
  public class BusinessProfileController : ControllerBase
{
    private readonly IBusinessProfileService _service;

    public BusinessProfileController(IBusinessProfileService service)
    {
        _service = service;
    }

        // ------------------- ADD BUSINESS PROFILE -------------------
        [HttpPost("addBusinessProfile")]
        public async Task<IActionResult> AddBusinessProfile(
            [FromForm] BusinessProfileCreateDto businessProfileDto,
            IFormFile? bannerFile,
            IFormFile? logoFile)
        {
            return await _service.AddBusinessProfile(businessProfileDto, bannerFile, logoFile);
        }

        //--------------Update to Blob-------------

        [HttpPost("upload_Business_Images_toBlob")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file, string ImageType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var result = await _service.UploadProfileImageAsync(file, ImageType);

            // return string directly (frontend unchanged)
            return Ok(result);
        }

        // ------------------- UPDATE BANNER -------------------
        [HttpPut("update-banner/{busRegId}")]
        public async Task<IActionResult> UpdateBannerPath(int busRegId, [FromBody] UpdateBannerRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.BannerPath))
                return BadRequest("Banner path cannot be empty");

            var updated = await _service.UpdateBannerPathAsync(busRegId, request.BannerPath);
            if (!updated)
                return NotFound($"Business profile with BusRegId {busRegId} not found.");

            return Ok(new { message = "Banner path updated successfully" });
        }

        // ------------------- UPDATE LOGO -------------------
        [HttpPut("update-logo/{busRegId}")]
        public async Task<IActionResult> UpdateLogoPath(int busRegId, [FromBody] UpdateLogoRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.LogoPath))
                return BadRequest("Logo path cannot be empty");

            var updated = await _service.UpdateLogoPathAsync(busRegId, request.LogoPath);
            if (!updated)
                return NotFound($"Business profile with BusRegId {busRegId} not found.");

            return Ok(new { message = "Logo path updated successfully" });
        }

        // ------------------- GET ALL SUBCATEGORIES -------------------
        [HttpGet("GetAllProductsubcategories")]
        public async Task<IActionResult> GetAllProductsubcategories()
        {
            var subCategories = await _service.GetAllSubCategoriesAsync();
            return Ok(subCategories);
        }

        // ------------------- GET SUBCATEGORY DETAILS -------------------
        [HttpGet("GetSubcatdetails_onaddproductform")]
        public async Task<IActionResult> GetBySubCategory(int subcatId)
        {
            var result = await _service.GetDetailsBySubCategoryAsync(subcatId);

            if ((result.ProductTypes == null || !result.ProductTypes.Any()) &&
                (result.Fabrics == null || !result.Fabrics.Any()) &&
                (result.Designs == null || !result.Designs.Any()))
            {
                return NotFound(new { message = "No details found for this subcategory." });
            }

            return Ok(result);
        }

        // ------------------- GET SUBCATEGORIES FOR BUSINESS -------------------
        [HttpGet("GetProductCategoriesbybusregid")]
        public IActionResult GetProductSubCategories(int busRegId)
        {
            var subCategories = _service.GetProductSubCategoriesByBusRegId(busRegId);

            if (subCategories == null || !subCategories.Any())
                return NotFound(new { message = "No subcategories found for the given BusRegId." });

            return Ok(subCategories);
        }

        // ------------------- GET ALL BUSINESS PROFILES -------------------
        [HttpGet("getBusinessProfiles")]
        public async Task<IActionResult> GetBusinessProfiles()
        {
            var profiles = await _service.GetAllBusinessProfilesAsync();
            return Ok(profiles);
        }

        // ------------------- GET BUSINESS PROFILES BY BUSREGID -------------------
        [HttpGet("getBusinessProfilesByBusRegId")]
        public async Task<IActionResult> GetBusinessProfilesByBusRegId(int busRegId)
        {
            var profiles = await _service.GetBusinessProfilesByBusRegIdAsync(busRegId);

            if (profiles == null || !profiles.Any())
                return NotFound(new { message = "No business profiles found for the given BusRegId" });

            return Ok(new { message = "Business profiles retrieved successfully", data = profiles });
        }

        // ------------------- GET PRODUCTS BY BUSREGID & SUBCATID -------------------
        [HttpGet("by-busreg-and-subcat")]
        public IActionResult GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
        {
            var products = _service.GetProductsByBusRegIdAndSubcatId(busRegId, prodSubcatId);
            if (products == null || !products.Any())
                return NotFound("No products found for the given criteria.");

            return Ok(products);
        }

        // ------------------- GET BUSINESS PROFILES WITH DISCOUNTED PRODUCTS -------------------
        [HttpGet("BusinessprofileswithDiscountproducts")]
        public async Task<IActionResult> GetStoresWithDiscountedProducts()
        {
            var result = await _service.GetBusinessProfilesWithDiscountedProductsAsync();
            if (result == null || !result.Any())
                return NotFound("No business profiles with discounted products found.");

            return Ok(result);
        }

        // ------------------- GET UNIQUE COUNTRIES -------------------
        [HttpGet("uniquecountries")]
        public async Task<IActionResult> GetUniqueCountries()
        {
            var countries = await _service.GetUniqueCountriesAsync();
            return Ok(countries);
        }
    }
}
