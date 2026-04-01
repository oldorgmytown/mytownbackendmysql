using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class TransporterDashboardRepository : ITransporterDashboardRepository
    {
        private readonly AppDbContext _context;

        public TransporterDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        // ========== DASHBOARD SUMMARY ==========
        public async Task<TransporterDashboardDto> GetDashboardSummaryAsync(int transporterRegId)
        {
            var transporter = await _context.TransporterRegisters
                .FirstOrDefaultAsync(t => t.TransporterRegId == transporterRegId);

            if (transporter == null) return null;

            var totalDeliveries = await _context.TransporterDeliveryRequests
                .Where(d => d.TransporterRegId == transporterRegId && d.DeliveryStatus == "Delivered")
                .CountAsync();

            var activeDeliveries = await _context.TransporterDeliveryRequests
                .Where(d => d.TransporterRegId == transporterRegId
                    && d.DeliveryStatus != "Delivered"
                    && d.DeliveryStatus != "Pending")
                .CountAsync();

            var totalEarned = await _context.TransporterDeliveryRequests
                .Where(d => d.TransporterRegId == transporterRegId && d.DeliveryStatus == "Delivered")
                .SumAsync(d => d.DeliveryFee);

            var kyc = await _context.TransporterKYCs
                .FirstOrDefaultAsync(k => k.TransporterRegId == transporterRegId);

            var bank = await _context.TransporterBankDetails
                .FirstOrDefaultAsync(b => b.TransporterRegId == transporterRegId);

            var activePlan = await _context.TransporterTravelPlans
                .AnyAsync(p => p.TransporterRegId == transporterRegId && p.IsActive && p.PlanStatus == "Available");

            return new TransporterDashboardDto
            {
                TransporterRegId = transporterRegId,
                TransporterName  = transporter.TransporterName,
                TotalDeliveries  = totalDeliveries,
                ActiveDeliveries = activeDeliveries,
                TotalEarned      = totalEarned,
                KycStatus        = kyc?.KycStatus ?? "NotSubmitted",
                BankVerified     = bank?.IsVerified ?? false,
                HasActivePlan    = activePlan
            };
        }

        // ========== TRAVEL PLAN ==========
        public async Task<TravelPlanDto?> GetActivePlanAsync(int transporterRegId)
        {
            var plan = await _context.TransporterTravelPlans
                .Where(p => p.TransporterRegId == transporterRegId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (plan == null) return null;
            return MapPlanToDto(plan);
        }

        public async Task<TravelPlanDto> SaveTravelPlanAsync(TravelPlanDto dto)
        {
            var oldPlans = await _context.TransporterTravelPlans
                .Where(p => p.TransporterRegId == dto.TransporterRegId && p.IsActive)
                .ToListAsync();

            foreach (var old in oldPlans)
            {
                old.IsActive   = false;
                old.PlanStatus = "Cancelled";
            }

            var plan = new TransporterTravelPlan
            {
                TransporterRegId   = dto.TransporterRegId,
                StartLocation      = dto.StartLocation,
                Destination        = dto.Destination,
                PreferredRoute     = dto.PreferredRoute,
                DistanceKm         = dto.DistanceKm,
                StartDate          = dto.StartDate,
                ArrivalDate        = dto.ArrivalDate,
                VehicleType        = dto.VehicleType,
                VehicleRegistration = dto.VehicleRegistration,
                VehicleName        = dto.VehicleName,
                MaxWeightKg        = dto.MaxWeightKg,
                PackageSizeL       = dto.PackageSizeL,
                PackageSizeW       = dto.PackageSizeW,
                PackageSizeH       = dto.PackageSizeH,
                NumberOfPackages   = dto.NumberOfPackages,
                AcceptsFragile     = dto.AcceptsFragile,
                AcceptsPerishable  = dto.AcceptsPerishable,
                PreferredContact   = dto.PreferredContact,
                LanguagePreference = dto.LanguagePreference,
                NotifyNewOrders    = dto.NotifyNewOrders,
                NotifyPayments     = dto.NotifyPayments,
                IsActive           = true,
                PlanStatus         = "Available",
                CreatedAt          = DateTime.UtcNow
            };

            _context.TransporterTravelPlans.Add(plan);
            await _context.SaveChangesAsync();

            dto.PlanId = plan.PlanId;
            return dto;
        }

        public async Task<bool> DeactivatePlanAsync(int planId, int transporterRegId)
        {
            var plan = await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(p => p.PlanId == planId && p.TransporterRegId == transporterRegId);

            if (plan == null) return false;

            plan.IsActive   = false;
            plan.PlanStatus = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        // ========== SEARCH AVAILABLE TRANSPORTERS ==========
        public async Task<List<AvailableTransporterDto>> SearchAvailableTransportersAsync(
            string fromLocation, string toLocation, DateTime travelDate)
        {
            return await (
                from p in _context.TransporterTravelPlans
                join t in _context.TransporterRegisters on p.TransporterRegId equals t.TransporterRegId
                where p.IsActive
                    && p.PlanStatus == "Available"
                    && p.StartDate.Date <= travelDate.Date
                    && p.ArrivalDate.Date >= travelDate.Date
                    && EF.Functions.Like(p.StartLocation.ToLower(), $"%{fromLocation.ToLower()}%")
                    && EF.Functions.Like(p.Destination.ToLower(), $"%{toLocation.ToLower()}%")
                select new AvailableTransporterDto
                {
                    PlanId            = p.PlanId,
                    TransporterRegId  = t.TransporterRegId,
                    TransporterName   = t.TransporterName,
                    VehicleType       = p.VehicleType,
                    VehicleName       = p.VehicleName,
                    StartLocation     = p.StartLocation,
                    Destination       = p.Destination,
                    StartDate         = p.StartDate,
                    ArrivalDate       = p.ArrivalDate,
                    MaxWeightKg       = p.MaxWeightKg,
                    NumberOfPackages  = p.NumberOfPackages,
                    AcceptsFragile    = p.AcceptsFragile,
                    AcceptsPerishable = p.AcceptsPerishable,
                    PreferredContact  = p.PreferredContact
                }
            ).ToListAsync();
        }

        // ========== DELIVERY REQUESTS ==========
        public async Task<TransporterDeliveryRequest> CreateDeliveryRequestAsync(ShopperDeliveryRequestDto dto)
        {
            var plan = await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(p => p.PlanId == dto.PlanId && p.IsActive);

            if (plan == null)
                throw new Exception("Travel plan not found or no longer available.");

            decimal deliveryFee = dto.PackageWeightKg * 10m;

            var request = new TransporterDeliveryRequest
            {
                PlanId           = dto.PlanId,
                TransporterRegId = plan.TransporterRegId,
                ShopperRegId     = dto.ShopperRegId,
                OrderId          = dto.OrderId,
                PickupLocation   = dto.PickupLocation,
                DropoffLocation  = dto.DropoffLocation,
                PackageWeightKg  = dto.PackageWeightKg,
                NumberOfPackages = dto.NumberOfPackages,
                DeliveryFee      = deliveryFee,
                PackageTags      = dto.PackageTags ?? "NA",
                DeliveryStatus   = "Pending",
                CreatedAt        = DateTime.UtcNow
            };

            _context.TransporterDeliveryRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<List<DeliveryRequestDto>> GetPendingRequestsAsync(int transporterRegId)
        {
            // ✅ Fetch raw primitives from DB — no string formatting in SQL
            var raw = await (
                from d in _context.TransporterDeliveryRequests
                join s in _context.ShopperRegisters on d.ShopperRegId equals s.ShopperRegId
                where d.TransporterRegId == transporterRegId && d.DeliveryStatus == "Pending"
                orderby d.CreatedAt descending
                select new
                {
                    d.DeliveryReqId,
                    d.PlanId,
                    d.ShopperRegId,
                    ShopperName      = s.Username,
                    d.PickupLocation,
                    d.DropoffLocation,
                    d.PackageWeightKg,
                    d.NumberOfPackages,
                    d.DeliveryFee,
                    d.PackageTags,
                    d.DeliveryStatus,
                    d.CreatedAt
                }
            )
            .AsNoTracking()
            .ToListAsync();

            // ✅ Map in memory
            return raw.Select(d => new DeliveryRequestDto
            {
                DeliveryReqId    = d.DeliveryReqId,
                PlanId           = d.PlanId,
                ShopperRegId     = d.ShopperRegId,
                ShopperName      = d.ShopperName,
                PickupLocation   = d.PickupLocation,
                DropoffLocation  = d.DropoffLocation,
                PackageWeightKg  = d.PackageWeightKg,
                NumberOfPackages = d.NumberOfPackages,
                DeliveryFee      = d.DeliveryFee,
                PackageTags      = d.PackageTags,
                DeliveryStatus   = d.DeliveryStatus,
                CreatedAt        = d.CreatedAt
            }).ToList();
        }

public async Task<List<ActiveDeliveryDto>> GetActiveDeliveryAsync(int transporterRegId)
{
    var activeStatuses = new List<string> { "Accepted", "ReachedPickup", "PickedUp", "InTransit" };

    var raw = await (
        from d in _context.TransporterDeliveryRequests
        join s in _context.ShopperRegisters on d.ShopperRegId equals s.ShopperRegId
        where d.TransporterRegId == transporterRegId
           && activeStatuses.Contains(d.DeliveryStatus)
        orderby d.AcceptedAt descending
        select new
        {
            d.DeliveryReqId,
            d.DeliveryStatus,
            d.PickupLocation,
            d.DropoffLocation,
            d.NumberOfPackages,
            d.PackageWeightKg,
            d.DeliveryFee,
            d.PackageTags,
            d.AcceptedAt,
            CustomerName = s.Username
        }
    )
    .AsNoTracking()
    .ToListAsync();  // ✅ ToList — returns ALL active deliveries

    return raw.Select(d => new ActiveDeliveryDto
    {
        DeliveryReqId    = d.DeliveryReqId,
        DeliveryCode     = "DEL-" + d.DeliveryReqId.ToString("D4"),
        CustomerName     = d.CustomerName,
        PickupLocation   = d.PickupLocation,
        DropoffLocation  = d.DropoffLocation,
        NumberOfPackages = d.NumberOfPackages,
        PackageWeightKg  = d.PackageWeightKg,
        DeliveryFee      = d.DeliveryFee,
        PackageTags      = d.PackageTags,
        DeliveryStatus   = d.DeliveryStatus,
        AcceptedAt       = d.AcceptedAt,
        EtaInfo          = "~42 min · 32.4 km"
    }).ToList();
}

        public async Task<bool> AcceptDeliveryRequestAsync(int deliveryReqId, int transporterRegId)
        {
            var request = await _context.TransporterDeliveryRequests
                .FirstOrDefaultAsync(d => d.DeliveryReqId == deliveryReqId
                    && d.TransporterRegId == transporterRegId
                    && d.DeliveryStatus == "Pending");

            if (request == null) return false;

            request.DeliveryStatus = "Accepted";
            request.AcceptedAt     = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDeliveryStatusAsync(UpdateDeliveryStatusDto dto)
        {
            var request = await _context.TransporterDeliveryRequests
                .FirstOrDefaultAsync(d => d.DeliveryReqId == dto.DeliveryReqId
                    && d.TransporterRegId == dto.TransporterRegId);

            if (request == null) return false;

            var now = DateTime.UtcNow;

            switch (dto.NewStatus)
            {
                case "ReachedPickup":
                    request.DeliveryStatus  = "ReachedPickup";
                    request.ReachedPickupAt = now;
                    break;
                case "PickedUp":
                    request.DeliveryStatus = "PickedUp";
                    request.PickedUpAt     = now;
                    break;
                case "InTransit":
                    request.DeliveryStatus = "InTransit";
                    request.InTransitAt    = now;
                    break;
                case "Delivered":
                    request.DeliveryStatus = "Delivered";
                    request.DeliveredAt    = now;
                    break;
                default:
                    return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ActiveDeliveryDto>> GetCompletedDeliveriesAsync(int transporterRegId)
        {
            // ✅ Fetch raw primitives — no ToString("D4") in SQL
            var raw = await (
                from d in _context.TransporterDeliveryRequests
                join s in _context.ShopperRegisters on d.ShopperRegId equals s.ShopperRegId
                where d.TransporterRegId == transporterRegId && d.DeliveryStatus == "Delivered"
                orderby d.DeliveredAt descending
                select new
                {
                    d.DeliveryReqId,
                    d.DeliveryStatus,
                    d.PickupLocation,
                    d.DropoffLocation,
                    d.NumberOfPackages,
                    d.PackageWeightKg,
                    d.DeliveryFee,
                    d.PackageTags,
                    d.AcceptedAt,
                    CustomerName = s.Username
                }
            )
            .AsNoTracking()
            .ToListAsync();

            // ✅ Map in memory
            return raw.Select(d => new ActiveDeliveryDto
            {
                DeliveryReqId    = d.DeliveryReqId,
                DeliveryCode     = "DEL-" + d.DeliveryReqId.ToString("D4"),
                CustomerName     = d.CustomerName,
                PickupLocation   = d.PickupLocation,
                DropoffLocation  = d.DropoffLocation,
                NumberOfPackages = d.NumberOfPackages,
                PackageWeightKg  = d.PackageWeightKg,
                DeliveryFee      = d.DeliveryFee,
                PackageTags      = d.PackageTags,
                DeliveryStatus   = d.DeliveryStatus,
                AcceptedAt       = d.AcceptedAt
            }).ToList();
        }

        // ========== EXCEPTION REPORTS ==========
        public async Task<bool> SubmitExceptionReportAsync(ExceptionReportDto dto)
        {
            var report = new TransporterExceptionReport
            {
                DeliveryReqId    = dto.DeliveryReqId,
                TransporterRegId = dto.TransporterRegId,
                ExceptionType    = dto.ExceptionType,
                Description      = dto.Description,
                ReportedAt       = DateTime.UtcNow,
                IsResolved       = false
            };

            _context.TransporterExceptionReports.Add(report);
            await _context.SaveChangesAsync();
            return true;
        }

        // ========== KYC ==========
        public async Task<TransporterKYC?> GetKycAsync(int transporterRegId)
        {
            return await _context.TransporterKYCs
                .FirstOrDefaultAsync(k => k.TransporterRegId == transporterRegId);
        }

        public async Task<TransporterKYC> SubmitKycAsync(
            int transporterRegId, string docType, string docNumber, string fileName)
        {
            var existing = await _context.TransporterKYCs
                .FirstOrDefaultAsync(k => k.TransporterRegId == transporterRegId);

            if (existing != null)
            {
                existing.DocumentType     = docType;
                existing.DocumentNumber   = docNumber;
                existing.DocumentFileName = fileName;
                existing.KycStatus        = "Pending";
                existing.SubmittedAt      = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existing;
            }

            var kyc = new TransporterKYC
            {
                TransporterRegId  = transporterRegId,
                DocumentType      = docType,
                DocumentNumber    = docNumber,
                DocumentFileName  = fileName,
                KycStatus         = "Pending",
                SubmittedAt       = DateTime.UtcNow
            };

            _context.TransporterKYCs.Add(kyc);
            await _context.SaveChangesAsync();
            return kyc;
        }

        // ========== BANK DETAILS ==========
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
                existing.BankName      = dto.BankName;
                existing.AccountNumber = dto.AccountNumber;
                existing.BranchName    = dto.BranchName;
                existing.IfscCode      = dto.IfscCode;
                existing.IsVerified    = false;
                existing.SubmittedAt   = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existing;
            }

            var bank = new TransporterBankDetails
            {
                TransporterRegId = dto.TransporterRegId,
                BankName         = dto.BankName,
                AccountNumber    = dto.AccountNumber,
                BranchName       = dto.BranchName,
                IfscCode         = dto.IfscCode,
                IsVerified       = false,
                SubmittedAt      = DateTime.UtcNow
            };

            _context.TransporterBankDetails.Add(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        // ========== PROFILE ==========
        public async Task<TransporterProfileDto?> GetProfileAsync(int transporterRegId)
        {
            var t = await _context.TransporterRegisters
                .FirstOrDefaultAsync(x => x.TransporterRegId == transporterRegId);
            if (t == null) return null;

            var kyc  = await _context.TransporterKYCs
                .FirstOrDefaultAsync(k => k.TransporterRegId == transporterRegId);
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
                KycStatus          = kyc?.KycStatus ?? "NotSubmitted",
                BankVerified       = bank?.IsVerified ?? false,
                TransporterRegDate = t.TransporeterRegDate
            };
        }

        public async Task<bool> UpdateProfileAsync(UpdateTransporterProfileDto dto)
        {
            var t = await _context.TransporterRegisters
                .FirstOrDefaultAsync(x => x.TransporterRegId == dto.TransporterRegId);
            if (t == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.TransporterName)) t.TransporterName = dto.TransporterName;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))     t.PhoneNumber     = dto.PhoneNumber;
            if (dto.Address    != null) t.Address    = dto.Address;
            if (dto.Town       != null) t.Town       = dto.Town;
            if (dto.City       != null) t.City       = dto.City;
            if (dto.State      != null) t.State      = dto.State;
            if (dto.Country    != null) t.Country    = dto.Country;
            if (dto.PostalCode != null) t.PostalCode = dto.PostalCode;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePasswordAsync(int transporterRegId, string newHashedPassword)
        {
            var t = await _context.TransporterRegisters
                .FirstOrDefaultAsync(x => x.TransporterRegId == transporterRegId);
            if (t == null) return false;

            t.Password = newHashedPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        // ========== HELPERS ==========
        private static TravelPlanDto MapPlanToDto(TransporterTravelPlan plan) => new TravelPlanDto
        {
            PlanId              = plan.PlanId,
            TransporterRegId    = plan.TransporterRegId,
            StartLocation       = plan.StartLocation,
            Destination         = plan.Destination,
            PreferredRoute      = plan.PreferredRoute,
            DistanceKm          = plan.DistanceKm,
            StartDate           = plan.StartDate,
            ArrivalDate         = plan.ArrivalDate,
            VehicleType         = plan.VehicleType,
            VehicleRegistration = plan.VehicleRegistration,
            VehicleName         = plan.VehicleName,
            MaxWeightKg         = plan.MaxWeightKg,
            PackageSizeL        = plan.PackageSizeL,
            PackageSizeW        = plan.PackageSizeW,
            PackageSizeH        = plan.PackageSizeH,
            NumberOfPackages    = plan.NumberOfPackages,
            AcceptsFragile      = plan.AcceptsFragile,
            AcceptsPerishable   = plan.AcceptsPerishable,
            PreferredContact    = plan.PreferredContact,
            LanguagePreference  = plan.LanguagePreference,
            NotifyNewOrders     = plan.NotifyNewOrders,
            NotifyPayments      = plan.NotifyPayments
        };

        public async Task<List<TransporterDBNotifications>> GetUnreadNotificationsAsync(int transporterId)
        {
            return await _context.TransporterDBNotifications
                .Where(n => n.TransporterRegId == transporterId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new TransporterDBNotifications
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    CreatedDate = n.CreatedDate,
                    IsRead = n.IsRead
                })
                .ToListAsync();
        }

        public async Task MarkAllAsReadAsync(int transporterId)
        {
            var notifications = await _context.TransporterDBNotifications
                .Where(n => n.TransporterRegId == transporterId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

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
    }
}