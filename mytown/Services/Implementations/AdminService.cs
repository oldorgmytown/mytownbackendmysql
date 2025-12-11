using mytown.DataAccess.Interfaces;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepo;

        public AdminService(IAdminRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
            GetBusinessRegistersPaginatedAsync(int page, int pageSize, string? search)
        {
            return await _adminRepo.GetBusinessRegistersPaginatedAsync(page, pageSize, search);
        }

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
            GetBusinessesstoresByStatusPaginatedAsync(string status, int page, int pageSize, string? search)
        {
            return await _adminRepo.GetBusinessesstoresByStatusPaginatedAsync(status, page, pageSize, search);
        }

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
            GetBusinessesservicesByStatusPaginatedAsync(string status, int page, int pageSize)
        {
            return await _adminRepo.GetBusinessesservicesByStatusPaginated(status, page, pageSize);
        }

        public async Task<object> BusinessprofilestatuscountsAsync()
        {
            return await _adminRepo.Businessprofilestatuscounts();
        }

        public async Task<bool> UpdateProfileStatusbyAdminAsync(int busRegId, string status, string? comment)
        {
            return await _adminRepo.UpdateProfileStatusbyAdminAsync(busRegId, status, comment);
        }

        public async Task<object> GetDashboardCountsAsync()
        {
            return await _adminRepo.GetDashboardCountsAsync();
        }

        public async Task<(int uniqueTowns, int uniqueCities, int uniqueStates, int uniqueCountries)>
            GetUniqueCountsAsync()
        {
            return await _adminRepo.GetUniqueCountsAsync();
        }

        public async Task<int> GetBusinessRegisterCountAsync()
        {
            return await _adminRepo.GetBusinessRegisterCountAsync();
        }

        public async Task<int> GetShoppersRegisterCountAsync()
        {
            return await _adminRepo.GetShoppersRegisterCountAsync();
        }

        public async Task<int> GetCourierRegisterCountAsync()
        {
            return await _adminRepo.GetCourierserviceCountAsync();
        }

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
            GetShopperRegistersPaginatedAsync(int page, int pageSize)
        {
            return await _adminRepo.GetShopperRegistersPaginatedAsync(page, pageSize);
        }

        public async Task<bool> UpdateShopperStatusByAdminAsync(int shopperId, string status)
        {
            // Keeping your existing repository call — NO change to repo method
            return await _adminRepo.UpdateProfileStatusbyAdminAsync(shopperId, status);
        }

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
            GetCourierRegistersPaginatedAsync(int page, int pageSize)
        {
            return await _adminRepo.GetCourierRegistersPaginatedAsync(page, pageSize);
        }

        public async Task<IEnumerable<object>> GetLocationsWithCompletedStoresAsync()
        {
            return await _adminRepo.GetLocationsWithCompletedStoresAsync();
        }
    }
}
