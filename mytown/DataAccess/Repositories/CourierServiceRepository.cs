using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
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

        //Upload CVS file for courier branches

        //public async Task<List<CourierBranchCsvRowDto>> ParseAndValidateCsv(IFormFile file)
        //{
        //    var result = new List<CourierBranchCsvRowDto>();

        //    using var stream = new StreamReader(file.OpenReadStream());

        //    int rowNumber = 0;

        //    while (!stream.EndOfStream)
        //    {
        //        string line = await stream.ReadLineAsync();
        //        rowNumber++;

        //        if (rowNumber == 1) continue; // Skip header

        //        var cols = line.Split(',');

        //        var dto = new CourierBranchCsvRowDto
        //        {
        //            RowNumber = rowNumber,
        //            CourierServiceName = cols[0],
        //            Country = cols[1],
        //            State = cols[2],
        //            City = cols[3],
        //            Town = cols[4],
        //            BranchAddress = cols[5],
        //            BranchPhoneNumber = cols[6],
        //            BranchEmailId = cols[7],
        //            BranchContactPerson = cols[8],
        //            Destinations = cols[9],
        //            ShippingMode = cols[10],
        //            DistanceRange = cols[11],
        //            WeightRange = cols[12],
        //            Charges = decimal.TryParse(cols[13], out var p) ? p : 0
        //        };

        //        bool valid =
        //            // Required fields
        //            !string.IsNullOrWhiteSpace(dto.CourierServiceName) &&
        //            !string.IsNullOrWhiteSpace(dto.Country) &&
        //            !string.IsNullOrWhiteSpace(dto.State) &&
        //            !string.IsNullOrWhiteSpace(dto.City) &&
        //            !string.IsNullOrWhiteSpace(dto.Town) &&
        //            !string.IsNullOrWhiteSpace(dto.BranchAddress) &&
        //            !string.IsNullOrWhiteSpace(dto.ShippingMode) &&
        //            !string.IsNullOrWhiteSpace(dto.WeightRange) &&
        //            !string.IsNullOrWhiteSpace(dto.DistanceRange) &&

        //            // Email
        //            CsvValidationHelper.IsValidEmail(dto.BranchEmailId) &&

        //            // Phone
        //            CsvValidationHelper.IsValidPhone(dto.BranchPhoneNumber) &&

        //            // Destinations comma-separate check
        //            CsvValidationHelper.IsCommaSeparatedList(dto.Destinations) &&

        //            // Shipping mode (optional)
        //            (dto.ShippingMode.ToLower() == "air" ||
        //             dto.ShippingMode.ToLower() == "surface") &&

        //            // Charges
        //            dto.Charges > 0;

        //        dto.IsValid = valid;
        //        result.Add(dto);
        //    }

        //    return result;
        //}

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
                    continue; // Skip header

                // --- STEP 1: Split ---
                var raw = line.Split(',').ToList();

                // --- STEP 2: Fix destinations column if commas exist ---
                if (raw.Count > 14)
                {
                    // Expected 14 columns. Anything extra belongs to Destinations.
                    var destinationPieces = raw.Skip(9).Take(raw.Count - 13).ToList();

                    // Merge into one proper destinations field
                    raw[9] = string.Join(", ", destinationPieces);

                    // Fix remaining columns (ShippingMode, DistanceRange, WeightRange, Charges)
                    int total = raw.Count;
                    raw[10] = raw[total - 4];
                    raw[11] = raw[total - 3];
                    raw[12] = raw[total - 2];
                    raw[13] = raw[total - 1];

                    // Trim list back to 14 fields
                    raw = raw.Take(14).ToList();
                }

                var cols = raw.ToArray();

                // --- STEP 3: Map to DTO ---
                var dto = new CourierBranchCsvRowDto
                {
                    RowNumber = rowNumber,
                    CourierServiceName = cols[0]?.Trim(),
                    Country = cols[1]?.Trim(),
                    State = cols[2]?.Trim(),
                    City = cols[3]?.Trim(),
                    Town = cols[4]?.Trim(),
                    BranchAddress = cols[5]?.Trim(),
                    BranchPhoneNumber = cols[6]?.Trim(),
                    BranchEmailId = cols[7]?.Trim(),
                    BranchContactPerson = cols[8]?.Trim(),
                    Destinations = cols[9]?.Trim().Trim('"'),
                    ShippingMode = cols[10]?.Trim(),
                    DistanceRange = cols[11]?.Trim(),
                    WeightRange = cols[12]?.Trim(),
                    Charges = decimal.TryParse(cols[13], out var p) ? p : 0
                };

                // --- STEP 4: Validation ---
                bool valid =
                    !string.IsNullOrWhiteSpace(dto.CourierServiceName) &&
                    !string.IsNullOrWhiteSpace(dto.Country) &&
                    !string.IsNullOrWhiteSpace(dto.State) &&
                    !string.IsNullOrWhiteSpace(dto.City) &&
                    !string.IsNullOrWhiteSpace(dto.Town) &&
                    !string.IsNullOrWhiteSpace(dto.BranchAddress) &&
                    !string.IsNullOrWhiteSpace(dto.ShippingMode) &&
                    !string.IsNullOrWhiteSpace(dto.WeightRange) &&
                    !string.IsNullOrWhiteSpace(dto.DistanceRange) &&
                    CsvValidationHelper.IsValidEmail(dto.BranchEmailId) &&
                    CsvValidationHelper.IsValidPhone(dto.BranchPhoneNumber) &&
                    CsvValidationHelper.IsCommaSeparatedList(dto.Destinations) &&
                    (dto.ShippingMode.Equals("air", StringComparison.OrdinalIgnoreCase) ||
                     dto.ShippingMode.Equals("surface", StringComparison.OrdinalIgnoreCase)) &&
                    dto.Charges > 0;

                dto.IsValid = valid;
                result.Add(dto);
            }

            return result;


}



        public async Task<bool> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows)
        {
            if (rows == null || !rows.Any())
                throw new Exception("No data received.");

            if (rows.Any(r => !r.IsValid))
                throw new Exception("Some rows are invalid. Please fix them before saving.");

            // 1️⃣ Find CourierId for every row using CourierServiceName
            foreach (var r in rows)
            {
                var service = await _context.CourierService
                    .FirstOrDefaultAsync(s => s.CourierServiceName == r.CourierServiceName);

                if (service == null)
                    throw new Exception($"Courier service '{r.CourierServiceName}' does not exist. Please create courier service first.");

                r.CourierId = service.CourierId;
            }

            // 2️⃣ Check duplicates in DB
            foreach (var r in rows)
            {
                bool exists = await _context.CourierBranches.AnyAsync(cb =>
                    cb.CourierId == r.CourierId &&               // IMPORTANT
                    cb.Country == r.Country &&
                    cb.State == r.State &&
                    cb.City == r.City &&
                    cb.Town == r.Town &&
                    cb.BranchAddress == r.BranchAddress &&
                    cb.BranchPhoneNumber == r.BranchPhoneNumber &&
                    cb.BranchEmailId == r.BranchEmailId &&
                    cb.BranchContactPerson == r.BranchContactPerson &&
                    cb.ShippingMode == r.ShippingMode &&
                    cb.DistanceRange == r.DistanceRange &&
                    cb.WeightRange == r.WeightRange
                );

                if (exists)
                    throw new Exception("Duplicate rows found in database. Please remove duplicates and reupload.");
            }

            // 3️⃣ Convert and save
            var entities = rows.Select(r => new CourierBranch
            {
                CourierId = r.CourierId,                // REQUIRED ✔
                CourierServiceName = r.CourierServiceName,
                Country = r.Country,
                State = r.State,
                City = r.City,
                Town = r.Town,
                BranchAddress = r.BranchAddress,
                BranchPhoneNumber = r.BranchPhoneNumber,
                BranchEmailId = r.BranchEmailId,
                BranchContactPerson = r.BranchContactPerson,
                Destinations = r.Destinations,
                ShippingMode = r.ShippingMode,
                DistanceRange = r.DistanceRange,
                WeightRange = r.WeightRange,
                Charges = r.Charges,
                EstimateDays = r.EstimateDays,

            }).ToList();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.CourierBranches.AddRangeAsync(entities);
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
