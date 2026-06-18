using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;

namespace mytown.DataAccess.Repositories
{
    public class TransporterDashboardRepository : ITransporterDashboardRepository
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;

        public TransporterDashboardRepository(AppDbContext context, IOrderRepository orderRepo, IEmailService emailService)
        {
            _context = context;
            _orderRepository = orderRepo;
            _emailService = emailService;
        }

        // -------------------------------------------------------------------------
        // DASHBOARD SUMMARY
        // -------------------------------------------------------------------------
        public async Task<TransporterDashboardDto> GetDashboardSummaryAsync(int transporterRegId)
        {
            var transporter = await _context.TransporterRegisters
                .FirstOrDefaultAsync(t => t.TransporterRegId == transporterRegId);

            if (transporter == null)
                throw new Exception("Transporter not found.");

            var activeStatuses = new List<string>
    {
        "Assigned",
        "ReachedPickup",
        "PickedUp",
        "InTransit"
    };

            // ==========================
            // SHOPPER DELIVERIES
            // ==========================

            var shopperDeliveredCount = await _context.TransporterDeliveryRequests
                .Where(d => d.TransporterRegId == transporterRegId
                         && d.DeliveryStatus == "Delivered")
                .CountAsync();

            var shopperActiveCount = await _context.TransporterDeliveryRequests
                .Where(d => d.TransporterRegId == transporterRegId
                         && activeStatuses.Contains(d.DeliveryStatus))
                .CountAsync();

            var shopperEarned = await _context.TransporterDeliveryRequests
                .Where(d => d.TransporterRegId == transporterRegId
                         && d.DeliveryStatus == "Delivered")
                .SumAsync(d => (decimal?)d.DeliveryFee) ?? 0;

            // ==========================
            // SENDER DELIVERIES
            // ==========================

            var senderDeliveredCount = await _context.SenderOrders
                .Where(s => s.TransporterRegId == transporterRegId
                         && s.DeliveryStatus == "Delivered")
                .CountAsync();

            var senderActiveCount = await _context.SenderOrders
                .Where(s => s.TransporterRegId == transporterRegId
                         && activeStatuses.Contains(s.DeliveryStatus))
                .CountAsync();

            var senderEarned = senderDeliveredCount * 50;

            // ==========================
            // TOTALS
            // ==========================

            var totalDeliveries = shopperDeliveredCount + senderDeliveredCount;

            var activeDeliveries = shopperActiveCount + senderActiveCount;

            var totalEarned = shopperEarned + senderEarned;

            var kyc = await _context.TransporterKYCs
                .Where(k => k.TransporterRegId == transporterRegId)
                .OrderByDescending(k => k.SubmittedAt)
                .FirstOrDefaultAsync();

            var bank = await _context.TransporterBankDetails
                .FirstOrDefaultAsync(b => b.TransporterRegId == transporterRegId);

            var hasActivePlan = await _context.TransporterTravelPlans
                .AnyAsync(p => p.TransporterRegId == transporterRegId && p.IsActive);

            return new TransporterDashboardDto
            {
                TransporterRegId = transporterRegId,
                TransporterName = transporter.TransporterName,
                TotalDeliveries = totalDeliveries,
                ActiveDeliveries = activeDeliveries,
                TotalEarned = totalEarned,
                KycStatus = kyc?.KycStatus ?? "NotSubmitted",
                BankVerified = bank?.IsVerified ?? false,
                HasActivePlan = hasActivePlan
            };
        }
        // -------------------------------------------------------------------------
        // TRAVEL PLANS
        // -------------------------------------------------------------------------
        public async Task<TravelPlanDto?> GetActivePlanAsync(int transporterRegId)
        {
            return await _context.TransporterTravelPlans
                .Where(p => p.TransporterRegId == transporterRegId && p.IsActive)
                .Select(p => MapPlanToDto(p))
                .FirstOrDefaultAsync();
        }

