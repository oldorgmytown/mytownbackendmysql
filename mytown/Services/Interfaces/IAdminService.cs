using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IAdminService
    {
        Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize, string? search);
        Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessesstoresByStatusPaginatedAsync(string status, int page, int pageSize, string? search);
        Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessesservicesByStatusPaginatedAsync(string status, int page, int pageSize);
        Task<object> BusinessprofilestatuscountsAsync();
        Task<bool> UpdateProfileStatusbyAdminAsync(int busRegId, string status, string? comment);
        Task<object> GetDashboardCountsAsync();
        Task<(int uniqueTowns, int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync();
        Task<int> GetBusinessRegisterCountAsync();
        Task<int> GetShoppersRegisterCountAsync();
        Task<int> GetCourierRegisterCountAsync();
        Task<(IEnumerable<object> Records, int TotalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize);
        Task<bool> UpdateShopperStatusByAdminAsync(int shopperId, string status);
        Task<(IEnumerable<object> Records, int TotalRecords)> GetCourierRegistersPaginatedAsync(int page, int pageSize);
        Task<IEnumerable<object>> GetLocationsWithCompletedStoresAsync();
    }
}
