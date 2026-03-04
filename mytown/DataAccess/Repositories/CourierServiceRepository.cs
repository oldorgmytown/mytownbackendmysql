using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;
using System.Text;

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

        //resend email 
        public async Task<PendingCourierVerification?> FindPendingVerificationByEmail(string email)
        {
            return await _context.PendingCourierVerifications
                .FirstOrDefaultAsync(p =>
                    p.Email == email &&
                    p.ExpiryDate > DateTime.UtcNow
                );
        }


        public async Task RemoveVerification(PendingCourierVerification verification)
        {
            _context.PendingCourierVerifications.Remove(verification);
            await _context.SaveChangesAsync();
        }

        public async Task SavePendingVerification(PendingCourierVerification pending)
        {
            _context.PendingCourierVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }



        //Upload CVS file for courier branches



        public async Task<List<CourierBranchCsvRowDto>> ParseAndValidateCsv(IFormFile file)
        {
            var result = new List<CourierBranchCsvRowDto>();

            using var stream = new StreamReader(file.OpenReadStream());
            int rowNumber = 0;

            while (!stream.EndOfStream)
            {
                string line = await stream.ReadLineAsync();
                rowNumber++;

                if (rowNumber == 1)
                    continue; // header

                var raw = SplitCsvLine(line);

                if (raw.Count != 15)
                {
                    result.Add(new CourierBranchCsvRowDto
                    {
                        RowNumber = rowNumber,
                        IsValid = false
                    });
                    continue;
                }

                var dto = new CourierBranchCsvRowDto
                {
                    RowNumber = rowNumber,
                    CourierServiceName = raw[0],
                    Country = raw[1],
                    State = raw[2],
                    City = raw[3],
                    Town = raw[4],
                    BranchAddress = raw[5],
                    BranchPhoneNumber = raw[6],
                    BranchEmailId = raw[7],
                    BranchContactPerson = raw[8],
                    Destinations = raw[9],
                    ShippingMode = raw[10],
                    DistanceRange = raw[11],
                    WeightRange = raw[12],
                    Charges = decimal.TryParse(raw[13], out var p) ? p : 0,
                    EstimateDaysRaw = raw[14],
                    EstimateDays = CsvValidationHelper.ExtractMaxDays(raw[14])
                };

                dto.IsValid =
                    !string.IsNullOrWhiteSpace(dto.CourierServiceName) &&
                    !string.IsNullOrWhiteSpace(dto.Country) &&
                    !string.IsNullOrWhiteSpace(dto.State) &&
                    !string.IsNullOrWhiteSpace(dto.City) &&
                    !string.IsNullOrWhiteSpace(dto.Town) &&
                    !string.IsNullOrWhiteSpace(dto.BranchAddress) &&
                    CsvValidationHelper.IsValidPhone(dto.BranchPhoneNumber) &&
                    CsvValidationHelper.IsValidEmail(dto.BranchEmailId) &&
                    CsvValidationHelper.IsCommaSeparatedList(dto.Destinations) &&
                    (dto.ShippingMode.Equals("air", StringComparison.OrdinalIgnoreCase) ||
                     dto.ShippingMode.Equals("surface", StringComparison.OrdinalIgnoreCase)) &&
                    dto.Charges > 0 &&
                    dto.EstimateDays > 0;

                result.Add(dto);
            }

            return result;
        }

        private static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString().Trim());
            return result;
        }


        //public async Task<bool> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows)
        //{
        //    if (rows == null || !rows.Any())
        //        throw new Exception("No data received.");

        //    if (rows.Any(r => !r.IsValid))
        //        throw new Exception("Some rows are invalid. Please fix them before saving.");

        //    // 1️⃣ Find CourierId for every row using CourierServiceName
        //    foreach (var r in rows)
        //    {
        //        var service = await _context.CourierService
        //            .FirstOrDefaultAsync(s => s.CourierServiceName == r.CourierServiceName);

        //        if (service == null)
        //            throw new Exception($"Courier service '{r.CourierServiceName}' does not exist. Please create courier service first.");

        //        r.CourierId = service.CourierId;
        //    }

        //    // 2️⃣ Check duplicates in DB
        //    foreach (var r in rows)
        //    {
        //        bool exists = await _context.CourierBranches.AnyAsync(cb =>
        //            cb.CourierId == r.CourierId &&               // IMPORTANT
        //            cb.Country == r.Country &&
        //            cb.State == r.State &&
        //            cb.City == r.City &&
        //            cb.Town == r.Town &&
        //            cb.BranchAddress == r.BranchAddress &&
        //            cb.BranchPhoneNumber == r.BranchPhoneNumber &&
        //            cb.BranchEmailId == r.BranchEmailId &&
        //            cb.BranchContactPerson == r.BranchContactPerson &&
        //            cb.ShippingMode == r.ShippingMode &&
        //            cb.DistanceRange == r.DistanceRange &&
        //            cb.WeightRange == r.WeightRange
        //        );

        //        if (exists)
        //            throw new Exception("Duplicate rows found in database. Please remove duplicates and reupload.");
        //    }

        //    // 3️⃣ Convert and save
        //    var entities = rows.Select(r => new CourierBranch
        //    {
        //        CourierId = r.CourierId,                // REQUIRED ✔
        //        CourierServiceName = r.CourierServiceName,
        //        Country = r.Country,
        //        State = r.State,
        //        City = r.City,
        //        Town = r.Town,
        //        BranchAddress = r.BranchAddress,
        //        BranchPhoneNumber = r.BranchPhoneNumber,
        //        BranchEmailId = r.BranchEmailId,
        //        BranchContactPerson = r.BranchContactPerson,
        //        Destinations = r.Destinations,
        //        ShippingMode = r.ShippingMode,
        //        DistanceRange = r.DistanceRange,
        //        WeightRange = r.WeightRange,
        //        Charges = r.Charges,
        //        EstimateDays = r.EstimateDays,

        //    }).ToList();

        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        await _context.CourierBranches.AddRangeAsync(entities);
        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        throw new Exception("SAVE ERROR: " + ex.Message);
        //    }
        //}


        public async Task<bool> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows)
        {
            if (rows == null || !rows.Any())
                throw new Exception("No data received.");

            if (rows.Any(r => !r.IsValid))
                throw new Exception("Some rows are invalid. Please fix them before saving.");

            // 1️⃣ Resolve CourierId
            foreach (var r in rows)
            {
                var service = await _context.CourierService
                    .FirstOrDefaultAsync(s => s.CourierServiceName == r.CourierServiceName);

                if (service == null)
                    throw new Exception($"Courier service '{r.CourierServiceName}' does not exist.");

                r.CourierId = service.CourierId;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var r in rows)
                {
                    // 2️⃣ Check if BRANCH already exists
                    var branch = await _context.CourierBranches.FirstOrDefaultAsync(b =>
                        b.CourierId == r.CourierId &&
                        b.BranchEmailId == r.BranchEmailId
                    );

                    // 3️⃣ If branch not exists → create it
                    if (branch == null)
                    {
                        branch = new CourierBranch
                        {
                            CourierId = r.CourierId,
                            CourierServiceName = r.CourierServiceName,
                            Country = r.Country,
                            State = r.State,
                            City = r.City,
                            Town = r.Town,
                            BranchAddress = r.BranchAddress,
                            BranchPhoneNumber = r.BranchPhoneNumber,
                            BranchEmailId = r.BranchEmailId,
                            BranchContactPerson = r.BranchContactPerson,
                            IsActive = true
                        };

                        _context.CourierBranches.Add(branch);
                        await _context.SaveChangesAsync(); // 🔑 get BranchId
                    }

                    // 4️⃣ Prevent duplicate SERVICE for same branch
                    bool serviceExists = await _context.CourierBranchServices.AnyAsync(s =>
                        s.BranchId == branch.BranchId &&
                        s.ShippingMode == r.ShippingMode &&
                        s.DistanceRange == r.DistanceRange &&
                        s.WeightRange == r.WeightRange
                    );

                    if (serviceExists)
                        continue; // skip duplicate service row

                    // 5️⃣ Add service
                    var serviceEntity = new CourierBranchService
                    {
                        BranchId = branch.BranchId,
                        ShippingMode = r.ShippingMode,
                        DistanceRange = r.DistanceRange,
                        WeightRange = r.WeightRange,
                        Charges = r.Charges,
                        EstimateDays = r.EstimateDays,
                        Destinations = r.Destinations
                    };

                    _context.CourierBranchServices.Add(serviceEntity);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("SAVE ERROR: " + ex.Message);
            }
        }

        //public async Task<List<BestcourierinfoDto>> GetBestCourierOptions(
        // string storeCity,
        // string storeState,
        // string storeCountry,
        // string shopperCity,
        // decimal productWeightKg)
        //{
        //    try
        //    {
        //        var courierBranches = await _context.CourierBranches
        //            .Where(cb =>
        //                cb.City.ToLower() == storeCity.ToLower() &&
        //                cb.State.ToLower() == storeState.ToLower() &&
        //                cb.Country.ToLower() == storeCountry.ToLower() &&
        //                !string.IsNullOrEmpty(cb.Destinations))
        //            .AsNoTracking()
        //            .ToListAsync();

        //        var matchingCouriers = courierBranches
        //            .Where(cb =>
        //                cb.Destinations
        //                  .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                  .Select(d => d.Trim().ToLower())
        //                  .Contains(shopperCity.ToLower()));

        //        var bestCourierOptions = matchingCouriers
        //            .Select(cb =>
        //            {
        //                var maxWeight = ExtractMaxWeight(cb.WeightRange);
        //                var maxDays = GetMaxDeliveryDays(cb.ShippingMode);

        //                return new
        //                {
        //                    Dto = new BestcourierinfoDto
        //                    {
        //                        BranchId = cb.BranchId,
        //                        ShippingMode = cb.ShippingMode,
        //                        Cost = cb.Charges,

        //                        // ✅ Delivery info for frontend
        //                        MaxDeliveryDays = maxDays,
        //                        DeliveryDaysRange = GetDeliveryRangeText(maxDays),
        //                        EstimatedDeliveryDate = GetEstimatedDeliveryDate(maxDays)
        //                    },
        //                    MaxWeight = maxWeight
        //                };
        //            })

        //            // ✅ Filter invalid couriers
        //            .Where(x => x.MaxWeight >= productWeightKg)

        //            // ✅ One best option per ShippingMode
        //            .GroupBy(x => x.Dto.ShippingMode.ToLower())
        //            .Select(g => g.OrderBy(x => x.Dto.Cost).First().Dto)

        //            .ToList();

        //        return bestCourierOptions;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception in GetBestCourierOptions: {ex.Message}");
        //        Console.WriteLine(ex.StackTrace);
        //        return new List<BestcourierinfoDto>();
        //    }
        //}


        public async Task<List<BestcourierinfoDto>> GetBestCourierOptions(
    string storeCity,
    string storeState,
    string storeCountry,
    string shopperCity,
    decimal productWeightKg)
        {
            try
            {
                var data = await (
                    from branch in _context.CourierBranches
                    join service in _context.CourierBranchServices
                        on branch.BranchId equals service.BranchId
                    where branch.City.ToLower() == storeCity.ToLower()
                       && branch.State.ToLower() == storeState.ToLower()
                       && branch.Country.ToLower() == storeCountry.ToLower()
                       && !string.IsNullOrEmpty(service.Destinations)
                    select new
                    {
                        branch.BranchId,
                        service.Destinations,
                        service.ShippingMode,
                        service.Charges,
                        service.WeightRange,
                        service.EstimateDays
                    }
                )
                .AsNoTracking()
                .ToListAsync();

                var matchingCouriers = data
                    .Where(x =>
                        x.Destinations
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(d => d.Trim().ToLower())
                            .Contains(shopperCity.ToLower())
                    );

                var bestCourierOptions = matchingCouriers
                    .Select(x =>
                    {
                        var maxWeight = ExtractMaxWeight(x.WeightRange);
                        var maxDays = x.EstimateDays ?? GetMaxDeliveryDays(x.ShippingMode);

                        return new
                        {
                            Dto = new BestcourierinfoDto
                            {
                                BranchId = x.BranchId,
                                ShippingMode = x.ShippingMode,
                                Cost = x.Charges,
                                MaxDeliveryDays = maxDays,
                                DeliveryDaysRange = GetDeliveryRangeText(maxDays),
                                EstimatedDeliveryDate = GetEstimatedDeliveryDate(maxDays)
                            },
                            MaxWeight = maxWeight
                        };
                    })
                    .Where(x => x.MaxWeight >= productWeightKg)

                    // ✅ one best option per ShippingMode
                    .GroupBy(x => x.Dto.ShippingMode.ToLower())
                    .Select(g => g.OrderBy(x => x.Dto.Cost).First().Dto)
                    .ToList();

                return bestCourierOptions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetBestCourierOptions: {ex.Message}");
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

        private string GetDeliveryRangeText(int maxDays)
        {
            if (maxDays <= 1) return "1 day";
            if (maxDays == 2) return "1–2 days";
            if (maxDays == 3) return "2–3 days";
            if (maxDays == 4) return "3–4 days";
            if (maxDays == 5) return "3–5 days";
            return "5–7 days";
        }
        private int GetMaxDeliveryDays(string shippingMode)
        {
            return shippingMode.ToLower() switch
            {
                "air" => 2,
                "surface" => 5,
                _ => 7
            };
        }

        private string GetEstimatedDeliveryDate(int maxDays)
        {
            var deliveryDate = DateTime.Today.AddDays(maxDays);
            return deliveryDate.ToString("MMM dd, yyyy"); // Jan 22, 2026
        }



        public async Task<ShopperRegister?> GetShopperByIdAsync(int shopperId)
        {
            return await _context.ShopperRegisters
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShopperRegId == shopperId);
        }

        public async Task<Dictionary<int, BusinessRegister>> GetStoresByIdsAsync(List<int> storeIds)
        {
            return await _context.BusinessRegisters
                .Where(b => storeIds.Contains(b.BusRegId))
                .AsNoTracking()
                .ToDictionaryAsync(b => b.BusRegId);
        }

        public async Task<Dictionary<int, decimal>> GetStoreWeightsAsync(
     int shopperId,
     List<int> storeIds)
        {
            return await (
                from cart in _context.addtocart
                join sku in _context.Sku_ProductVariants
                    on cart.SkuId equals sku.SkuId
                where cart.ShopperRegId == shopperId
                      && storeIds.Contains(cart.BusRegId)
                      && cart.orderstatus == "cart"
                group new { cart, sku } by cart.BusRegId into g
                select new
                {
                    StoreId = g.Key,
                    TotalWeight = g.Sum(x =>
                        (x.sku.Weight ?? 0) * x.cart.ProdQty)
                }
            ).ToDictionaryAsync(x => x.StoreId, x => x.TotalWeight);
        }


        //    public async Task<CourierService> AddCourierAsync(CourierService courier)
        //{
        //    _context.CourierService.Add(courier);
        //    await _context.SaveChangesAsync();
        //    return courier;
        //}

        //public async Task<List<AssignedOrderDto>> GetAssignedOrdersByCourierIdAsync(int courierId)
        //{
        //    var result = await (from shipping in _context.ShippingDetails
        //                        join orderDetail in _context.OrderDetails on shipping.OrderDetailId equals orderDetail.OrderDetailId
        //                        join product in _context.products on orderDetail.ProductId equals product.ProductId
        //                        join order in _context.Orders on orderDetail.OrderId equals order.OrderId
        //                        join shopper in _context.ShopperRegisters on order.ShopperRegId equals shopper.ShopperRegId
        //                        join store in _context.BusinessRegisters on orderDetail.StoreId equals store.BusRegId
        //                        join branch in _context.CourierBranches on shipping.BranchId equals branch.BranchId
        //                        where branch.CourierId == courierId
        //                        select new AssignedOrderDto
        //                        {
        //                            ShippingDetailId = shipping.ShippingDetailId,
        //                            OrderId = order.OrderId,
        //                            CustomerName = shopper.Username,
        //                            CustomerPhoneNumber = shopper.PhoneNumber,
        //                            ShippingAddress = $"{shopper.Address}, {shopper.City}, {shopper.State}, {shopper.Country} - {shopper.PostalCode}",
        //                            StoreName = store.BusinessName,
        //                            ProductName = product.ProductName,
        //                           // ProductWeight = product.product_weight??0,
        //                            Quantity = orderDetail.Quantity,
        //                            ShippingType = shipping.ShippingType,
        //                            ShippingStatus = shipping.ShippingStatus,
        //                            Cost = shipping.Cost,
        //                            TrackingId = shipping.TrackingId,
        //                            EstimatedDeliveryDate = order.OrderDate.AddDays(shipping.EstimatedDays)
        //                        }).ToListAsync();

        //    return result;
        //}

    }
}
