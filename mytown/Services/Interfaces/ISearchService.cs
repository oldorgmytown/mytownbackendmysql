using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ISearchService
    {
        // Products search
        List<Products> SearchBusinessesWithProducts(string locationQuery, string productQuery);

        // Stores search
        Task<List<BusinessProfile>> SearchBusinessesAsync(string location, string categoryProduct);

        // Profiles by location
        List<BusinessProfile> GetBusinessProfilesByLocation(string location);

        // Profiles + products (single search bar)
        SearchResultDto GetBusinessProfilesAndProductsBySearchTerm(string searchTerm, string? locationQuery = null);

        // Profiles + products (product + location)
        SearchResultDto GetBusinessProfilesAndProductsByProductAndLocation(
            string productSearchTerm,
            string locationSearchTerm);

        // Product subcategories
        Task<IEnumerable<ProductSubCategory>> GetProductSubCategoriesByLocationAsync(string location);

        // Business categories by location
        Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByLocationAsync(string location);

        // Stores only
        List<BusinessProfile> GetBusinessProfilesByFilters(string? searchTerm, string? locationQuery);

        // Categories by product
        Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByProductAsync(string productName);
    }
}
