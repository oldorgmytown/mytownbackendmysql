using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<SearchService> _logger;

        public SearchService(
            ISearchRepository searchRepository,
            ILogger<SearchService> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        public List<Products> SearchBusinessesWithProducts(string locationQuery, string productQuery)
        {
            return _searchRepository.SearchBusinessesWithProducts(locationQuery, productQuery);
        }

        public async Task<List<BusinessProfile>> SearchBusinessesAsync(string location, string categoryProduct)
        {
            return await _searchRepository.SearchBusinessesAsync(location, categoryProduct);
        }

        public List<BusinessProfile> GetBusinessProfilesByLocation(string location)
        {
            return _searchRepository.GetBusinessProfilesByLocation(location);
        }

        public SearchResultDto GetBusinessProfilesAndProductsBySearchTerm(string searchTerm, string? locationQuery = null)
        {
            return _searchRepository.GetBusinessProfilesAndProductsBySearchTerm(searchTerm, locationQuery);
        }

        public SearchResultDto GetBusinessProfilesAndProductsByProductAndLocation(
            string productSearchTerm,
            string locationSearchTerm)
        {
            return _searchRepository
                .GetBusinessProfilesAndProductsByProductAndLocation(productSearchTerm, locationSearchTerm);
        }

        public async Task<IEnumerable<ProductSubCategory>> GetProductSubCategoriesByLocationAsync(string location)
        {
            return await _searchRepository.GetProductSubCategoriesByLocationAsync(location);
        }

        public async Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByLocationAsync(string location)
        {
            return await _searchRepository.GetBusinessCategoriesByLocationAsync(location);
        }

        public List<BusinessProfile> GetBusinessProfilesByFilters(string? searchTerm, string? locationQuery)
        {
            return _searchRepository.GetBusinessProfilesByFilters(searchTerm, locationQuery);
        }

        public async Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByProductAsync(string productName)
        {
            return await _searchRepository.GetBusinessCategoriesByProductAsync(productName);
        }

        // get both business profiles and service profiles
        public async Task<BusinessAndServiceSearchResultsDto> GetBusinessAndServiceSearchResults(
            string? searchTerm,
            string? locationQuery)
        {
            return await _searchRepository.GetBusinessAndServiceSearchResults(
                searchTerm,
                locationQuery);
        }

        // Track order by tracking ID
        public async Task<TrackingResultDto> TrackOrderByTrackingIdAsync(string trackingId)
        {
            return await _searchRepository.TrackOrderByTrackingIdAsync(trackingId);
        }

        //  Get popular cities
        public async Task<IEnumerable<PopularCityDto>> GetPopularCitiesAsync()
        {
            return await _searchRepository.GetPopularCitiesAsync();
        }

        public async Task<SenderOrderTrackingDto?> GetSenderOrderTrackingAsync(string trackingId)
        {
            return await _searchRepository
                .GetSenderOrderTrackingAsync(trackingId);
        }

    }
}