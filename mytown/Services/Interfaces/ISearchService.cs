using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ISearchService
    {
        List<Products> SearchBusinessesWithProducts(string locationQuery, string productQuery);
        Task<List<BusinessProfile>> SearchBusinessesAsync(string location, string categoryProduct);
        List<BusinessProfile> GetBusinessProfilesByLocation(string location);
        SearchResultDto GetBusinessProfilesAndProductsBySearchTerm(string searchTerm, string? locationQuery = null);
        SearchResultDto GetBusinessProfilesAndProductsByProductAndLocation(string productSearchTerm, string locationSearchTerm);
        Task<IEnumerable<ProductSubCategory>> GetProductSubCategoriesByLocationAsync(string location);
        Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByLocationAsync(string location);
        List<BusinessProfile> GetBusinessProfilesByFilters(string? searchTerm, string? locationQuery);
        Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByProductAsync(string productName);
        Task<BusinessAndServiceSearchResultsDto> GetBusinessAndServiceSearchResults(string? searchTerm, string? town,
    string? city,
    string? state,
    string? country);

        // New method
        Task<TrackingResultDto> TrackOrderByTrackingIdAsync(string trackingId);
        Task<IEnumerable<PopularCityDto>> GetPopularCitiesAsync();
        Task<SenderOrderTrackingDto?> GetSenderOrderTrackingAsync(string trackingId);
    }
}