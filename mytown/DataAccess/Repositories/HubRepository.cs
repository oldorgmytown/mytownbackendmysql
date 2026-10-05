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
                    StoreId = b.BusRegId,
                    b.BusinessName,
                    b.Town,
                    b.BusinessCity,

                    Package = _context.ShippingPackageDetails
                        .Where(p => p.StoreOrderId == so.StoreOrderId)
                        .Select(p => new
                        {
                            p.PackageLength,
                            p.PackageWidth,
                            p.PackageHeight,
                            p.DimensionUnit,
                            p.PackageWeight,
                            p.WeightUnit
                        })
                        .FirstOrDefault()
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

                StoreId = r.StoreId,
                StoreName = r.BusinessName,
                StoreLocation = r.Town + ", " + r.BusinessCity,

                PackageSpecs = r.Package != null
                    && r.Package.PackageLength.HasValue
                    && r.Package.PackageWidth.HasValue
                    && r.Package.PackageHeight.HasValue
                    ? $"{r.Package.PackageLength:0.##} × {r.Package.PackageWidth:0.##} × {r.Package.PackageHeight:0.##} {r.Package.DimensionUnit}"
                    : null,
                PackageWeight = r.Package?.PackageWeight,
                WeightUnit = r.Package?.WeightUnit,

                HubStatus = null
            }).ToList();
        }

    }
        
}
