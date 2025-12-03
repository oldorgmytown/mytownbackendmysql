using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface ISearchRepository
    {
        // Get list of products based on location and category/subcategory/product search
        List<Products> SearchBusinessesWithProducts(string locationQuery, string productQuery);

        // Search for businesses by location and category/product
        Task<List<BusinessProfile>> SearchBusinessesAsync(string location, string categoryProduct);

        // Get business profiles by location
        List<BusinessProfile> GetBusinessProfilesByLocation(string location);

        // stores and profiles based on both search bars - location, search term for product n store
        SearchResultDto GetBusinessProfilesAndProductsBySearchTerm(string searchTerm, string? locationQuery = null);

        // Get business profiles and products based on both product and location search terms
        SearchResultDto GetBusinessProfilesAndProductsByProductAndLocation(string productSearchTerm, string locationSearchTerm);

        //Get product sub categories for that searched town or exitsing in that town
        Task<IEnumerable<ProductSubCategory>> GetProductSubCategoriesByLocationAsync(string location);

        Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByLocationAsync(string location);

        //2-12-25  search stores only
        List<BusinessProfile> GetBusinessProfilesByFilters(string? searchTerm, string? locationQuery);
        Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByProductAsync(string productName);
    }
}