        public async Task<List<TravelPlanDto>> GetAllPlansAsync(int transporterRegId)
        {
            return await _context.TransporterTravelPlans
                .Where(p => p.TransporterRegId == transporterRegId)
                .OrderByDescending(p => p.PlanId)
                .Select(p => MapPlanToDto(p))
                .ToListAsync();
        }

private static TravelPlanDto MapPlanToDto(TransporterTravelPlan p)
{
    bool effectivelyActive = p.IsActive && p.ArrivalDate.Date >= DateTime.UtcNow.Date;

    return new TravelPlanDto
    {
        PlanId              = p.PlanId,
        TransporterRegId    = p.TransporterRegId,
        IsActive            = effectivelyActive,
        PlanStatus          = effectivelyActive ? "Available" : "Inactive",
        // =========================================================
        // START LOCATION
        // =========================================================

        StartTown = p.StartTown,
        StartCity = p.StartCity,
        StartState = p.StartState,
        StartCountry = p.StartCountry,

        // =========================================================
        // DESTINATION LOCATION
        // =========================================================

        DestinationTown = p.DestinationTown,
        DestinationCity = p.DestinationCity,
        DestinationState = p.DestinationState,
        DestinationCountry = p.DestinationCountry,

        PreferredRoute = p.PreferredRoute,
        DistanceKm          = p.DistanceKm,
        StartDate           = p.StartDate,
        ArrivalDate         = p.ArrivalDate,
        VehicleType         = p.VehicleType,
        VehicleRegistration = p.VehicleRegistration,
        VehicleName         = p.VehicleName,
        MaxWeightKg         = p.MaxWeightKg,
        PackageSizeL        = p.PackageSizeL,
        PackageSizeW        = p.PackageSizeW,
        PackageSizeH        = p.PackageSizeH,
        NumberOfPackages    = p.NumberOfPackages,
        AcceptsFragile      = p.AcceptsFragile,
        AcceptsPerishable   = p.AcceptsPerishable,
        PreferredContact    = p.PreferredContact,
        LanguagePreference  = p.LanguagePreference,
        NotifyNewOrders     = p.NotifyNewOrders,
        NotifyPayments      = p.NotifyPayments,
    };
}

public async Task<TravelPlanDto> SaveTravelPlanAsync(TravelPlanDto dto)
{
    // ── Validation 1: Arrival must be after Start ─────────────────────
    if (dto.ArrivalDate <= dto.StartDate)
        throw new Exception("Arrival date/time must be after start date/time.");

    // ── Validation 2: No duplicate active plan on same date range ─────
    // Transporter cannot have 2 active plans whose date ranges overlap.
    // Two plans overlap if: existingStart < newArrival AND existingArrival > newStart
    var overlapping = await _context.TransporterTravelPlans
        .Where(p =>
            p.TransporterRegId == dto.TransporterRegId &&
            p.IsActive &&
            p.StartDate < dto.ArrivalDate &&
            p.ArrivalDate > dto.StartDate)
        .AnyAsync();

    if (overlapping)
        throw new Exception(
            "You already have an active plan overlapping these dates. " +
            "Please deactivate it from My Plans before creating a new one.");

    // ── Always create NEW plan — never update existing ────────────────
    var plan = new TransporterTravelPlan
    {
        TransporterRegId    = dto.TransporterRegId,
        IsActive            = true,
        // =========================================================
        // START LOCATION
        // =========================================================

        StartTown = dto.StartTown,
        StartCity = dto.StartCity,
        StartState = dto.StartState,
        StartCountry = dto.StartCountry,

        // =========================================================
        // DESTINATION LOCATION
        // =========================================================

        DestinationTown = dto.DestinationTown,
        DestinationCity = dto.DestinationCity,
        DestinationState = dto.DestinationState,
        DestinationCountry = dto.DestinationCountry,
        PreferredRoute      = dto.PreferredRoute,
        DistanceKm          = dto.DistanceKm,
        StartDate           = dto.StartDate,
        ArrivalDate         = dto.ArrivalDate,
        VehicleType         = dto.VehicleType,
        VehicleRegistration = dto.VehicleRegistration,
        VehicleName         = dto.VehicleName,
        MaxWeightKg         = dto.MaxWeightKg,
        PackageSizeL        = dto.PackageSizeL,
        PackageSizeW        = dto.PackageSizeW,
        PackageSizeH        = dto.PackageSizeH,
        NumberOfPackages    = dto.NumberOfPackages,
        AcceptsFragile      = dto.AcceptsFragile,
        AcceptsPerishable   = dto.AcceptsPerishable,
        PreferredContact    = dto.PreferredContact,
        LanguagePreference  = dto.LanguagePreference,
        NotifyNewOrders     = dto.NotifyNewOrders,
        NotifyPayments      = dto.NotifyPayments,
    };

    _context.TransporterTravelPlans.Add(plan);
    await _context.SaveChangesAsync();

    dto.PlanId = plan.PlanId;
    return dto;
}

