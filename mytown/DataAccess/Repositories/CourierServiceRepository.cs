using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class CourierServiceRepository : ICourierServiceRepository
    {
        private readonly AppDbContext _context;

        public CourierServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCourierEmailTaken(string email)
        {
            return await _context.CourierService.AnyAsync(c => c.CourierEmail == email);
        }

        public async Task SavePendingCourierVerification(PendingCourierVerification pending)
        {
            _context.PendingCourierVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingCourierVerification> FindPendingCourierVerificationByToken(string token)
        {
            return await _context.PendingCourierVerifications.FirstOrDefaultAsync(p => p.Token == token);
        }
        public async Task DeletePendingCourierVerification(string token)
        {
            var record = await _context.PendingCourierVerifications.FirstOrDefaultAsync(p => p.Token == token);
            if (record != null)
            {
                _context.PendingCourierVerifications.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<CourierService> RegisterCourier(CourierService courier)
        {
            _context.CourierService.Add(courier);
            await _context.SaveChangesAsync();
            return courier;
        }

        //     public async Task<List<CourierBranch>> GetBestCourierOptions(BusinessRegister business, ShopperRegister shopper, decimal productWeightKg)
        //{
        //    try
        //    {
        //        if (business == null || shopper == null)
        //        {
        //            Console.WriteLine("Error: Business or Shopper object is null.");
        //            return new List<CourierBranch>();
        //        }

        //        var storeCity = business.businessCity?.Trim().ToLower() ?? string.Empty;
        //        var storeState = business.businessState?.Trim().ToLower() ?? string.Empty;
        //        var storeCountry = business.businessCountry?.Trim().ToLower() ?? string.Empty;
        //        var shopperCity = shopper.City?.Trim().ToLower() ?? string.Empty;

        //        if (string.IsNullOrEmpty(storeCity) || string.IsNullOrEmpty(storeState) || string.IsNullOrEmpty(storeCountry) || string.IsNullOrEmpty(shopperCity))
        //        {
        //            Console.WriteLine("Error: One or more required fields are empty.");
        //            return new List<CourierBranch>();
        //        }

        //        var courierList = await _context.CourierBranchs
        //            .Where(cb => cb.City.ToLower() == storeCity &&
        //                         cb.State.ToLower() == storeState &&
        //                         cb.Country.ToLower() == storeCountry &&
        //                         !string.IsNullOrEmpty(cb.Destinations))
        //            .AsNoTracking()
        //            .ToListAsync();

        //        if (courierList == null || !courierList.Any())
        //        {
        //            Console.WriteLine("No matching couriers found.");
        //            return new List<CourierBranch>();
        //        }

        //        var matchingCouriers = courierList
        //            .Where(cb =>
        //                cb.Destinations.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                    .Select(dest => dest.Trim().ToLower())
        //                    .Contains(shopperCity))
        //            .Select(cb => new
        //            {
        //                Courier = cb,
        //                MaxWeight = ExtractMaxWeight(cb.WeightRange),
        //                MaxDistance = ExtractMaxDistance(cb.DistanceRange),
        //                CostPerKm = cb.Charges / (decimal)(ExtractMaxDistance(cb.DistanceRange) == 0 ? 1 : ExtractMaxDistance(cb.DistanceRange))
        //            })
        //            .Where(x => x.MaxWeight >= productWeightKg)
        //            .GroupBy(x => x.Courier.ShippingMode)
        //            .Select(g => g.OrderBy(x => x.CostPerKm).First().Courier)
        //            .ToList();

        //        return matchingCouriers;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception in GetBestCourierOptions: {ex.Message}");
        //        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        //        return new List<CourierBranch>();
        //    }
        //}

        public async Task<List<BestcourierinfoDto>> GetBestCourierOptions(string storeCity, string storeState, string storeCountry, string shopperCity, decimal productWeightKg)
        {
            try
            {
                var courierList = await _context.CourierBranches
                    .Where(cb => cb.City.ToLower() == storeCity.ToLower() &&
                                 cb.State.ToLower() == storeState.ToLower() &&
                                 cb.Country.ToLower() == storeCountry.ToLower() &&
                                 !string.IsNullOrEmpty(cb.Destinations))
                    .AsNoTracking()
                    .ToListAsync();

                var matchingCouriers = courierList
                    .Where(cb =>
                        cb.Destinations.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(dest => dest.Trim().ToLower())
                            .Contains(shopperCity.ToLower()));

                var bestCourierOptions = matchingCouriers
                    .Select(cb => new BestcourierinfoDto
                    { BranchId  = cb.BranchId,
                        ShippingMode = cb.ShippingMode,
                        Cost = cb.Charges,
                        MaxWeight = ExtractMaxWeight(cb.WeightRange),
                        MaxDistance = ExtractMaxDistance(cb.DistanceRange)
                    })
                    .Where(x => x.MaxWeight >= productWeightKg)
                    .GroupBy(x => x.ShippingMode.ToLower())
                    .Select(g => g.OrderBy(x => x.Cost).First())
                    .ToList();

                return bestCourierOptions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CourierRepository.GetBestCourierOptions: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return new List<BestcourierinfoDto>();
            }
        }



        private decimal ExtractMaxWeight(string weightRange)
        {
            if (string.IsNullOrEmpty(weightRange)) return 0;
            var parts = weightRange.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return 0;
            var max = parts[1].ToLower().Replace("kg", "").Trim();
            return decimal.TryParse(max, out var result) ? result : 0;
        }

        private int ExtractMaxDistance(string distanceRange)
        {
            if (string.IsNullOrEmpty(distanceRange)) return 0;
            var parts = distanceRange.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return 0;
            var max = parts[1].ToLower().Replace("km", "").Trim();
            return int.TryParse(max, out var result) ? result : 0;
        }


        //    public async Task<CourierService> AddCourierAsync(CourierService courier)
        //{
        //    _context.CourierService.Add(courier);
        //    await _context.SaveChangesAsync();
        //    return courier;
        //}

        public async Task<List<AssignedOrderDto>> GetAssignedOrdersByCourierIdAsync(int courierId)
        {
            var result = await (from shipping in _context.ShippingDetails
                                join orderDetail in _context.OrderDetails on shipping.OrderDetailId equals orderDetail.OrderDetailId
                                join product in _context.products on orderDetail.ProductId equals product.ProductId
                                join order in _context.Orders on orderDetail.OrderId equals order.OrderId
                                join shopper in _context.ShopperRegisters on order.ShopperRegId equals shopper.ShopperRegId
                                join store in _context.BusinessRegisters on orderDetail.StoreId equals store.BusRegId
                                join branch in _context.CourierBranches on shipping.BranchId equals branch.BranchId
                                where branch.CourierId == courierId
                                select new AssignedOrderDto
                                {
                                    ShippingDetailId = shipping.ShippingDetailId,
                                    OrderId = order.OrderId,
                                    CustomerName = shopper.Username,
                                    CustomerPhoneNumber = shopper.PhoneNumber,
                                    ShippingAddress = $"{shopper.Address}, {shopper.City}, {shopper.State}, {shopper.Country} - {shopper.PostalCode}",
                                    StoreName = store.BusinessName,
                                    ProductName = product.ProductName,
                                   // ProductWeight = product.product_weight??0,
                                    Quantity = orderDetail.Quantity,
                                    ShippingType = shipping.ShippingType,
                                    ShippingStatus = shipping.ShippingStatus,
                                    Cost = shipping.Cost,
                                    TrackingId = shipping.TrackingId,
                                    EstimatedDeliveryDate = order.OrderDate.AddDays(shipping.EstimatedDays)
                                }).ToListAsync();

            return result;
        }

    }
}
