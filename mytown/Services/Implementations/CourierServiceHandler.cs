using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace mytown.Services.Implementations
{
    public class CourierServiceHandler : ICourierServiceHandler
    {
        private readonly ICourierServiceRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IVerificationLinkBuildercourier _verificationLinkBuilder;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CourierService> _logger;
        
        public CourierServiceHandler(
            ICourierServiceRepository repo,
            IEmailService emailService,
            IVerificationLinkBuildercourier verificationLinkBuilder,
            IConfiguration configuration,
            ILogger<CourierService> logger)
        {
            _repo = repo;
            _emailService = emailService;
            _verificationLinkBuilder = verificationLinkBuilder;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> IsCourierEmailTakenAsync(string email)
        {
            return await _repo.IsCourierEmailTaken(email);
        }

        public async Task<CourierService?> RegisterCourierAsync(CourierServiceDto courierDto, bool sendVerification = true)
        {
            if (courierDto == null) return null;

            // check email
            if (await _repo.IsCourierEmailTaken(courierDto.CourierEmail))
                return null;

            if (sendVerification)
            {
                // create pending verification and send email
                var token = Guid.NewGuid().ToString();
                var expiry = DateTime.UtcNow.AddHours(24);
                var frontendBaseUrl = _configuration["FrontendBaseUrl"] ?? "";
                var link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

                string jsonPayload = JsonSerializer.Serialize(courierDto);

                var pending = new PendingCourierVerification
                {
                    Email = courierDto.CourierEmail,
                    Token = token,
                    ExpiryDate = expiry,
                    JsonPayload = jsonPayload
                };

                await _repo.SavePendingCourierVerification(pending);
                await _emailService.SendVerificationEmail(courierDto.CourierEmail, link);
                return null; // registration deferred until verification
            }
            else
            {
                // register immediately
                var hashed = BCrypt.Net.BCrypt.HashPassword(courierDto.Password);

                var courier = new CourierService
                {
                    CourierServiceName = courierDto.CourierServiceName,
                    CourierWebsiteName = courierDto.CourierWebsiteName,
                    CourierPhone = courierDto.CourierPhone,
                    CourierEmail = courierDto.CourierEmail,

                    //  Address (MISSING BEFORE)
                    Address = courierDto.Address,
                    Town = courierDto.Town,
                    City = courierDto.City,
                    State = courierDto.State,
                    Country = courierDto.Country,
                    PostalCode = courierDto.PostalCode,

                    //  Security
                    Password = hashed,
                    IsEmailVerified = true,
                    RegisteredDate = DateTime.UtcNow,

                    IsCity = courierDto.IsCity,
                    IsState = courierDto.IsState,
                };

                var created = await _repo.RegisterCourier(courier);
                return created;
            }
        }

        public async Task<CourierService?> VerifyCourierEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var pending = await _repo.FindPendingCourierVerificationByToken(token);
            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                return null;

            CourierServiceDto courierDto;
            try
            {
                courierDto = JsonSerializer.Deserialize<CourierServiceDto>(pending.JsonPayload);
                if (courierDto == null) return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize pending courier payload.");
                return null;
            }

            var hashed = BCrypt.Net.BCrypt.HashPassword(courierDto.Password);

            var courier = new CourierService
            {
                //  Basic Info
                CourierServiceName = courierDto.CourierServiceName,
                CourierWebsiteName = courierDto.CourierWebsiteName,
                CourierEmail = courierDto.CourierEmail,
                CourierPhone = courierDto.CourierPhone,

                //  Address
                Address = courierDto.Address,
                Town = courierDto.Town,
                City = courierDto.City,
                State = courierDto.State,
                Country = courierDto.Country,
                PostalCode = courierDto.PostalCode,

                // Coverage
                IsCity = courierDto.IsCity,
                IsState = courierDto.IsState,

                //  Security
                Password = hashed,
                IsEmailVerified = true,
                RegisteredDate = DateTime.UtcNow
            };

            var created = await _repo.RegisterCourier(courier);
            await _repo.DeletePendingCourierVerification(token);

            return created;
        }

        public async Task<PendingCourierVerification?> FindPendingVerificationByEmail(string email)
        {
            var verification = await _repo.FindPendingVerificationByEmail(email);
            return verification;
        }

        public Task SavePendingVerification(PendingCourierVerification pending)
        {
            return _repo.SavePendingVerification(pending);
        }

        public Task RemoveVerification(PendingCourierVerification verification)
        {
            return _repo.RemoveVerification(verification);
        }


        public async Task<List<CourierBranchCsvRowDto>> ParseAndValidateCsvAsync(IFormFile file)
        {
            return await _repo.ParseAndValidateCsv(file);
        }

        public async Task<string> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows)
        {
            if (rows == null || rows.Count == 0)
                throw new Exception("No rows received.");

            var invalid = rows.Any(r => !r.IsValid);
            if (invalid)
                throw new Exception("Some rows are invalid. Please fix them before saving.");

            var result =  await _repo.SaveCourierBranchesAsync(rows);

            // Get unique emails from rows
            var emails = rows
                .Select(r => r.BranchEmailId)
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .ToList();

            // Send login email to each branch
            foreach (var email in emails)
            {
                await _emailService.SendBranchLoginEmailAsync(email, "Branch@123");
            }

            return result;
        }

// ============================================================
// REPLACE the entire GetBestCourierOptionsByStoresAsync method
// in CourierServiceHandler.cs with this
// ============================================================

public async Task<List<StoreCourierResultDto>> GetBestCourierOptionsByStoresAsync(
    int shopperId,
    List<int> storeIds)
{
    var shopper = await _repo.GetShopperByIdAsync(shopperId)
        ?? throw new Exception("Shopper not found");

    var stores = await _repo.GetStoresByIdsAsync(storeIds);
    var storeWeights = await _repo.GetStoreWeightsAsync(shopperId, storeIds);

    var results = new List<StoreCourierResultDto>();

    foreach (var storeId in storeIds)
    {
        if (!stores.TryGetValue(storeId, out var store))
            continue;

        var totalWeight = storeWeights.TryGetValue(storeId, out var weight)
            ? weight
            : 0;

        // ─── Standard + Express courier options ───────────────────────────
        var allCourierOptions = await _repo.GetBestCourierOptions(
            store.BusinessCity,
            store.BusinessState,
            store.BusinessCountry,
            shopper.City,
            totalWeight
        );

        // Cheapest Surface → Standard Delivery
        var cheapestSurface = allCourierOptions
            .Where(c => c.ShippingMode.Equals("Surface", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Cost)
            .FirstOrDefault();

        // Fastest Air → Express Delivery
        var fastestAir = allCourierOptions
            .Where(c => c.ShippingMode.Equals("Air", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.MaxDeliveryDays)
            .FirstOrDefault();

        var selectedCouriers = new List<BestcourierinfoDto>();

        if (cheapestSurface != null)
        {
            cheapestSurface.ShippingMode = "Standard Delivery";
            selectedCouriers.Add(cheapestSurface);
        }

        if (fastestAir != null)
        {
            fastestAir.ShippingMode = "Express Delivery";
            selectedCouriers.Add(fastestAir);
        }

        // ─── P2P option — find real matching transporter ──────────────────
        // storeCity = store.BusinessCity  (transporter picks up FROM here)
        // shopperCity = shopper.City      (transporter drops off TO here)
        var matchingTransporter = await _repo.FindMatchingTransporterAsync(
            store.Town,
            store.BusinessCity,
            store.BusinessState,
            store.BusinessCountry, shopper.City,shopper.Town,shopper.State, shopper.Country,
            
            totalWeight
        );

        if (matchingTransporter != null)
        {
            // P2P cost = 30% of Standard Delivery price
            // If no standard option exists, fall back to 30% of Express, else ₹100 minimum
            decimal basePrice = cheapestSurface?.Cost
                ?? fastestAir?.Cost
                ?? 333m; // fallback so 30% = ~₹100

            decimal p2pCost = Math.Round(basePrice * 0.30m, 2);
            p2pCost = Math.Max(p2pCost, 50m); // minimum ₹50

            matchingTransporter.Cost = p2pCost;
            matchingTransporter.ShippingMode = "P2P";

            selectedCouriers.Add(matchingTransporter);
        }
        // If no transporter matches → P2P option is simply NOT shown to shopper
        // (no fallback dummy P2P entry)

        results.Add(new StoreCourierResultDto
        {
            StoreId = storeId,
            TotalWeightKg = totalWeight,
            CourierOptions = selectedCouriers
        });
    }

    return results;
}



        //public async Task<List<AssignedOrderDto>> GetAssignedOrdersByCourierIdAsync(int courierId)
        //{
        //    return await _repo.GetAssignedOrdersByCourierIdAsync(courierId);
        //}
    }
}