        public async Task<bool> DeactivatePlanAsync(int planId, int transporterRegId)
        {
            var plan = await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(p => p.PlanId == planId
                                       && p.TransporterRegId == transporterRegId);
            if (plan == null) return false;

            plan.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------------------------------------------------------------
        // SEARCH AVAILABLE TRANSPORTERS (for shoppers)
        // -------------------------------------------------------------------------
        public async Task<List<AvailableTransporterDto>> SearchAvailableTransportersAsync(
       string startTown,
       string startCity,
       string startState,
       string startCountry,
       string destinationTown,
       string destinationCity,
       string destinationState,
       string destinationCountry)
        {
            DateTime bookingDateTime =
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    DateTime.UtcNow,
                    "India Standard Time");

            return await _context.TransporterTravelPlans

                .Include(p => p.TransporterRegister)

                .Where(p =>

                    // Active plans only
                    p.IsActive &&

                    // Only future transporter plans
                    p.StartDate > bookingDateTime &&

                    // Exact pickup location match
                    p.StartTown.ToLower() == startTown.ToLower() &&
                    p.StartCity.ToLower() == startCity.ToLower() &&
                    p.StartState.ToLower() == startState.ToLower() &&
                    p.StartCountry.ToLower() == startCountry.ToLower() &&

                    // Exact destination match
                    p.DestinationTown.ToLower() == destinationTown.ToLower() &&
                    p.DestinationCity.ToLower() == destinationCity.ToLower() &&
                    p.DestinationState.ToLower() == destinationState.ToLower() &&
                    p.DestinationCountry.ToLower() == destinationCountry.ToLower()
                )

                // Oldest created matching plan gets priority
                .OrderBy(p => p.CreatedAt)

                .Select(p => new AvailableTransporterDto
                {
                    PlanId = p.PlanId,
                    TransporterRegId = p.TransporterRegId,
                    TransporterName = p.TransporterRegister.TransporterName,

                    VehicleType = p.VehicleType,
                    VehicleName = p.VehicleName,

                    StartTown = p.StartTown,
                    StartCity = p.StartCity,
                    StartState = p.StartState,
                    StartCountry = p.StartCountry,

                    DestinationTown = p.DestinationTown,
                    DestinationCity = p.DestinationCity,
                    DestinationState = p.DestinationState,
                    DestinationCountry = p.DestinationCountry,

                    StartDate = p.StartDate,
                    ArrivalDate = p.ArrivalDate,

                    MaxWeightKg = p.MaxWeightKg,
                    NumberOfPackages = p.NumberOfPackages,

                    AcceptsFragile = p.AcceptsFragile,
                    AcceptsPerishable = p.AcceptsPerishable,

                    PreferredContact = p.PreferredContact
                })

                .ToListAsync();
        }

