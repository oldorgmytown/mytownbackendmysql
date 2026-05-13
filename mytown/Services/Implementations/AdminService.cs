using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IEmailService _emailService;

        public AdminService(IAdminRepository adminRepo, IEmailService emailService)
        {
            _adminRepo = adminRepo;
            _emailService = emailService;
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

        public Task<(List<ShopperRegister>, int)>
    GetShoppersByStatusAsync(string status, int page, int pageSize, string? search)
        {
            return _adminRepo.GetShoppersByStatusAsync(status, page, pageSize, search);

        }

      
        // get shopper stats on admin panel
        public async Task<ShopperStatsDto> GetActiveShopperStatsAsync()
        {
            return await _adminRepo.GetActiveShopperStatsAsync();
        }

        public async Task<bool> UpdateShopperStatusByAdminAsync(int shopperId, string status)
        {
            // 1️ Update status
            var updated = await _adminRepo.UpdateShopperStatusAsync(shopperId, status);

            if (!updated)
                return false;

            // 2️ Get shopper details
            var shopper = await _adminRepo.GetShopperByIdAsync(shopperId);

            if (shopper == null)
                return false;

            // 3️ Send email only if reactivated
            if (status.ToLower() == "active")
            {
                await _emailService.SendShopperReactivationEmailAsync(
                    shopper.Email,
                    shopper.Username
                );
            }

            return true;
        }
        public async Task<bool> DeactivateShopperAsync(int shopperRegId)
        {
            var shopper = await _adminRepo.GetShopperByIdAsync(shopperRegId);

            if (shopper == null)
                return false;

            var result = await _adminRepo.DeactivateShopperAsync(shopperRegId);

            if (result)
            {
                await _emailService.SendShopperDeactivationEmailAsync(
                    shopper.Email,
                    shopper.Username
                );
            }

            return result;
        }

        //public async Task<bool> SendReativateShopperemail(int shopperRegId)
        //{
        //    var shopper = await _adminRepo.GetShopperByIdAsync(shopperRegId);

        //    if (shopper == null)
        //        return false;


        //        await _emailService.SendShopperReactivationEmailAsync(
        //            shopper.Email,
        //            shopper.Username
        //        );


        //    return true;
        //}

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
  GetCourierRegistersPaginatedAsync(int page, int pageSize, string? search)
        {
            return await _adminRepo.GetCourierRegistersPaginatedAsync(page, pageSize, search);
        }

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
    GetTransporterRegistersPaginatedAsync(int page, int pageSize, string? search)
        {
            return await _adminRepo.GetTransporterRegistersPaginatedAsync(page, pageSize, search);
        }
        public async Task<IEnumerable<object>> GetLocationsWithCompletedStoresAsync()
        {
            return await _adminRepo.GetLocationsWithCompletedStoresAsync();
        }

        public async Task<List<LocationStoresDto>>
        GetLocationsWithCompletedStores_DapperAsync()
        {
            return await _adminRepo
                .GetLocationsWithCompletedStores_DapperAsync();
        }

        public async Task<List<LocationStoresDto>>
            GetLocationsWithCompletedStores_EFAsync()
        {
            return await _adminRepo
                .GetLocationsWithCompletedStores_EFAsync();
        }

        public async Task TestConnectionAsync()
        {
            await _adminRepo.TestConnectionAsync();
        }


        //courier tab
        public async Task<List<AdminCouriertabDto>> GetCouriersAsync()
        {
            var couriers = await _adminRepo.GetAllCouriersAsync();

            return couriers.Select(c => new AdminCouriertabDto
            {
                CourierId = c.CourierId,
                CourierServiceName = c.CourierServiceName,
                CourierWebsiteName = c.CourierWebsiteName,
                CourierEmail = c.CourierEmail,
                CourierPhone = c.CourierPhone
            }).ToList();
        }

        public async Task<AdminLocationCourierSummaryDto> GetLocationCourierSummaryAsync()
        {
            var summary = await _adminRepo.GetCourierLocationSummaryAsync();

            return new AdminLocationCourierSummaryDto
            {
                TotalCouriers = summary.TotalCouriers,
                TotalCountries = summary.TotalCountries,
                TotalStates = summary.TotalStates,
                TotalCities = summary.TotalCities,
                TotalTowns = summary.TotalTowns
            };
        }

        public async Task<List<BranchBasicDto>> GetBasicBranchesAsync(int courierId)
        {
            if (courierId <= 0)
            {
                throw new ArgumentException("Invalid courierId");
            }

            var branches = await _adminRepo.GetBasicBranches(courierId);

            return branches ?? new List<BranchBasicDto>();
        }

        // branches 

        public async Task<CourierBranchDto> GetBranchAsync(int branchId)
        {
            return await _adminRepo.GetBranchAsync(branchId);
        }

        public async Task<(IEnumerable<object> Records, int TotalRecords)>
GetSenderRegistersPaginatedAsync(
    int page,
    int pageSize,
    string? search)
        {
            return await _adminRepo
                .GetSenderRegistersPaginatedAsync(
                    page,
                    pageSize,
                    search);
        }

    }
}
