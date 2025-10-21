using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Services;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AdminRepository(AppDbContext context, IEmailService emailservice)
        {
            _context = context;
            _emailService = emailservice;
        }

        //ADMIN PANEL

        //to get all business profiles with status
        public async Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize, string? search = null)
        {
            var skip = (page - 1) * pageSize;

            // Step 1: start query
            var query = from b in _context.BusinessRegisters
                        join bp in _context.BusinessProfiles
                            on b.BusRegId equals bp.BusRegId into bpJoin
                        from bp in bpJoin.DefaultIfEmpty()
                        select new
                        {
                            b.BusRegId,
                            b.BusinessUsername,
                            b.Businessname,
                            b.LicenseType,
                            b.Gstin,
                            b.BusservId,
                            b.BuscatId,
                            b.Town,
                            b.BusMobileNo,
                            b.BusEmail,
                            b.IsEmailVerified,
                            b.Address1,
                            b.Address2,
                            b.businessCity,
                            b.businessState,
                            b.businessCountry,
                            b.postalCode,
                            b.Password,
                            b.BusinessRegDate,
                            ProfileStatus = bp != null && bp.profile_status != null ? bp.profile_status : "pending",
                            bp.approved_date,
                            ServiceType =
                                b.BusservId == 1 && b.BuscatId == 1 ? "product, service" :
                                b.BuscatId == 1 ? "product" :
                                b.BusservId == 1 ? "service" : "none"
                        };

            // Step 2: apply search if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Businessname.ToLower().Contains(search) ||
                    b.BusinessUsername.ToLower().Contains(search) ||
                    b.BusEmail.ToLower().Contains(search) ||
                    b.BusMobileNo.ToLower().Contains(search) ||
                    b.Town.ToLower().Contains(search) ||
                     b.businessCity.ToLower().Contains(search) ||
                    b.businessState.ToLower().Contains(search) ||
                    b.businessCountry.ToLower().Contains(search) ||
                    b.ProfileStatus.ToLower().Contains(search));
            }

            // Step 3: get total records after filtering
            var totalRecords = await query.CountAsync();

            // Step 4: apply pagination
            var records = await query
                .OrderByDescending(b => b.BusRegId)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }


        //get business stores by sttaus

        public async Task<(List<BusinessRegister> Records, int TotalRecords)>
      GetBusinessesstoresByStatusPaginatedAsync(string status, int page, int pageSize, string? search)
        {
            var query = from br in _context.BusinessRegisters
                        join bp in _context.BusinessProfiles
                            on br.BusRegId equals bp.BusRegId into bpGroup
                        from bp in bpGroup.DefaultIfEmpty()
                        where br.BuscatId == 1 && (
                            (bp != null && bp.profile_status.ToLower() == status.ToLower()) ||
                            (bp == null && status.ToLower() == "incomplete")
                        )
                        select br;

            //  Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(br =>
                    br.Businessname.ToLower().Contains(search) ||
                    br.BusinessUsername.ToLower().Contains(search) ||
                    br.BusEmail.ToLower().Contains(search) ||
                    br.Town.ToLower().Contains(search) ||
                    br.businessCity.ToLower().Contains(search) ||
                    br.businessState.ToLower().Contains(search) ||
                    br.businessCountry.ToLower().Contains(search));
            }

            int totalRecords = await query.CountAsync();

            var records = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }


        //Business summary count for profile status
        public async Task<Dictionary<string, Dictionary<string, int>>> Businessprofilestatuscounts()
        {
            var allStatuses = new[] { "incomplete", "submitted", "approved", "rejected", "blocked" };

            var businessProfiles = await _context.BusinessProfiles
                .Select(bp => new
                {
                    bp.profile_status,
                    bp.BusCatId,
                    bp.BusServId
                })
                .ToListAsync();

            // Stores
            var storeCounts = allStatuses.ToDictionary(
                status => status,
                status => businessProfiles.Count(bp =>
                    bp.profile_status.Equals(status, StringComparison.OrdinalIgnoreCase) &&
                    bp.BusCatId == 1
                )
            );

            // Services
            var serviceCounts = allStatuses.ToDictionary(
                status => status,
                status => businessProfiles.Count(bp =>
                    bp.profile_status.Equals(status, StringComparison.OrdinalIgnoreCase) &&
                    bp.BusServId == 1
                )
            );

            // Final structure
            var result = new Dictionary<string, Dictionary<string, int>>
    {
        { "stores", storeCounts },
        { "services", serviceCounts }
    };

            return result;
        }


        // Admin  - Approve, Reject, Block business profiles

        public async Task<bool> UpdateProfileStatusbyAdminAsync(int busRegId, string status, string? comments = null)
        {
            var profile = await _context.BusinessProfiles
      .Include(p => p.BusinessRegister) // Include the related BusinessRegister
      .FirstOrDefaultAsync(p => p.BusRegId == busRegId);

            if (profile == null)
                return false;

            profile.profile_status = status;
            profile.approved_date = status.ToLower() == "approved" ? DateTime.Now : profile.approved_date;

            _context.BusinessProfiles.Update(profile);

            //Save admin comments to admin_comments table
    if (!string.IsNullOrEmpty(comments))
            {
                var adminComment = new AdminComment
                {
                    BusRegId = busRegId,
                    Comments = comments,
                    Status = status, // optionally save the new status in comments table too
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _context.AdminComments.AddAsync(adminComment);
            }
            await _context.SaveChangesAsync();

            // Now capture the business details
            var business = profile.BusinessRegister;
            if (business != null)
            {
                string businessName = business.Businessname;
                string username = business.BusinessUsername;
                string email = business.BusEmail;

                // Call your email sending logic here
                await _emailService.SendBusinessStatusEmailAsync(email, username, businessName, status);
            }

            return true;
        }
        public async Task<(List<BusinessRegister> Records, int TotalRecords)> GetBusinessesservicesByStatusPaginated(string status, int page, int pageSize)
        {
            var query = from br in _context.BusinessRegisters
                        join bp in _context.BusinessProfiles
                            on br.BusRegId equals bp.BusRegId into bpGroup
                        from bp in bpGroup.DefaultIfEmpty() // Left join
                        where
                            br.BusservId == 1 && // Filter by servicecategory
                            (
                                (bp != null && bp.profile_status.ToLower() == status.ToLower()) ||
                                (bp == null && status.ToLower() == "incomplete")
                            )
                        select br;

            int totalRecords = await query.CountAsync();

            var records = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }


        public async Task<AdminDashboardcountDto> GetDashboardCountsAsync()
        {
            // Unique Towns
            var uniqueTowns = await _context.BusinessRegisters
                .Select(b => b.Town)
                .Where(town => !string.IsNullOrEmpty(town))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.Town)
                        .Where(town => !string.IsNullOrEmpty(town))
                )
                .Distinct()
                .CountAsync();

            // Unique Cities
            var uniqueCities = await _context.BusinessRegisters
                .Select(b => b.businessCity)
                .Where(city => !string.IsNullOrEmpty(city))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.City)
                        .Where(city => !string.IsNullOrEmpty(city))
                )
                .Distinct()
                .CountAsync();

            // Unique States
            var uniqueStates = await _context.BusinessRegisters
                .Select(b => b.businessState)
                .Where(state => !string.IsNullOrEmpty(state))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.State)
                        .Where(state => !string.IsNullOrEmpty(state))
                )
                .Distinct()
                .CountAsync();

            // Unique Countries
            var uniqueCountries = await _context.BusinessRegisters
                .Select(b => b.businessCountry)
                .Where(country => !string.IsNullOrEmpty(country))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.Country)
                        .Where(country => !string.IsNullOrEmpty(country))
                )
                .Distinct()
                .CountAsync();

            // Other counts
            var businessCount = await _context.BusinessRegisters.CountAsync();
            var shopperCount = await _context.ShopperRegisters.CountAsync();
            var courierServiceCount = await _context.CourierService.CountAsync();

            // Return everything in one object
            return new AdminDashboardcountDto
            {
                UniqueTowns = uniqueTowns,
                UniqueCities = uniqueCities,
                UniqueStates = uniqueStates,
                UniqueCountries = uniqueCountries,
                BusinessRegisterCount = businessCount,
                ShopperRegisterCount = shopperCount,
                CourierServiceCount = courierServiceCount
            };
        }


        public async Task<(int uniqueTowns,int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync()
        {
                    var uniqueTowns = await _context.BusinessRegisters
            .Select(b => b.Town)
            .Where(town => !string.IsNullOrEmpty(town))
            .Union(
                _context.ShopperRegisters
                    .Select(s => s.Town)
                    .Where(town => !string.IsNullOrEmpty(town))
            )
            .Distinct()
            .CountAsync();

            // Fetch unique cities from both tables
            var uniqueCities = await _context.BusinessRegisters
                .Select(b => b.businessCity)
                .Where(city => !string.IsNullOrEmpty(city))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.City)
                        .Where(city => !string.IsNullOrEmpty(city))
                )
                .Distinct()
                .CountAsync();

            // Fetch unique states from both tables
            var uniqueStates = await _context.BusinessRegisters
                .Select(b => b.businessState)
                .Where(state => !string.IsNullOrEmpty(state))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.State)
                        .Where(state => !string.IsNullOrEmpty(state))
                )
                .Distinct()
                .CountAsync();

            // Fetch unique countries from both tables
            var uniqueCountries = await _context.BusinessRegisters
                .Select(b => b.businessCountry)
                .Where(country => !string.IsNullOrEmpty(country))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.Country)
                        .Where(country => !string.IsNullOrEmpty(country))
                )
                .Distinct()
                .CountAsync();

            return (uniqueTowns,uniqueCities, uniqueStates, uniqueCountries);
        }

        public async Task<int> GetBusinessRegisterCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.BusinessRegisters.CountAsync();
            return count;
        }

        // Shoppers tab
        public async Task<(IEnumerable<ShopperRegister> records, int totalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.ShopperRegisters.CountAsync();
            var records = await _context.ShopperRegisters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }
        public async Task<bool> UpdateShopperStatusAsync(int shopperId, string newStatus)
        {
            var shopper = await _context.ShopperRegisters.FindAsync(shopperId);
            if (shopper == null)
                return false;

            shopper.status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ShopperRegister?> GetShopperByIdAsync(int shopperId)
        {
            return await _context.ShopperRegisters
                                 .FirstOrDefaultAsync(s => s.ShopperRegId == shopperId);
        }


        public async Task<int> GetShoppersRegisterCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.ShopperRegisters.CountAsync();
            return count;
        }

        public async Task<int> GetCourierserviceCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.CourierService.CountAsync();
            return count;
        }

        public async Task<(IEnumerable<CourierService> records, int totalRecords)> GetCourierRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.CourierService.CountAsync();
            var records = await _context.CourierService
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }


        // landing page
        public async Task<List<LocationStoresDto>> GetLocationsWithCompletedStoresAsync()
        {
            // 1. Get all pending profiles from DB
            var pendingProfiles = await _context.BusinessProfiles
                .Where(bp => bp.profile_status.ToLower() == "incomplete")
                .ToListAsync(); // Materialize here!

            // 2. Group and process in memory
            var result = pendingProfiles
                .GroupBy(bp => bp.business_location.Trim())
                .Where(g => g.Count() >= 3)
                .Select(g =>
                {
                    var parts = g.Key.Split(',').Select(p => p.Trim()).ToArray();

                    var town = parts.Length > 0 ? parts[0] : "";
                    var city = parts.Length > 1 ? parts[1] : "";
                    var country = parts.Length > 3 ? parts[3] : "";

                    // Clean join, skips empty values
                    var locationDisplay = string.Join(", ", new[] { town, city, country }.Where(x => !string.IsNullOrWhiteSpace(x)));

                    return new LocationStoresDto
                    {
                        Location = locationDisplay,
                        Stores = g.ToList()
                    };
                })
                .ToList();

            return result;
        }


    }
}