        // -------------------------------------------------------------------------
        // CREATE DELIVERY REQUEST — AUTO-ASSIGNED (no Accept step)
        // -------------------------------------------------------------------------
        public async Task<TransporterDeliveryRequest> CreateDeliveryRequestAsync(ShopperDeliveryRequestDto dto)
        {
            var plan = await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(p => p.PlanId == dto.PlanId && p.IsActive);

            if (plan == null)
                throw new Exception("Travel plan not found or is no longer active.");

            var existing = await _context.TransporterDeliveryRequests
                .AnyAsync(d =>
                    d.PlanId == dto.PlanId &&
                    d.ShopperRegId == dto.ShopperRegId &&
                    d.OrderId == dto.OrderId &&
                    d.StoreOrderId == dto.StoreOrderId);

            if (existing)
                throw new Exception("A delivery request already exists for this order and plan.");

            string deliveryCode = "DEL-" + new Random().Next(1000, 9999).ToString();

            var request = new TransporterDeliveryRequest
            {
                PlanId = dto.PlanId,
                TransporterRegId = plan.TransporterRegId,   // auto-assign from plan
                ShopperRegId = dto.ShopperRegId,
                GuestRegId = dto.GuestRegId,
                IsGuestOrder = dto.GuestRegId.HasValue,
                OrderId = dto.OrderId,
                StoreOrderId = dto.StoreOrderId,
                PickupLocation = dto.PickupLocation,
                DropoffLocation = dto.DropoffLocation,
                PackageWeightKg = dto.PackageWeightKg,
                NumberOfPackages = dto.NumberOfPackages,
                PackageTags = dto.PackageTags ?? "NA",
                DeliveryStatus = "Assigned",                // straight to Assigned
                DeliveryCode = deliveryCode,
                CreatedAt = DateTime.UtcNow,
                AssignedAt = DateTime.UtcNow,
                DeliveryProofFile = ""
            };

            _context.TransporterDeliveryRequests.Add(request);

            // Notify transporter
            _context.TransporterDBNotifications.Add(new TransporterDBNotifications
            {
                TransporterRegId = plan.TransporterRegId,
                Title = "New Delivery Assigned",
                Message = $"A new delivery ({deliveryCode}) has been assigned to you from {dto.PickupLocation} to {dto.DropoffLocation}.",
                IsRead = false,
                CreatedDate = DateTime.UtcNow          // ✅ CreatedDate not CreatedAt
            });

            await _context.SaveChangesAsync();
            return request;
        }

        // -------------------------------------------------------------------------
        // ACTIVE DELIVERIES — Assigned + ReachedPickup + PickedUp + InTransit
        // -------------------------------------------------------------------------
        public async Task<List<ActiveDeliveryDto>> GetActiveDeliveryAsync(int transporterRegId)
        {
            var activeStatuses = new List<string> { "Assigned", "ReachedPickup", "PickedUp", "InTransit" };

            var result = await (
                from d in _context.TransporterDeliveryRequests
                join p in _context.ShippingPackageDetails
                    on d.StoreOrderId equals p.StoreOrderId into packageGroup
                from p in packageGroup.DefaultIfEmpty()

                where d.TransporterRegId == transporterRegId
                   && activeStatuses.Contains(d.DeliveryStatus)

                orderby d.CreatedAt descending

                select new ActiveDeliveryDto
                {
                    DeliveryReqId = d.DeliveryReqId,
                    PlanId = d.PlanId,
                    StoreOrderId     = d.StoreOrderId,   // ← ADD THIS
                    OrderId          = d.OrderId,
                    DeliveryCode = d.DeliveryCode,
                    CustomerName = d.ShopperRegister.Username,
                    PickupLocation = d.PickupLocation,
                    DropoffLocation = d.DropoffLocation,
                    NumberOfPackages = d.NumberOfPackages,
                    PackageWeightKg = d.PackageWeightKg,

                    // ✅ From ShippingPackageDetails
                    PackageLengthCm = p != null ? p.PackageLength : null,
                    PackageWidthCm  = p != null ? p.PackageWidth  : null,
                    PackageHeightCm = p != null ? p.PackageHeight : null,

                    DeliveryFee = d.DeliveryFee,
                    PackageTags = d.PackageTags,
                    //DeliveryStatus = d.DeliveryStatus,
                    DeliveryStatus =
                    d.DeliveryStatus != "Delivered" &&
                    d.TravelPlan.ArrivalDate.Date < DateTime.UtcNow.Date
                        ? "Incomplete"
                        : d.DeliveryStatus,
                    AcceptedAt = d.AssignedAt,
                    EtaInfo = d.TravelPlan.ArrivalDate.ToString("dd MMM yyyy")
                }
            ).ToListAsync();

            return result;
        }

