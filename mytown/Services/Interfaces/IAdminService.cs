using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.Services.Interfaces
{
    public interface IAdminService
    {
        Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize, string? search);
        Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessesstoresByStatusPaginatedAsync(string status, int page, int pageSize, string? search);
        //Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessesservicesByStatusPaginatedAsync(string status, int page, int pageSize);
        Task<(IEnumerable<object> Records, int TotalRecords)>
    GetBusinessesservicesByStatusPaginatedAsync(
        string status,
        string? searchTerm,
        int page,
        int pageSize);
        Task<object> BusinessprofilestatuscountsAsync();
        Task<bool> UpdateProfileStatusbyAdminAsync(int busRegId, string status, string? comment);
        Task<object> GetDashboardCountsAsync();
        Task<(int uniqueTowns, int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync();
        Task<int> GetBusinessRegisterCountAsync();
        Task<int> GetShoppersRegisterCountAsync();
        Task<(List<ShopperRegister> records, int totalCount)>
    GetShoppersByStatusAsync(string status, int page, int pageSize, string? search);
        //Decativate shopper account
        Task<bool> DeactivateShopperAsync(int shopperRegId);
        //Recativate shopper account
      

        // Shopper Summary on Admin panel
        Task<ShopperStatsDto> GetActiveShopperStatsAsync();



        Task<int> GetCourierRegisterCountAsync();
       // Task<(IEnumerable<object> Records, int TotalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize);
        Task<bool> UpdateShopperStatusByAdminAsync(int shopperId, string status);
        Task<(IEnumerable<object> Records, int TotalRecords)>
  GetCourierRegistersPaginatedAsync(int page, int pageSize, string? search);

        Task<(IEnumerable<object> Records, int TotalRecords)>
   GetTransporterRegistersPaginatedAsync(int page, int pageSize, string? search);
        Task<IEnumerable<object>> GetLocationsWithCompletedStoresAsync();
        Task<List<LocationStoresDto>> GetLocationsWithCompletedStores_DapperAsync();
        Task<List<LocationStoresDto>> GetLocationsWithCompletedStores_EFAsync();
        Task TestConnectionAsync();

        // courier tab
        Task<List<AdminCouriertabDto>> GetCouriersAsync();
        Task<AdminLocationCourierSummaryDto> GetLocationCourierSummaryAsync();
        Task<List<BranchBasicDto>> GetBasicBranchesAsync(int courierId);

        //for branches 
        Task<CourierBranchDto> GetBranchAsync(int branchId);

        Task<(IEnumerable<object> Records, int TotalRecords)>
    GetSenderRegistersPaginatedAsync(
        int page,
        int pageSize,
        string? search);

        Task<bool> UpdateServiceProfileStatusByAdminAsync(
        int busRegId,
        string status,
        string? comments = null);
    }
}
