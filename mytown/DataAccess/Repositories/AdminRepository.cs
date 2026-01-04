using Dapper;   
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using MyTown.Models;
using System.Data;
using System.Diagnostics;
//using MySql.Data.MySqlClient; 


namespace mytown.DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;
        //private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context, IEmailService emailservice, IConfiguration config,IMemoryCache cache)
        {
            _context = context;
            _emailService = emailservice;
            _connectionString = config.GetConnectionString("mysqlConnection");
            _cache = cache;
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
                            b.BusinessName,
                            b.LicenseType,
                            b.Gstin,
                            b.BusServId,
                            b.BusCatId,
                            b.Town,
                            b.BusMobileNo,
                            b.BusEmail,
                            b.IsEmailVerified,
                            b.Address1,
                            b.Address2,
                            b.BusinessCity,
                            b.BusinessState,
                            b.BusinessCountry,
                            b.PostalCode,
                            b.Password,
                            b.BusinessRegDate,
                            ProfileStatus = bp != null && bp.ProfileStatus != null ? bp.ProfileStatus : "pending",
                            bp.ApprovedDate,
                            ServiceType =
                                b.BusServId == 1 && b.BusCatId == 1 ? "product, service" :
                                b.BusCatId == 1 ? "product" :
                                b.BusServId == 1 ? "service" : "none"
                        };

            // Step 2: apply search if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.BusinessName.ToLower().Contains(search) ||
                    b.BusinessUsername.ToLower().Contains(search) ||
                    b.BusEmail.ToLower().Contains(search) ||
                    b.BusMobileNo.ToLower().Contains(search) ||
                    b.Town.ToLower().Contains(search) ||
                     b.BusinessCity.ToLower().Contains(search) ||
                    b.BusinessState.ToLower().Contains(search) ||
                    b.BusinessCountry.ToLower().Contains(search) ||
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
                        where br.BusCatId == 1 && (
                            (bp != null && bp.ProfileStatus.ToLower() == status.ToLower()) ||
                            (bp == null && status.ToLower() == "incomplete")
                        )
                        select br;

            //  Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(br =>
                    br.BusinessName.ToLower().Contains(search) ||
                    br.BusinessUsername.ToLower().Contains(search) ||
                    br.BusEmail.ToLower().Contains(search) ||
                    br.Town.ToLower().Contains(search) ||
                    br.BusinessCity.ToLower().Contains(search) ||
                    br.BusinessState.ToLower().Contains(search) ||
                    br.BusinessCountry.ToLower().Contains(search));
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
                    bp.ProfileStatus,
                    bp.BusCatId,
                    bp.BusServId
                })
                .ToListAsync();

            // Stores
            var storeCounts = allStatuses.ToDictionary(
                status => status,
                status => businessProfiles.Count(bp =>
                    bp.ProfileStatus.Equals(status, StringComparison.OrdinalIgnoreCase) &&
                    bp.BusCatId == 1
                )
            );

            // Services
            var serviceCounts = allStatuses.ToDictionary(
                status => status,
                status => businessProfiles.Count(bp =>
                    bp.ProfileStatus.Equals(status, StringComparison.OrdinalIgnoreCase) &&
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

            profile.ProfileStatus = status;
            profile.ApprovedDate = status.ToLower() == "approved" ? DateTime.Now : profile.ApprovedDate;

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
                string businessName = business.BusinessName;
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
                            br.BusServId == 1 && // Filter by servicecategory
                            (
                                (bp != null && bp.ProfileStatus.ToLower() == status.ToLower()) ||
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
                .Select(b => b.BusinessCity)
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
                .Select(b => b.BusinessState)
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
                .Select(b => b.BusinessCountry)
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
                .Select(b => b.BusinessCity)
                .Where(city => !string.IsNullOrEmpty(city))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.City)
                        .Where(city => !string.IsNullOrEmpty(city))
                )
                .Distinct()
                .CountAsync();

            // Fetch unique states from both tables
            // Fetch unique states from both tables
            var uniqueStates = await _context.BusinessRegisters
                .Select(b => b.BusinessState)
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
                .Select(b => b.BusinessCountry)
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
        public async Task<(List<ShopperRegister> records, int totalCount)>
      GetShoppersByStatusAsync(string status, int page, int pageSize)
        {
            var query = _context.ShopperRegisters.AsQueryable();

            if (status == "active")
            {
                query = query.Where(s => s.Status == null || s.Status != "Deactivated");
            }
            else if (status == "deactivated")
            {
                query = query.Where(s => s.Status == "Deactivated");
            }

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(s => s.ShopperRegId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalCount);
        }


        //Shopper summary on Admin panel

        public async Task<ShopperStatsDto> GetActiveShopperStatsAsync()
        {
            var activeShoppers = _context.ShopperRegisters
                .Where(s => s.Status == null || s.Status != "Deactivated");

            var result = new ShopperStatsDto
            {
                TotalActiveShoppers = await activeShoppers.CountAsync(),

                TotalTowns = await activeShoppers
                    .Select(s => s.Town)
                    .Distinct()
                    .CountAsync(),

                TotalCities = await activeShoppers
                    .Select(s => s.City)
                    .Distinct()
                    .CountAsync(),

                TotalStates = await activeShoppers
                    .Select(s => s.State)
                    .Distinct()
                    .CountAsync(),

                TotalCountries = await activeShoppers
                    .Select(s => s.Country)
                    .Distinct()
                    .CountAsync()
            };

            return result;
        }

        public async Task<bool> UpdateShopperStatusAsync(int shopperId, string newStatus)
        {
            var shopper = await _context.ShopperRegisters.FindAsync(shopperId);
            if (shopper == null)
                return false;

            shopper.Status = newStatus;
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

        public async Task<bool> DeactivateShopperAsync(int shopperRegId)
        {
            var shopper = await _context.ShopperRegisters
                .FirstOrDefaultAsync(s => s.ShopperRegId == shopperRegId);

            if (shopper == null)
                return false;

            shopper.Status = "Deactivated";

            await _context.SaveChangesAsync();
            return true;
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
                .Where(bp => bp.ProfileStatus.ToLower() == "approved")
                .ToListAsync(); // Materialize here!

            // 2. Group and process in memory
            var result = pendingProfiles
                .GroupBy(bp => bp.BusinessLocation.Trim())
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

        public async Task<List<LocationStoresDto>> GetLocationsWithCompletedStores_EFAsync()
        {
            if (_cache.TryGetValue("dashboardLocations", out List<LocationStoresDto> cachedData))
                return cachedData;

            var sw = Stopwatch.StartNew();

            var profiles = await _context.BusinessProfiles
                .AsNoTracking()
                .Where(bp => bp.ProfileStatus == "approved" && bp.BusinessLocation != null)
                .Select(bp => new
                {
                    bp.BusinessProfileId,
                    bp.BusRegId,
                    bp.BusinessName,
                    bp.BusinessLocation,
                    bp.BannerPath,
                    bp.LogoPath
                })
                .ToListAsync();

            var result = profiles
                .GroupBy(bp => bp.BusinessLocation.Trim())
                .Where(g => g.Count() >= 3)
                .Select(g =>
                {
                    var parts = g.Key.Split(',').Select(p => p.Trim()).ToArray();
                    var town = parts.Length > 0 ? parts[0] : "";
                    var city = parts.Length > 1 ? parts[1] : "";
                    var country = parts.Length > 3 ? parts[3] : "";

                    return new LocationStoresDto
                    {
                        Location = string.Join(", ", new[] { town, city, country }.Where(x => !string.IsNullOrWhiteSpace(x))),
                        Stores = g.Select(x => new BusinessProfile
                        {
                            BusinessProfileId = x.BusinessProfileId,
                            BusRegId = x.BusRegId,
                            BusinessName = x.BusinessName,
                            BusinessLocation = x.BusinessLocation,
                            BannerPath = x.BannerPath,
                            LogoPath = x.LogoPath
                        }).ToList()
                    };
                })
                .ToList();

            sw.Stop();
            Console.WriteLine($"EF execution time (repo): {sw.ElapsedMilliseconds} ms");

            // Cache the result for 5 minutes
            _cache.Set("dashboardLocations", result, TimeSpan.FromMinutes(5));

            return result;
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task TestConnectionAsync()
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync(); // will throw if connection fails
        }


        public async Task<List<LocationStoresDto>> GetLocationsWithCompletedStores_DapperAsync()
        {
            const string cacheKey = "dapper_locations_completed";

            // 1️⃣ Return from cache if exists
            if (_cache.TryGetValue(cacheKey, out List<LocationStoresDto> cached))
                return cached;

            var sql = @"
        SELECT 
            business_profile_id AS BusinessProfileId,
            bus_reg_id AS BusRegId,
            business_name AS BusinessName,
            business_location AS BusinessLocation,
            banner_path AS BannerPath,
            logo_path AS LogoPath
        FROM business_profiles
        WHERE profile_status = 'approved'
          AND business_location IS NOT NULL;
    ";

            using var connection = CreateConnection();

            var rows = (await connection
                .QueryAsync<LocationStoreFlatDto>(sql))
                .ToList();

            var result = rows
                .GroupBy(r => r.BusinessLocation?.Trim())
                .Where(g => g.Count() >= 3)
                .Select(g =>
                {
                    var parts = g.Key?
                        .Split(',')
                        .Select(p => p.Trim())
                        .ToArray() ?? Array.Empty<string>();

                    var town = parts.ElementAtOrDefault(0) ?? "";
                    var city = parts.ElementAtOrDefault(1) ?? "";
                    var country = parts.LastOrDefault() ?? "";

                    return new LocationStoresDto
                    {
                        Location = string.Join(", ",
                            new[] { town, city, country }
                                .Where(x => !string.IsNullOrWhiteSpace(x))),

                        Stores = g.Select(r => new BusinessProfile
                        {
                            BusinessProfileId = r.BusinessProfileId,
                            BusRegId = r.BusRegId,
                            BusinessName = r.BusinessName,
                            BusinessLocation = r.BusinessLocation,
                            BannerPath = r.BannerPath,
                            LogoPath = r.LogoPath
                        }).ToList()
                    };
                })
                .ToList();

            // 2️⃣ Cache result
            _cache.Set(
                cacheKey,
                result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3)
                });

            return result;
        }





    }
}
