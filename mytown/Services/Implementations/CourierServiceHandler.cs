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

        public async Task<CourierService?> RegisterCourierAsync(CourierServiceDto courierDto, bool sendVerification = false)
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

                    // 🔐 Security
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
                CourierServiceName = courierDto.CourierServiceName,
                CourierWebsiteName = courierDto.CourierWebsiteName,
                CourierPhone = courierDto.CourierPhone,
                CourierEmail = courierDto.CourierEmail,

                // 🔐 Security
                Password = hashed,
                IsEmailVerified = true,
                RegisteredDate = DateTime.UtcNow,

                // 🚚 Coverage flags (updated)
                IsCity = courierDto.IsCity,
                IsState = courierDto.IsState,

              
            };


            var created = await _repo.RegisterCourier(courier);
            await _repo.DeletePendingCourierVerification(token);

            return created;
        }

        public async Task<List<CourierBranchCsvRowDto>> ParseAndValidateCsvAsync(IFormFile file)
        {
            return await _repo.ParseAndValidateCsv(file);
        }

        public async Task<bool> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows)
        {
            if (rows == null || rows.Count == 0) return false;

            // double-check validity before saving (repo will save)
            var invalid = rows.Any(r => !r.IsValid);
            if (invalid) return false;

            return await _repo.SaveCourierBranchesAsync(rows);
        }

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

                var allCourierOptions = await _repo.GetBestCourierOptions(
                    store.BusinessCity,
                    store.BusinessState,
                    store.BusinessCountry,
                    shopper.City,
                    totalWeight
                );

                // ✅ Cheapest Surface option
                var cheapestSurface = allCourierOptions
                    .Where(c => c.ShippingMode.Equals("Surface", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(c => c.Cost)
                    .FirstOrDefault();

                // ✅ Fastest Air option
                var fastestAir = allCourierOptions
                    .Where(c => c.ShippingMode.Equals("Air", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(c => c.MaxDeliveryDays) // <-- fastest delivery
                    .FirstOrDefault();

                var selectedCouriers = new List<BestcourierinfoDto>();

                if (cheapestSurface != null)
                    cheapestSurface.ShippingMode = "Standard Delivery";
                selectedCouriers.Add(cheapestSurface);

                if (fastestAir != null)
                    fastestAir.ShippingMode = "Express Delivery";
                selectedCouriers.Add(fastestAir);

                // ✅ HARD-CODED P2P OPTION
                var p2pCost = Math.Max(100, totalWeight * 80); // min ₹100

                selectedCouriers.Add(new BestcourierinfoDto
                {
                    BranchId = 0, // not from courier_branch table
                    ShippingMode = "P2P",
                    Cost = p2pCost,
                    MaxDeliveryDays = 1,
                    DeliveryDaysRange = "Same day / Next day",
                    EstimatedDeliveryDate = DateTime.UtcNow
        .ToString("MMM dd, yyyy", CultureInfo.InvariantCulture)
                });


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
