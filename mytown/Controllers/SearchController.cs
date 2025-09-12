using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Services;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace mytown.Controllers
{

    [Route("api/search")]
    [ApiController]
    public class SearchController: ControllerBase

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
        [HttpGet("search")]
        public IActionResult SearchBusinesses([FromQuery] string locationQuery, [FromQuery] string productQuery)
        {
            var filteredProducts = _searchRepository.SearchBusinessesWithProducts(locationQuery, productQuery);

            if (filteredProducts.Count == 0)
                return NotFound("No products found matching your criteria.");

            return Ok(filteredProducts);
        }

        //search to get business stores data based on location and category/product
        [HttpGet("searchstore")]
        public async Task<IActionResult> SearchBusinessstores([FromQuery] string location, [FromQuery] string categoryProduct)
        {
            // Ensure at least one search parameter is provided
            if (string.IsNullOrEmpty(location) && string.IsNullOrEmpty(categoryProduct))
            {
                return BadRequest("At least one search term (location or category/product) must be provided.");
            }

            // Fetch search results from the repository
            var searchResults = await _searchRepository.SearchBusinessesAsync(location, categoryProduct);

            // If no results are found, return an appropriate message
            if (searchResults.Count == 0)
            {
                return NotFound("No matching business profiles found.");
            }

            return Ok(searchResults);
        }

        //[HttpGet("searchbusprofilebylocandproduct")]
        //public IActionResult Search([FromQuery] string location, [FromQuery] string product)
        //{
        //    var results = _userRepository.SearchBusinessProfiles(location, product);

        //    if (!results.Any())
        //    {
        //        return NotFound("No matching business profiles found.");
        //    }

        //    return Ok(results);
        //}



        ///** get profile based on location search 
        [HttpGet("searchbylocation")]
        public ActionResult<List<businessprofile>> GetBusinessProfilesByLocation([FromQuery] string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Location is required.");
            }

            var profiles = _searchRepository.GetBusinessProfilesByLocation(location);

            if (profiles == null || profiles.Count == 0)
            {
                return NotFound("No matching business profiles found.");
            }

            return Ok(profiles);
        }


        [HttpGet("searchproductandbusiness")]
        public IActionResult SearchBusinessByCategoryOrProduct(
     [FromQuery] string searchterm,
     [FromQuery] string? location)
        {
            if (string.IsNullOrEmpty(searchterm))
            {
                return BadRequest("search parameteris required.");
            }

            var results = _searchRepository.GetBusinessProfilesAndProductsBySearchTerm(searchterm, location);

            if (results == null)
            {
                return NotFound("No businesses or products found matching the criteria.");
            }

            return Ok(results);
        }

        [HttpGet("SearchProfilesandProducts_ByProductAndLocation")]
        public IActionResult SearchProfilesByProductAndLocation([FromQuery] string productSearchTerm, [FromQuery] string locationSearchTerm)
        {
            if (string.IsNullOrWhiteSpace(productSearchTerm) || string.IsNullOrWhiteSpace(locationSearchTerm))
            {
                return BadRequest(new { message = "Both product and location search terms are required." });
            }

            var businessProfiles = _searchRepository.GetBusinessProfilesAndProductsByProductAndLocation(productSearchTerm, locationSearchTerm);

            if (businessProfiles == null)
            {
                return NotFound(new { message = "No matching business profiles found." });
            }

            return Ok(businessProfiles);
        }

        [HttpGet("product-subcategories-by-location")]
        public async Task<IActionResult> GetProductSubCategoriesByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location cannot be empty.");

            var subCategories = await _searchRepository.GetProductSubCategoriesByLocationAsync(location);

            if (!subCategories.Any())
                return NotFound("No product subcategories found for the given location.");

            return Ok(subCategories);
        }

        [HttpGet("business-categories-by-location")]
        public async Task<IActionResult> GetBusinessCategoriesByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location cannot be empty.");

            var busCategories = await _searchRepository.GetBusinessCategoriesByLocationAsync(location);

            if (!busCategories.Any())
                return NotFound("No Business categories found for the given location.");

            return Ok(busCategories);
        }

    }
}
