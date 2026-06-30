using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Services.Interfaces;
using Stripe;

namespace mytown.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            ISearchService searchService,
            IConfiguration configuration,
            ILogger<SearchController> logger)
        {
            _searchService = searchService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("search")]
        public IActionResult SearchBusinesses([FromQuery] string locationQuery, [FromQuery] string productQuery)
        {
            var result = _searchService.SearchBusinessesWithProducts(locationQuery, productQuery);

            if (!result.Any())
                return NotFound(new { code = 404, message = "No products found matching your criteria." });

            return Ok(new { code = 200, data = result });
        }

        [HttpGet("searchstore")]
        public async Task<IActionResult> SearchBusinessstores(string location, string categoryProduct)
        {
            if (string.IsNullOrEmpty(location) && string.IsNullOrEmpty(categoryProduct))
                return BadRequest(new { code = 400, message = "At least one search term is required." });

            var result = await _searchService.SearchBusinessesAsync(location, categoryProduct);

            if (!result.Any())
                return NotFound(new { code = 404, message = "No matching business profiles found." });

            return Ok(new { code = 200, data = result });
        }

        [HttpGet("searchbylocation")]
        public IActionResult GetBusinessProfilesByLocation(string location)
        {
            var profiles = _searchService.GetBusinessProfilesByLocation(location);

            if (!profiles.Any())
                return NotFound(new { code = 404, message = "No matching business profiles found." });

            return Ok(new { code = 200, data = profiles });
        }

        [HttpGet("searchproductandbusiness")]
        public IActionResult SearchBusinessByCategoryOrProduct(string searchterm, string? location)
        {
            var result = _searchService.GetBusinessProfilesAndProductsBySearchTerm(searchterm, location);

            return Ok(new { code = 200, data = result });
        }

        [HttpGet("SearchProfilesandProducts_ByProductAndLocation")]
        public IActionResult SearchProfilesByProductAndLocation(string productSearchTerm, string locationSearchTerm)
        {
            var result = _searchService
                .GetBusinessProfilesAndProductsByProductAndLocation(productSearchTerm, locationSearchTerm);

            return Ok(new { code = 200, data = result });
        }

        [HttpGet("product-subcategories-by-location")]
        public async Task<IActionResult> GetProductSubCategoriesByLocation(string location)
        {
            var result = await _searchService.GetProductSubCategoriesByLocationAsync(location);

            if (!result.Any())
                return NotFound(new { code = 404, message = "No product subcategories found." });

            return Ok(new { code = 200, data = result });
        }

        [HttpGet("business-categories-by-location")]
        public async Task<IActionResult> GetBusinessCategoriesByLocation(string location)
        {
            var result = await _searchService.GetBusinessCategoriesByLocationAsync(location);

            if (!result.Any())
                return NotFound(new { code = 404, message = "No business categories found." });

            return Ok(new { code = 200, data = result });
        }

        [HttpGet("searchstoresonly")]
        public IActionResult SearchStores(string? searchTerm, string? location)
        {
            var stores = _searchService.GetBusinessProfilesByFilters(searchTerm, location);

            return Ok(new { stores, storeCount = stores.Count });
        }

        [HttpGet("Searhcategoriesfilter")]
        public async Task<IActionResult> GetCategories(string? product, string? location)
        {
            var result = !string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(product)
                ? await _searchService.GetBusinessCategoriesByLocationAsync(location)
                : await _searchService.GetBusinessCategoriesByProductAsync(product!);

            return Ok(result);
        }

        // 27-05-26
        // get both business profiles and service profiles
        [HttpGet("getbusinessandservicesearchresults")]
        public async Task<IActionResult> GetBusinessAndServiceSearchResults(
            string? searchTerm,
            string? locationQuery)
        {
            var result = await _searchService.GetBusinessAndServiceSearchResults(
                searchTerm,
                locationQuery);

            return Ok(result);
        }

        // Track order by tracking ID
        //this is not working on QA
        [AllowAnonymous]
        [HttpGet("track/{trackingId}")]
        public async Task<IActionResult> TrackOrder(string trackingId)
        {
            try
            {
                var result = await _searchService.TrackOrderByTrackingIdAsync(trackingId);

                if (result == null)
                    return NotFound(new { error = "Tracking ID not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking order {TrackingId}", trackingId);
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }

        // ✅ Get popular cities
        [AllowAnonymous]
        [HttpGet("popular-cities")]
        public async Task<IActionResult> GetPopularCities()
        {
            try
            {
                var result = await _searchService.GetPopularCitiesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching popular cities");
                return StatusCode(500, new { error = "Something went wrong." });
            }
        }

        // Sender Order Tracking
[AllowAnonymous]
[HttpGet("sendertrack/{trackingId}")]
public async Task<IActionResult> GetSenderOrderTracking(string trackingId)
{
    try
    {
        var result = await _searchService
            .GetSenderOrderTrackingAsync(trackingId);

        if (result == null)
            return NotFound(new { error = "Tracking ID not found." });

        return Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,
            "Error fetching sender order tracking {TrackingId}",
            trackingId);

        return StatusCode(500,
            new { error = "Something went wrong." });
    }
}



    }
}