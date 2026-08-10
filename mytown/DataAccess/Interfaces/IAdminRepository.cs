using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IAdminRepository
    {
        // Admin panel methods

        Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize, string? search = null);
        Task<(List<BusinessRegister> Records, int TotalRecords)>
      GetBusinessesstoresByStatusPaginatedAsync(string status, int page, int pageSize, string? search);
        Task<(List<BusinessRegister> Records, int TotalRecords)>
    GetBusinessesservicesByStatusPaginated(
        string status,
        string? searchTerm,
        int page,
        int pageSize);
        Task<Dictionary<string, Dictionary<string, int>>> Businessprofilestatuscounts();

        Task<bool> UpdateProfileStatusbyAdminAsync(int busRegId, string status, string? comments = null);

        Task<(List<ShopperRegister> records, int totalCount)>
       GetShoppersByStatusAsync(string status, int page, int pageSize, string? search);



        //Shopper Summary on Admin panel
        Task<ShopperStatsDto> GetActiveShopperStatsAsync();

        Task<bool> UpdateShopperStatusAsync(int shopperId, string newStatus);
        Task<ShopperRegister?> GetShopperByIdAsync(int shopperId);

        Task<AdminDashboardcountDto> GetDashboardCountsAsync();
        Task<(int uniqueTowns, int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync();

        Task<int> GetBusinessRegisterCountAsync();
        Task<int> GetShoppersRegisterCountAsync();
        Task<int> GetCourierserviceCountAsync();
        Task<(IEnumerable<CourierService> records, int totalRecords)>
GetCourierRegistersPaginatedAsync(int page, int pageSize, string? search);

        Task<(IEnumerable<TransporterRegisterDto> records, int totalRecords)>
     GetTransporterRegistersPaginatedAsync(int page, int pageSize, string? search);
        // shopper tab
        Task<bool> DeactivateShopperAsync(int shopperRegId);
        //landing page
        Task<List<LocationStoresDto>> GetLocationsWithCompletedStoresAsync();

        Task<List<LocationStoresDto>> GetLocationsWithCompletedStores_DapperAsync();
        Task<List<LocationStoresDto>> GetLocationsWithCompletedStores_EFAsync();
        Task TestConnectionAsync();

        //Courier tab

        Task<List<CourierService>> GetAllCouriersAsync();

        Task<AdminLocationCourierSummaryDto> GetCourierLocationSummaryAsync();
        Task<List<BranchBasicDto>> GetBasicBranches(int courierId);       

        Task<CourierBranchDto> GetBranchAsync(int branchId);

        Task<(IEnumerable<SenderRegisterDto> records, int totalRecords)>
    GetSenderRegistersPaginatedAsync(
        int page,
        int pageSize,
        string? search);
   
    
        Task<bool> UpdateServiceProfileStatusByAdminAsync(
            int busRegId,
            string status,
            string? comments = null);

        // Orders tab — full combined order details by store order id
        Task<OrderFullDetailsDto?> GetOrderFullDetailsByStoreOrderIdAsync(int storeOrderId);

        // Orders tab — summary counts for dashboard cards
        Task<OrdersSummaryCountsDto> GetOrdersSummaryCountsAsync();

        // Orders tab — full order list, paginated, filterable by status tab + search
        Task<(List<OrderFullDetailsDto> Records, int TotalRecords)>
            GetAllOrdersFullDetailsPaginatedAsync(int page, int pageSize, string? status, string? search);
    }
}