using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;

namespace mytown.Controllers
{
  //  [Authorize]
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            ISearchRepository searchRepository,
            IConfiguration configuration,
            ILogger<SearchController> logger)
        {
            _searchRepository = searchRepository ?? throw new ArgumentNullException(nameof(searchRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //search to get products data based on location and category/product
        //1-12-25 (no products list)
        [HttpGet("search")]
        public IActionResult SearchBusinesses([FromQuery] string locationQuery, [FromQuery] string productQuery)
        {
            var filteredProducts = _searchRepository.SearchBusinessesWithProducts(locationQuery, productQuery);

            if (filteredProducts.Count == 0)
                return NotFound(new { code = 404, message = "No products found matching your criteria." });

            return Ok(new { code = 200, data = filteredProducts });
        }

        //search to get business stores data based on location and category/product
        [HttpGet("searchstore")]
        public async Task<IActionResult> SearchBusinessstores([FromQuery] string location, [FromQuery] string categoryProduct)
        {
            if (string.IsNullOrEmpty(location) && string.IsNullOrEmpty(categoryProduct))
            {
                return BadRequest(new { code = 400, message = "At least one search term (location or category/product) must be provided." });
            }

            var searchResults = await _searchRepository.SearchBusinessesAsync(location, categoryProduct);

            if (searchResults.Count == 0)
            {
                return NotFound(new { code = 404, message = "No matching business profiles found." });
            }

            return Ok(new { code = 200, data = searchResults });
        }

        //get profile based on location search 
        [HttpGet("searchbylocation")]
        public ActionResult<List<BusinessProfile>> GetBusinessProfilesByLocation([FromQuery] string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest(new { code = 400, message = "Location is required." });
            }

            var profiles = _searchRepository.GetBusinessProfilesByLocation(location);

            if (profiles == null || profiles.Count == 0)
            {
                return NotFound(new { code = 404, message = "No matching business profiles found." });
            }

            return Ok(new { code = 200, data = profiles });
        }

        //[HttpGet("searchproductandbusiness")]
        //public IActionResult SearchBusinessByCategoryOrProduct(
        //    [FromQuery] string searchterm,
        //    [FromQuery] string? location)
        //{
        //    if (string.IsNullOrEmpty(searchterm))
        //    {
        //        return BadRequest(new { code = 400, message = "Search parameter is required." });
        //    }

        //    var results = _searchRepository.GetBusinessProfilesAndProductsBySearchTerm(searchterm, location);

        //    if (results == null)
        //    {
        //        return NotFound(new { code = 404, message = "No businesses or products found matching the criteria." });
        //    }

        //    return Ok(new { code = 200, data = results });
        //}

        //[HttpGet("SearchProfilesandProducts_ByProductAndLocation")]
        //public IActionResult SearchProfilesByProductAndLocation(
        //    [FromQuery] string productSearchTerm,
        //    [FromQuery] string locationSearchTerm)
        //{
        //    if (string.IsNullOrWhiteSpace(productSearchTerm) || string.IsNullOrWhiteSpace(locationSearchTerm))
        //    {
        //        return BadRequest(new { code = 400, message = "Both product and location search terms are required." });
        //    }

        //    var businessProfiles = _searchRepository.GetBusinessProfilesAndProductsByProductAndLocation(productSearchTerm, locationSearchTerm);

        //    if (businessProfiles == null)
        //    {
        //        return NotFound(new { code = 404, message = "No matching business profiles found." });
        //    }

        //    return Ok(new { code = 200, data = businessProfiles });
        //}

        [HttpGet("product-subcategories-by-location")]
        public async Task<IActionResult> GetProductSubCategoriesByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest(new { code = 400, message = "Location cannot be empty." });

            var subCategories = await _searchRepository.GetProductSubCategoriesByLocationAsync(location);

            if (!subCategories.Any())
                return NotFound(new { code = 404, message = "No product subcategories found for the given location." });

            return Ok(new { code = 200, data = subCategories });
        }

        [HttpGet("business-categories-by-location")]
        public async Task<IActionResult> GetBusinessCategoriesByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest(new { code = 400, message = "Location cannot be empty." });

            var busCategories = await _searchRepository.GetBusinessCategoriesByLocationAsync(location);

            if (!busCategories.Any())
                return NotFound(new { code = 404, message = "No business categories found for the given location." });

            return Ok(new { code = 200, data = busCategories });
        }

       
    }
}
