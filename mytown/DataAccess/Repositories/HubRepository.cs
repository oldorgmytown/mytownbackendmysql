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
using Razorpay.Api;
using System.Data;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace mytown.DataAccess.Repositories
{
    public class HubRepository : IHubRepository
    {

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;

        public HubRepository(AppDbContext context, IEmailService emailservice, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _emailService = emailservice;
            _connectionString = config.GetConnectionString("mysqlConnection");
            _cache = cache;
        }
        
            public async Task<List<HubAddressDto>> GetAllHubLocationsAsync()
            {
                return await _context.HubDetails
                    .AsNoTracking()
                    .OrderBy(h => h.State).ThenBy(h => h.City).ThenBy(h => h.HubName)
                    .Select(h => new HubAddressDto
                    {
                        HubId = h.HubId,
                        HubAddressId = h.HubAddressId,
                        HubName = h.HubName,
                        AddressLine = h.AddressLine,
                        Town = h.Town,
                        City = h.City,
                        State = h.State,
                        Country = h.Country,
                        Pin = h.Pin,
                        Phone = h.Phone
                    })
                    .ToListAsync();
            }


        public async Task<List<HubStoreOrderListDto>> GetTransporterStoreOrdersAsync()
        {
            var rows = await (
                from sd in _context.ShippingDetails
                join so in _context.StoreOrders on sd.StoreOrderId equals so.StoreOrderId
                join b in _context.BusinessRegisters on so.StoreId equals b.BusRegId
                join t in _context.TransporterRegisters on sd.TransporterRegId equals t.TransporterRegId

                join tp in _context.TransporterTravelPlans
                    on sd.TransporterPlanId equals tp.PlanId into tpGroup
                from tp in tpGroup.DefaultIfEmpty()

                where sd.TransporterRegId != null
                orderby so.StoreOrderId descending
                select new
                {
                    so.StoreOrderId,
                    so.OrderId,
                    so.Storeorder_Status,
                    t.TransporterRegId,
                    t.TransporterName,
                    PickupDate = tp != null ? tp.StartDate : (DateTime?)null,
                    EstimatedDeliveryDate = tp != null ? tp.ArrivalDate : (DateTime?)null,
                    StoreId = b.BusRegId,
                    b.BusinessName,
                    b.Town,
                    b.BusinessCity,

                    IsHandedOver = _context.HubStoreOrderVerifications
                        .Any(v => v.StoreOrderId == so.StoreOrderId)
                }
            ).AsNoTracking().ToListAsync();

            return rows.Select(r => new HubStoreOrderListDto
            {
                StoreOrderId = r.StoreOrderId,
                OrderId = r.OrderId,
                OrderStatus =
                    r.Storeorder_Status == "Pending" ? "New" :
                    r.Storeorder_Status == "Delivered" ? "Completed" :
                    "In Progress",

                TransporterRegId = r.TransporterRegId,
                TransporterName = r.TransporterName,
                PickupDate = r.PickupDate,
                EstimatedDeliveryDate = r.EstimatedDeliveryDate,

                StoreId = r.StoreId,
                StoreName = r.BusinessName,
                StoreLocation = r.Town + ", " + r.BusinessCity,

                HubStatus = r.IsHandedOver ? "Handed Over" : "New"
            }).ToList();
        }

        public async Task<List<HubSenderOrderListDto>> GetTransporterSenderOrdersAsync()
        {
            var rows = await (
                from so in _context.SenderOrders
                join s in _context.SenderRegisters on so.SenderRegId equals s.SenderRegId
                join t in _context.TransporterRegisters on so.TransporterRegId equals t.TransporterRegId

                join tp in _context.TransporterTravelPlans
                    on so.TransporterPlanId equals tp.PlanId into tpGroup
                from tp in tpGroup.DefaultIfEmpty()

                where so.TransporterRegId != null
                orderby so.SenderOrderId descending
                select new
                {
                    so.SenderOrderId,
                    so.DeliveryStatus,
                    t.TransporterRegId,
                    t.TransporterName,
                    PickupDate = tp != null ? tp.StartDate : so.PickupDate,
                    EstimatedDeliveryDate = tp != null ? tp.ArrivalDate : (DateTime?)null,
                    so.SenderRegId,
                    s.SenderName,
                    so.PickupTown,
                    so.PickupCity
                }
            ).AsNoTracking().ToListAsync();

            return rows.Select(r => new HubSenderOrderListDto
            {
                SenderOrderId = r.SenderOrderId,
                OrderStatus =
                    r.DeliveryStatus == "Pending" ? "New" :
                    r.DeliveryStatus == "Delivered" ? "Completed" :
                    "In Progress",

                TransporterRegId = r.TransporterRegId,
                TransporterName = r.TransporterName,
                PickupDate = r.PickupDate,
                EstimatedDeliveryDate = r.EstimatedDeliveryDate,

                SenderRegId = r.SenderRegId,
                SenderName = r.SenderName,
                SenderLocation = r.PickupTown + ", " + r.PickupCity,


                HubStatus = null
            }).ToList();
        }

        public Task<bool> StoreOrderExistsAsync(int storeOrderId)
    => _context.StoreOrders.AnyAsync(s => s.StoreOrderId == storeOrderId);

        public async Task<HubStoreVerificationDto?> GetVerificationAsync(int storeOrderId)
        {
            var v = await _context.HubStoreOrderVerifications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);

            return v == null ? null : ToDto(v);
        }
        private static HubStoreVerificationDto ToDto(HubStoreOrderVerification v) => new()
        {
            VerificationId = v.VerificationId,
            StoreOrderId = v.StoreOrderId,
            HubId = v.HubId,
            PackageVerified = v.PackageVerified,
            SecurityCheck = v.SecurityCheck,
            TravelPlanVerified = v.TravelPlanVerified,
            TransporterVerified = v.TransporterVerified,
            PackageHandedOver = v.PackageHandedOver,
            Remarks = v.Remarks,
            UpdatedAt = v.UpdatedAt,
            HubStatus = "Handed Over"
        //v.PackageHandedOver ? "Handed Over" :
        //(v.PackageVerified && v.SecurityCheck && v.TravelPlanVerified && v.TransporterVerified)
        //    ? "Ready for Handover" :
        //(v.PackageVerified || v.SecurityCheck || v.TravelPlanVerified || v.TransporterVerified)
        //    ? "In Verification" :
        //"New Intake"
        };

        public async Task<HubStoreVerificationDto> SaveVerificationAsync(int storeOrderId, SaveHubVerificationDto dto)
        {
            var v = await _context.HubStoreOrderVerifications
                .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);

            if (v == null)
            {
                v = new HubStoreOrderVerification
                {
                    StoreOrderId = storeOrderId,
                    HubId = dto.HubId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.HubStoreOrderVerifications.Add(v);
            }

            v.PackageVerified = dto.PackageVerified;
            v.SecurityCheck = dto.SecurityCheck;
            v.TravelPlanVerified = dto.TravelPlanVerified;
            v.TransporterVerified = dto.TransporterVerified;
            v.PackageHandedOver = dto.PackageHandedOver;
            v.Remarks = dto.Remarks;
            v.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ToDto(v);
        }

        // ---------------- SENDER ORDER VERIFICATION ----------------
        public Task<bool> SenderOrderExistsAsync(int senderOrderId)
            => _context.SenderOrders.AnyAsync(s => s.SenderOrderId == senderOrderId);

        public async Task<SenderVerificationDto?> GetSenderVerificationAsync(int senderOrderId)
        {
            var v = await _context.HubSenderOrderVerifications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SenderOrderId == senderOrderId);

            if (v == null)
                return null;

            return ToSenderDto(v);
        }

        public async Task<SenderVerificationDto> SaveSenderVerificationAsync(
            int senderOrderId, SaveHubVerificationDto dto)
        {
            var v = await _context.HubSenderOrderVerifications
                .FirstOrDefaultAsync(x => x.SenderOrderId == senderOrderId);

            if (v == null)
            {
                v = new HubSenderOrderVerification
                {
                    SenderOrderId = senderOrderId,
                    HubId = dto.HubId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.HubSenderOrderVerifications.Add(v);
            }

            v.PackageVerified = dto.PackageVerified;
            v.SecurityCheck = dto.SecurityCheck;
            v.TravelPlanVerified = dto.TravelPlanVerified;
            v.TransporterVerified = dto.TransporterVerified;
            v.PackageHandedOver = dto.PackageHandedOver;
            v.Remarks = dto.Remarks;
            v.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ToSenderDto(v);
        }

        private static SenderVerificationDto ToSenderDto(HubSenderOrderVerification v) => new()
        {
            VerificationId = v.VerificationId,
            SenderOrderId = v.SenderOrderId,
            HubId = v.HubId,
            PackageVerified = v.PackageVerified,
            SecurityCheck = v.SecurityCheck,
            TravelPlanVerified = v.TravelPlanVerified,
            TransporterVerified = v.TransporterVerified,
            PackageHandedOver = v.PackageHandedOver,
            Remarks = v.Remarks,
            UpdatedAt = v.UpdatedAt,
            HubStatus = v.PackageHandedOver ? "Handed Over" : "New"
        };

        public async Task<HubStoreOrderDetailsDto?> GetStoreOrderDetailsAsync(int storeOrderId)
        {
            var r = await (
                from sd in _context.ShippingDetails
                join so in _context.StoreOrders on sd.StoreOrderId equals so.StoreOrderId
                join b in _context.BusinessRegisters on so.StoreId equals b.BusRegId
                join t in _context.TransporterRegisters on sd.TransporterRegId equals t.TransporterRegId

                join tp in _context.TransporterTravelPlans
                    on sd.TransporterPlanId equals tp.PlanId into tpGroup
                from tp in tpGroup.DefaultIfEmpty()

                where sd.StoreOrderId == storeOrderId && sd.TransporterRegId != null
                select new
                {
                    so.StoreOrderId,
                    so.OrderId,
                    so.Storeorder_Status,

                    b.BusRegId,
                    b.BusinessName,
                    b.Address1,
                    b.Address2,
                    b.Town,
                    b.BusinessCity,
                    b.BusinessState,
                    b.PostalCode,
                    b.BusMobileNo,

                    t.TransporterRegId,
                    t.TransporterName,
                    t.Address,
                    TransporterTown = t.Town,
                    TransporterCity = t.City,
                    TransporterState = t.State,
                    TransporterPostal = t.PostalCode,
                    t.PhoneNumber,
                    t.Status,

                    PlanStartDate = tp != null ? tp.StartDate : (DateTime?)null,
                    PlanArrivalDate = tp != null ? tp.ArrivalDate : (DateTime?)null,
                    DestTown = tp != null ? tp.DestinationTown : null,
                    DestCity = tp != null ? tp.DestinationCity : null,
                    DestState = tp != null ? tp.DestinationState : null,
                    DestPin = tp != null ? tp.DestinationPin : null,
                    VehicleName = tp != null ? tp.VehicleName : null,
                    VehicleReg = tp != null ? tp.VehicleRegistration : null,
                    VehicleType = tp != null ? tp.VehicleType : null,

                    // Start location = hub whose pin matches the store pincode
                    Hub = _context.HubDetails
                        .Where(h => h.Pin == b.PostalCode)
                        .Select(h => new { h.HubName, h.City, h.State })
                        .FirstOrDefault(),

                    Package = _context.ShippingPackageDetails
                        .Where(p => p.StoreOrderId == so.StoreOrderId)
                        .Select(p => new PackageDetailsDto
                        {
                            Length = p.PackageLength,
                            Width = p.PackageWidth,
                            Height = p.PackageHeight,
                            DimensionUnit = p.DimensionUnit,
                            Weight = p.PackageWeight,
                            WeightUnit = p.WeightUnit
                        })
                        .FirstOrDefault()
                }
            ).AsNoTracking().FirstOrDefaultAsync();

            if (r == null)
                return null;

            var v = await _context.HubStoreOrderVerifications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);

            return new HubStoreOrderDetailsDto
            {
                StoreOrderId = r.StoreOrderId,
                OrderId = r.OrderId,
                OrderStatus =
                    r.Storeorder_Status == "Pending" ? "New" :
                    r.Storeorder_Status == "Delivered" ? "Completed" : "In Progress",
                HubStatus = (v != null && v.PackageHandedOver) ? "Handed Over" : "New",

                Package = r.Package,

                Business = new BusinessDetailsDto
                {
                    StoreId = r.BusRegId,
                    StoreName = r.BusinessName,
                    Address = string.Join(", ", new[] { r.Address1, r.Address2, r.Town, r.BusinessCity, r.BusinessState }
                                  .Where(s => !string.IsNullOrWhiteSpace(s)))
                              + (string.IsNullOrEmpty(r.PostalCode) ? "" : " " + r.PostalCode),
                    Phone = r.BusMobileNo
                },

                Transporter = new TransporterDetailsDto
                {
                    TransporterRegId = r.TransporterRegId,
                    TransporterName = r.TransporterName,
                    Address = string.Join(", ", new[] { r.Address, r.TransporterTown, r.TransporterCity, r.TransporterState }
                                  .Where(s => !string.IsNullOrWhiteSpace(s)))
                              + (string.IsNullOrEmpty(r.TransporterPostal) ? "" : " - " + r.TransporterPostal),
                    Phone = r.PhoneNumber,
                    Status = r.Status
                },

                //Travel = new TravelDetailsDto
                //{
                //    StartLocationName = r.Hub?.HubName,
                //    StartLocationAddress = r.Hub == null ? null : $"{r.Hub.City}, {r.Hub.State}",

                //    EndLocationName = r.,
                //    EndLocationAddress = null,

                //    StartDate = r.PlanStartDate,
                //    EtaDate = r.PlanArrivalDate,

                //    VehicleName = r.PlanVehicleName,
                //    VehicleNumber = r.PlanVehicleNumber
                //},

                Checklist = new ChecklistDto
                {
                    PackageVerified = v?.PackageVerified ?? false,
                    SecurityCheck = v?.SecurityCheck ?? false,
                    TravelPlanVerified = v?.TravelPlanVerified ?? false,
                    TransporterVerified = v?.TransporterVerified ?? false,
                    PackageHandedOver = v?.PackageHandedOver ?? false
                }
            };
        }
    }
}