        // -------------------------------------------------------------------------
        // UPDATE DELIVERY STATUS — Assigned → ReachedPickup → PickedUp → InTransit → Delivered
        // -------------------------------------------------------------------------
        public async Task<bool> UpdateDeliveryStatusAsync(UpdateDeliveryStatusDto dto)
        {
            var allowedStatuses = new List<string> { "ReachedPickup", "PickedUp", "InTransit", "Delivered" };

            if (!allowedStatuses.Contains(dto.NewStatus))
                return false;

            var delivery = await _context.TransporterDeliveryRequests
                .FirstOrDefaultAsync(d =>
                    d.DeliveryReqId == dto.DeliveryReqId &&
                    d.TransporterRegId == dto.TransporterRegId);

            if (delivery == null) return false;

            // Enforce forward-only transitions
            var validTransitions = new Dictionary<string, string>
            {
                { "Assigned",      "ReachedPickup" },
                { "ReachedPickup", "PickedUp"       },
                { "PickedUp",      "InTransit"      },
                { "InTransit",     "Delivered"      }
            };

            if (!validTransitions.TryGetValue(delivery.DeliveryStatus, out var expected)
                || expected != dto.NewStatus)
                return false;

            delivery.DeliveryStatus = dto.NewStatus;

            // Stamp the matching timestamp column
            switch (dto.NewStatus)
            {
                case "ReachedPickup": delivery.ReachedPickupAt = DateTime.UtcNow; break;
                case "PickedUp":      delivery.PickedUpAt      = DateTime.UtcNow; break;
                case "InTransit":     delivery.InTransitAt     = DateTime.UtcNow; break;
                case "Delivered":     delivery.DeliveredAt     = DateTime.UtcNow; break;
            }

            // ── SYNC ShippingDetails so shopper sees updated status ──
            if (delivery.StoreOrderId.HasValue)
            {
                var shipping = await _context.ShippingDetails
                    .FirstOrDefaultAsync(s => s.StoreOrderId == delivery.StoreOrderId.Value);

                if (shipping != null)
                {
                    shipping.ShippingStatus = dto.NewStatus switch
                    {
                        "ReachedPickup" => "ReachedPickup",
                        "PickedUp" => "PickedUp",
                        "InTransit" => "InTransit",
                        "Delivered" => "Delivered",
                        _ => shipping.ShippingStatus
                    };

                    // Generate tracking ID when transporter picks up the parcel
                    if (dto.NewStatus == "PickedUp" &&
                        string.IsNullOrEmpty(shipping.TrackingId))
                    {
                        shipping.TrackingId = GenerateTrackingId(shipping.StoreOrderId);
                    }

                    // Mark delivery completion date
                    if (dto.NewStatus == "Delivered")
                    {
                        shipping.DeliveredDate = DateTime.UtcNow;
                    }
                }
            }

            // ── NOTIFY SHOPPER ONLY FOR REGISTERED SHOPPERS , Not for guest ──
            if (!delivery.IsGuestOrder && delivery.ShopperRegId.HasValue)
            {
                _context.ShopperDBNotifications.Add(new ShopperDBNotifications
                {
                    ShopperRegId = delivery.ShopperRegId.Value,
                    Title = "Delivery Update",
                    Message = $"Your delivery ({delivery.DeliveryCode}) status is now: {dto.NewStatus}.",
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow
                });
            }

            // ── NOTIFY TRANSPORTER ──
            _context.TransporterDBNotifications.Add(new TransporterDBNotifications
            {
                TransporterRegId = delivery.TransporterRegId,
                Title = "Delivery Status Updated",
                Message = $"Delivery {delivery.DeliveryCode} status changed to {dto.NewStatus}.",
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // send tracking id email to guests
            if (dto.NewStatus == "PickedUp" &&
              delivery.OrderId.HasValue)
            {
                var orderConfirmation =
                    await _orderRepository.GetOrderConfirmationAsync(delivery.OrderId.Value);

                if (orderConfirmation != null &&
                    orderConfirmation.IsGuestOrder &&
                    !string.IsNullOrEmpty(orderConfirmation.ShopperEmail))
                {
                    await _emailService.SendGuestNotificationforTracking(
                        orderConfirmation.ShopperEmail,
                        orderConfirmation.ShopperName,
                        orderConfirmation
                    );
                }
            }
            return true;
        }

        //Generate tracking id based on storeorderid
        private string GenerateTrackingId(int storeOrderId)
        {
            return $"TRK-{storeOrderId}-{DateTime.UtcNow:yyyyMMddHHmm}";
        }

        // -------------------------------------------------------------------------
        // COMPLETED DELIVERIES
        // -------------------------------------------------------------------------
        public async Task<List<ActiveDeliveryDto>> GetCompletedDeliveriesAsync(int transporterRegId)
        {
            return await _context.TransporterDeliveryRequests
                .Where(d => d.TransporterRegId == transporterRegId
                         && d.DeliveryStatus == "Delivered")
                .OrderByDescending(d => d.DeliveredAt)
                .Select(d => new ActiveDeliveryDto
                {
                    DeliveryReqId = d.DeliveryReqId,
                    PlanId = d.PlanId,
                    StoreOrderId     = d.StoreOrderId,   // ← ADD THIS
                    OrderId          = d.OrderId,        // ← ADD THIS
                    DeliveryCode = d.DeliveryCode,
                    CustomerName = d.ShopperRegister.Username,
                    PickupLocation = d.PickupLocation,
                    DropoffLocation = d.DropoffLocation,
                    NumberOfPackages = d.NumberOfPackages,
                    PackageWeightKg = d.PackageWeightKg,
                    DeliveryFee = d.DeliveryFee,
                    PackageTags = d.PackageTags,
                    DeliveryStatus = d.DeliveryStatus,
                    AcceptedAt = d.AssignedAt,
                    EtaInfo = d.DeliveredAt.HasValue
                        ? d.DeliveredAt.Value.ToString("dd MMM yyyy")
                        : ""
                })
                .ToListAsync();
        }

        // -------------------------------------------------------------------------
        // EXCEPTION REPORTS
        // -------------------------------------------------------------------------
        public async Task<bool> SubmitExceptionReportAsync(ExceptionReportDto dto)
        {
            var report = new TransporterExceptionReport
            {
                DeliveryReqId = dto.DeliveryReqId,
                TransporterRegId = dto.TransporterRegId,
                ExceptionType = dto.ExceptionType,
                Description = dto.Description,
                ReportedAt = DateTime.UtcNow
            };

            _context.TransporterExceptionReports.Add(report);
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------------------------------------------------------------
        // KYC
        // -------------------------------------------------------------------------
        public async Task<TransporterKYC?> GetKycAsync(int transporterRegId)
        {
            return await _context.TransporterKYCs
                .Where(k => k.TransporterRegId == transporterRegId)
                .OrderByDescending(k => k.SubmittedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<TransporterKYC> SubmitKycAsync(
            int transporterRegId, string docType, string docNumber, string fileName)
        {
            var existing = await _context.TransporterKYCs
                .FirstOrDefaultAsync(k => k.TransporterRegId == transporterRegId);

            if (existing != null)
            {
                existing.DocumentType = docType;
                existing.DocumentNumber = docNumber;
                existing.DocumentFileName = fileName;
                existing.KycStatus = "Pending";         // ✅ KycStatus not Status
                existing.SubmittedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existing;
            }

            var kyc = new TransporterKYC
            {
                TransporterRegId = transporterRegId,
                DocumentType = docType,
                DocumentNumber = docNumber,
                DocumentFileName = fileName,
                KycStatus = "Pending",                  // ✅ KycStatus not Status
                SubmittedAt = DateTime.UtcNow
            };

            _context.TransporterKYCs.Add(kyc);
            await _context.SaveChangesAsync();
            return kyc;
        }

        // -------------------------------------------------------------------------
        // BANK DETAILS
        // -------------------------------------------------------------------------
        public async Task<TransporterBankDetails?> GetBankDetailsAsync(int transporterRegId)
        {
            return await _context.TransporterBankDetails
                .FirstOrDefaultAsync(b => b.TransporterRegId == transporterRegId);
        }

        public async Task<TransporterBankDetails> SubmitBankDetailsAsync(TransporterBankDto dto)
        {
            var existing = await _context.TransporterBankDetails
                .FirstOrDefaultAsync(b => b.TransporterRegId == dto.TransporterRegId);

            if (existing != null)
            {
                existing.BankName = dto.BankName;
                existing.AccountNumber = dto.AccountNumber;
                existing.BranchName = dto.BranchName;
                existing.IfscCode = dto.IfscCode;
                existing.IsVerified = false;
                await _context.SaveChangesAsync();
                return existing;
            }

            var bank = new TransporterBankDetails
            {
                TransporterRegId = dto.TransporterRegId,
                BankName = dto.BankName,
                AccountNumber = dto.AccountNumber,
                BranchName = dto.BranchName,
                IfscCode = dto.IfscCode,
                IsVerified = false
            };

            _context.TransporterBankDetails.Add(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        // -------------------------------------------------------------------------
        // PROFILE — queries KYC and Bank separately (no nav props on TransporterRegister)
        // -------------------------------------------------------------------------
        public async Task<TransporterProfileDto?> GetProfileAsync(int transporterRegId)
        {
            var t = await _context.TransporterRegisters
                .FirstOrDefaultAsync(x => x.TransporterRegId == transporterRegId);

            if (t == null) return null;

            // Query KYC and Bank separately — TransporterRegister has no nav props for them
            var kyc = await _context.TransporterKYCs
                .Where(k => k.TransporterRegId == transporterRegId)
                .OrderByDescending(k => k.SubmittedAt)
                .FirstOrDefaultAsync();

            var bank = await _context.TransporterBankDetails
                .FirstOrDefaultAsync(b => b.TransporterRegId == transporterRegId);

            return new TransporterProfileDto
            {
                TransporterRegId   = t.TransporterRegId,
                TransporterName    = t.TransporterName,
                Email              = t.Email,
                PhoneNumber        = t.PhoneNumber,
                Address            = t.Address,
                Town               = t.Town,
                City               = t.City,
                State              = t.State,
                Country            = t.Country,
                PostalCode         = t.PostalCode,
                Status             = t.Status,
                IsEmailVerified    = t.IsEmailVerified,
                KycStatus          = kyc?.KycStatus ?? "NotSubmitted",  // ✅ KycStatus not Status
                BankVerified       = bank?.IsVerified ?? false,
                TransporterRegDate = t.TransporeterRegDate              // ✅ matches model typo
            };
        }

        public async Task<bool> UpdateProfileAsync(UpdateTransporterProfileDto dto)
        {
            var transporter = await _context.TransporterRegisters
                .FirstOrDefaultAsync(t => t.TransporterRegId == dto.TransporterRegId);

            if (transporter == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.TransporterName))
                transporter.TransporterName = dto.TransporterName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                transporter.PhoneNumber = dto.PhoneNumber;

            if (dto.Address != null)    transporter.Address    = dto.Address;
            if (dto.Town != null)       transporter.Town       = dto.Town;
            if (dto.City != null)       transporter.City       = dto.City;
            if (dto.State != null)      transporter.State      = dto.State;
            if (dto.Country != null)    transporter.Country    = dto.Country;
            if (dto.PostalCode != null) transporter.PostalCode = dto.PostalCode;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePasswordAsync(int transporterRegId, string newHashedPassword)
        {
            var transporter = await _context.TransporterRegisters
                .FirstOrDefaultAsync(t => t.TransporterRegId == transporterRegId);

            if (transporter == null) return false;

            transporter.Password = newHashedPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------------------------------------------------------------
        // NOTIFICATIONS
        // -------------------------------------------------------------------------
        public async Task<List<TransporterDBNotifications>> GetUnreadNotificationsAsync(int transporterId)
        {
            return await _context.TransporterDBNotifications
                .Where(n => n.TransporterRegId == transporterId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)          // ✅ CreatedDate not CreatedAt
                .ToListAsync();
        }

        public async Task MarkAllAsReadAsync(int transporterId)
        {
            var notifications = await _context.TransporterDBNotifications
                .Where(n => n.TransporterRegId == transporterId && !n.IsRead)
                .ToListAsync();

            notifications.ForEach(n => n.IsRead = true);
            await _context.SaveChangesAsync();
        }

        public async Task MarkEachNotificationReadAsync(int notificationId)
        {
            var notification = await _context.TransporterDBNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        // mark delivered status
        public async Task<string> MarkAsDeliveredAsync(int storeOrderId)
        {
            var shipping = await _context.ShippingDetails
                .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);

            if (shipping == null)
                throw new Exception("Shipping record not found");

            // No file upload

            // Update status
            shipping.ShippingStatus = "Delivered";
            shipping.DeliveredDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return "Delivery marked as completed";
        }

        // sender orders Repository
        public async Task<List<SenderOrder>> GetTransporterDeliversSendersOrdersAsync(int transporterRegId)
        {
            return await _context.SenderOrders
                .Where(x => x.TransporterRegId == transporterRegId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        //update sender orders status to delivered
        // Repository Interface

        // Repository
        public async Task<bool> UpdateTransporterDeliveryStatusAsync(
     int senderOrderId,
     int transporterRegId,
     string deliveryStatus)
        {
            var order = await _context.SenderOrders
                .FirstOrDefaultAsync(x =>
                    x.SenderOrderId == senderOrderId &&
                    x.TransporterRegId == transporterRegId);

            if (order == null)
                return false;

            // Prevent duplicate update
            if (order.DeliveryStatus == deliveryStatus)
                throw new Exception("Status already updated");

            // Allow only after pickup date & time
            var pickupTime = DateTime.Parse(order.PickupTime).TimeOfDay;

            var pickupDateTime = order.PickupDate.Date.Add(pickupTime);

            var currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            if (currentDateTime < pickupDateTime)
            {
                throw new Exception(
                    $"You can update delivery status only after pickup time ({pickupDateTime:dd-MMM-yyyy hh:mm tt})");
            }

            order.DeliveryStatus = deliveryStatus;

            // Sender Notification
            _context.SenderDBNotifications.Add(
                new SenderDBNotifications
                {
                    SenderRegId = order.SenderRegId,

                    Title = "Shipment Status Updated",

                    Message =
                        $"Your shipment #{order.SenderOrderId} is now {deliveryStatus}.",

                    IsRead = false,

                    CreatedDate = DateTime.UtcNow
                });

            // Transporter Notification
            _context.TransporterDBNotifications.Add(
                new TransporterDBNotifications
                {
                    TransporterRegId = transporterRegId,

                    Title = "Shipment Status Updated",

                    Message =
                        $"Shipment #{order.SenderOrderId} status updated to {deliveryStatus}.",

                    IsRead = false,

                    CreatedDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task AddSenderNotificationAsync(
   SenderDBNotifications notification)
        {
            await _context
                .SenderDBNotifications
                .AddAsync(notification);
        }

        public async Task AddTransporterNotificationAsync(
    TransporterDBNotifications notification)
        {
            await _context
                .TransporterDBNotifications
                .AddAsync(notification);
        }
    }
}