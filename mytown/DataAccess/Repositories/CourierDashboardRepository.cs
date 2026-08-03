using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class CourierDashboardRepository : ICourierDashboardRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CourierDashboardRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<CourierOrderDto>> GetOrdersAsync(
            int courierId,
            string shippingStatus,
            string? search,
            int pageNumber,
            int pageSize)
        {
            var query = _context.ShippingDetails
                .Where(sd => sd.CourierBranch.CourierId == courierId);

            if (shippingStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                shippingStatus.Equals("NewOrders", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(sd =>
                    sd.ShippingStatus == "Pending" ||
                    sd.ShippingStatus == "Ready to Ship");
            }
            else
            {
                query = query.Where(sd => sd.ShippingStatus == shippingStatus);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(sd =>
                    sd.StoreOrder.Store.BusinessName.Contains(search) ||
                    sd.StoreOrder.Store.Town.Contains(search) ||
                    sd.StoreOrder.Store.BusMobileNo.Contains(search) ||
                    sd.TrackingId.Contains(search) ||
                    sd.StoreOrderId.ToString().Contains(search) ||
                    sd.OrderId.ToString().Contains(search)
                );
            }

            return await query
                .OrderByDescending(sd => sd.StoreOrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sd => new CourierOrderDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    OrderId = sd.OrderId,
                    BranchId = sd.BranchId ?? 0,
                    Orderdate = DateOnly.FromDateTime(sd.StoreOrder.Order.OrderDate),
                    EstimatedDeliveryDate = sd.StoreOrder.Order.OrderDate.AddDays(sd.EstimatedDays),
                    StoreName = sd.StoreOrder.Store.BusinessName,
                    StoreTown = sd.StoreOrder.Store.Town,
                    StoreContact = sd.StoreOrder.Store.BusMobileNo,
                    TrackingId = sd.TrackingId,
                    ShippingStatus = sd.ShippingStatus,
                    DeliveredDate = sd.DeliveredDate
                })
                .ToListAsync();
        }

        public async Task<List<CourierOrderDto>> GetOrdersByBranchAsync(
            int branchId,
            string shippingStatus,
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.ShippingDetails
                .Where(sd => sd.BranchId == branchId);
            query = query.Where(sd =>
                sd.StoreOrder.Order.OrderStatus == "Paid" ||
                _context.Payments.Any(p =>
                    p.OrderId == sd.OrderId &&
                    p.PaymentStatus == "Paid"));

            if (shippingStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                shippingStatus.Equals("NewOrders", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(sd =>
                    sd.ShippingStatus == "Pending" ||
                    sd.ShippingStatus == "Ready to Ship");
            }
            else
            {
                query = query.Where(sd => sd.ShippingStatus == shippingStatus);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(sd =>
                    sd.StoreOrder.Store.BusinessName.Contains(search) ||
                    sd.StoreOrder.Store.Town.Contains(search) ||
                    sd.TrackingId.Contains(search) ||
                    sd.StoreOrderId.ToString().Contains(search) ||
                    sd.OrderId.ToString().Contains(search)
                );
            }

            return await query
                .OrderByDescending(sd => sd.StoreOrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sd => new CourierOrderDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    OrderId = sd.OrderId,
                    BranchId = sd.BranchId ?? 0,
                    Orderdate = DateOnly.FromDateTime(sd.StoreOrder.Order.OrderDate),
                    EstimatedDeliveryDate = sd.StoreOrder.Order.OrderDate.AddDays(sd.EstimatedDays),
                    StoreName = sd.StoreOrder.Store.BusinessName,
                    StoreTown = sd.StoreOrder.Store.Town,
                    StoreContact = sd.StoreOrder.Store.BusMobileNo,
                    TrackingId = sd.TrackingId,
                    ShippingStatus = sd.ShippingStatus,
                    DeliveredDate = sd.DeliveredDate
                })
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ShippingDetails?> GetByStoreOrderIdAsync(int storeOrderId)
        {
            return await _context.ShippingDetails
                .Include(s => s.CourierBranch)
                .FirstOrDefaultAsync(s => s.StoreOrderId == storeOrderId);
        }

        // Return type is nullable because storeOrder may not be found
        public async Task<CourierOrderDetailDto?> GetCourierOrderDetailAsync(int storeOrderId)
        {
            var storeOrder = await _context.StoreOrders
                .Include(so => so.Order)
                    .ThenInclude(o => o.ShopperRegister)
                .Include(so => so.Order)
                    .ThenInclude(o => o.GuestRegister)
                .Include(so => so.Store)
                .Include(so => so.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(so => so.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Images)
                .FirstOrDefaultAsync(so => so.StoreOrderId == storeOrderId);

            if (storeOrder == null)
                return null;

            var shipping = await _context.ShippingDetails
                .Include(sd => sd.CourierBranch)
                    .ThenInclude(cb => cb.CourierService)
                .FirstOrDefaultAsync(sd => sd.StoreOrderId == storeOrderId);

            var package = await _context.ShippingPackageDetails
                .FirstOrDefaultAsync(p => p.StoreOrderId == storeOrderId);

            int estimateDays = shipping?.EstimatedDays ?? 0;
            DateTime estimatedDeliveryDate = storeOrder.Order.OrderDate.AddDays(estimateDays);

            var productTotal = storeOrder.OrderDetails.Sum(od => od.Price * od.Quantity);
            var shippingCost = shipping?.Cost ?? 0;

            string customerName;
            string customerPhone;
            int? shopperId = null;
            int? guestRegId = null;

            if (storeOrder.Order.IsGuestOrder)
            {
                guestRegId = storeOrder.Order.GuestRegId;
                customerName = storeOrder.Order.GuestRegister?.Username ?? "";
                customerPhone = storeOrder.Order.GuestRegister?.PhoneNumber ?? "";
            }
            else
            {
                shopperId = storeOrder.Order.ShopperRegId;
                customerName = storeOrder.Order.ShopperRegister?.Username ?? "";
                customerPhone = storeOrder.Order.ShopperRegister?.PhoneNumber ?? "";
            }

            return new CourierOrderDetailDto
            {
                StoreOrderId = storeOrder.StoreOrderId,
                OrderId = storeOrder.OrderId,
                OrderDate = storeOrder.Order.OrderDate,

                ShopperId = shopperId,
                GuestRegId = guestRegId,
                IsGuestOrder = storeOrder.Order.IsGuestOrder,

                CustomerName = customerName,
                CustomerPhone = customerPhone,

                StoreId = storeOrder.StoreId,
                StoreName = storeOrder.Store.BusinessName,

                StoreTown =
                    (storeOrder.Store.Address1 ?? "") + ", " +
                    (storeOrder.Store.Town ?? "") + ", " +
                    (storeOrder.Store.BusinessCity ?? "") + ", " +
                    (storeOrder.Store.BusinessState ?? "") + ", " +
                    (storeOrder.Store.BusinessCountry ?? ""),

                // shipping may be null — fall back to empty string for required string properties
                ShippingMethod = shipping?.ShippingType ?? "",
                ShippingCost = shippingCost,
                ShippingAddress = shipping?.DeliveryAddress ?? "",
                ShippingStatus = shipping?.ShippingStatus ?? "",

                EstimatedDeliveryDate = estimatedDeliveryDate,

                CourierServiceName = shipping?.CourierBranch?.CourierService?.CourierServiceName ?? "",
                TrackingId = shipping?.TrackingId ?? "",

                PackageLength = package?.PackageLength,
                PackageWidth = package?.PackageWidth,
                PackageHeight = package?.PackageHeight,
                PackageWeight = package?.PackageWeight,

                DimensionUnit = package?.DimensionUnit ?? "",
                WeightUnit = package?.WeightUnit ?? "",

                Products = storeOrder.OrderDetails.Select(od => new CourierOrderProductDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    VariantId = od.SkuId,
                    VariantCost = od.Price,
                    Quantity = od.Quantity,

                    // Images is guarded by Include; FirstOrDefault() returns null → fall back to ""
                    VariantImage = od.Variant.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.FileName)
                        .FirstOrDefault() ?? "",

                    Weight = od.Variant.Weight,
                    Length = od.Variant.Length,
                    Width = od.Variant.Width,
                    Height = od.Variant.Height
                }).ToList(),

                TotalProductAmount = productTotal,
                TotalShippingAmount = shippingCost,
                FinalTotalAmount = productTotal + shippingCost
            };
        }

        // Return type is nullable because FirstOrDefaultAsync can return null
        public async Task<CourierService?> GetCourierWithBranchesAsync(int courierId)
        {
            return await _context.CourierService
                .Include(c => c.CourierBranches)
                .FirstOrDefaultAsync(c => c.CourierId == courierId);
        }

        public async Task<int> GetCompletedDeliveriesCountAsync(int courierId, DateTime date)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == "Delivered" &&
                    sd.DeliveredDate.HasValue &&
                    sd.DeliveredDate.Value.Date == date)
                .CountAsync();
        }

        public async Task<int> GetTotalCompletedDeliveriesCountAsync(
            int courierId,
            int? month,
            int? year,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.ShippingDetails
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == "Delivered");

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(sd =>
                    sd.DeliveredDate.HasValue &&
                    sd.DeliveredDate.Value.Date >= fromDate.Value.Date &&
                    sd.DeliveredDate.Value.Date <= toDate.Value.Date);
            }
            else if (month.HasValue && year.HasValue)
            {
                query = query.Where(sd =>
                    sd.DeliveredDate.HasValue &&
                    sd.DeliveredDate.Value.Month == month.Value &&
                    sd.DeliveredDate.Value.Year == year.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetPendingTasksCountAsync(
            int courierId,
            int? month,
            int? year,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.ShippingDetails
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    (sd.ShippingStatus == "Ready to ship" ||
                     sd.ShippingStatus == "Pending"));

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(sd =>
                    sd.Order.OrderDate.Date >= fromDate.Value.Date &&
                    sd.Order.OrderDate.Date <= toDate.Value.Date);
            }
            else if (month.HasValue && year.HasValue)
            {
                query = query.Where(sd =>
                    sd.Order.OrderDate.Month == month.Value &&
                    sd.Order.OrderDate.Year == year.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesAsync(
            int courierId,
            DateTime? date)
        {
            var query = _context.ShippingDetails
                .Include(sd => sd.CourierBranch)
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == "Delivered" &&
                    sd.DeliveredDate.HasValue
                );

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(sd => sd.DeliveredDate!.Value.Date == targetDate);
            }

            return await query
                .OrderByDescending(sd => sd.DeliveredDate)
                .Select(sd => new CourierCompletedDeliveryDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    // DeliveredDate.HasValue is guaranteed by the Where above
                    DeliveredDate = sd.DeliveredDate!.Value,
                    TrackingId = sd.TrackingId
                })
                .ToListAsync();
        }

        public async Task<List<CourierDBNotifications>> GetUnreadNotificationsAsync(int courierId)
        {
            return await _context.CourierDBNotifications
                .Where(n => n.CourierId == courierId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task MarkNotificationsAsReadAsync(int courierId)
        {
            var unreadNotifications = await _context.CourierDBNotifications
                .Where(n => n.CourierId == courierId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<BranchBasicDto>> GetBasicBranches(int courierId)
        {
            return await _context.CourierBranches
                .Where(b => b.CourierId == courierId && b.IsActive)
                .Select(b => new BranchBasicDto
                {
                    BranchId = b.BranchId,
                    Town = b.Town,
                    Country = b.Country
                })
                .ToListAsync();
        }

        // Return type is nullable — caller must handle null (branch not found)
        public async Task<CourierBranchDto?> GetBranchAsync(int branchId)
        {
            return await _context.CourierBranches
                .Where(b => b.BranchId == branchId)
                .Select(b => new CourierBranchDto
                {
                    BranchId = b.BranchId,
                    CourierBranchName = b.CourierServiceName,
                    City = b.City,
                    State = b.State,
                    Town = b.Town,
                    BranchAddress = b.BranchAddress,
                    BranchPhoneNumber = b.BranchPhoneNumber,
                    BranchEmail = b.BranchEmailId,
                    Services = b.Services.Select(s => new CourierBranchServiceDto
                    {
                        BranchServiceId = s.BranchServiceId,
                        Destinations = s.Destinations,
                        ShippingMode = s.ShippingMode,
                        DistanceRange = s.DistanceRange,
                        WeightRange = s.WeightRange,
                        Charges = s.Charges,
                        EstimateDays = s.EstimateDays
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetCompletedDeliveriesCountByBranchAsync(int branchId, DateTime date)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.BranchId == branchId &&
                    sd.ShippingStatus == "Delivered" &&
                    sd.DeliveredDate.HasValue &&
                    sd.DeliveredDate.Value.Date == date.Date)
                .CountAsync();
        }

        public async Task<int> GetTotalCompletedDeliveriesCountByBranchAsync(
            int branchId,
            int? month,
            int? year,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.ShippingDetails
                .Where(sd =>
                    sd.BranchId == branchId &&
                    sd.ShippingStatus == "Delivered");

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(sd =>
                    sd.DeliveredDate.HasValue &&
                    sd.DeliveredDate.Value.Date >= fromDate.Value.Date &&
                    sd.DeliveredDate.Value.Date <= toDate.Value.Date);
            }
            else if (month.HasValue && year.HasValue)
            {
                query = query.Where(sd =>
                    sd.DeliveredDate.HasValue &&
                    sd.DeliveredDate.Value.Month == month.Value &&
                    sd.DeliveredDate.Value.Year == year.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetPendingTasksCountByBranchAsync(
            int branchId,
            int? month,
            int? year,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.ShippingDetails
                .Where(sd =>
                    sd.BranchId == branchId &&
                    (sd.ShippingStatus == "Pending" ||
                     sd.ShippingStatus == "Ready to ship"));

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(sd =>
                    sd.Order.OrderDate.Date >= fromDate.Value.Date &&
                    sd.Order.OrderDate.Date <= toDate.Value.Date);
            }
            else if (month.HasValue && year.HasValue)
            {
                query = query.Where(sd =>
                    sd.Order.OrderDate.Month == month.Value &&
                    sd.Order.OrderDate.Year == year.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<CourierCompletedDeliveryDto>> GetCompletedDeliveriesByBranchAsync(
            int branchId,
            DateTime? date)
        {
            var query = _context.ShippingDetails
                .Where(sd =>
                    sd.BranchId == branchId &&
                    sd.ShippingStatus == "Delivered" &&
                    sd.DeliveredDate.HasValue);

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(sd => sd.DeliveredDate!.Value.Date == targetDate);
            }

            return await query
                .OrderByDescending(sd => sd.DeliveredDate)
                .Select(sd => new CourierCompletedDeliveryDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    // HasValue guaranteed by the Where above
                    DeliveredDate = sd.DeliveredDate!.Value,
                    TrackingId = sd.TrackingId
                })
                .ToListAsync();
        }

        public async Task<List<CourierDBNotifications>> GetUnreadNotificationsByBranchAsync(int branchId)
        {
            return await _context.CourierDBNotifications
                .Where(n => n.BranchId == branchId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task MarkEachNotificationReadAsync(int notificationId)
        {
            var notification = await _context.CourierDBNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> UploadDeliveryProofAsync(int storeOrderId, IFormFile file)
        {
            var shipping = await _context.ShippingDetails
                .FirstOrDefaultAsync(x => x.StoreOrderId == storeOrderId);

            if (shipping == null)
                throw new Exception("Shipping record not found");

            string fileName = await UploadToBlobAsync(file, "deliveryproof");

            shipping.DeliveryProofFileName = fileName;
            shipping.ShippingStatus = "Delivered";
            shipping.DeliveredDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return fileName;
        }

        public async Task<string> UploadToBlobAsync(IFormFile file, string imageType)
        {
            var containerName = _configuration["AzureBlobStorage:ContainerName"] ?? "";
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"] ?? "";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var newFileName = $"{imageType}_{fileNameWithoutExtension}_{timestamp}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(newFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return newFileName;
        }

        public async Task<CourierService?> GetCourierByIdAsync(int courierId)
        {
            return await _context.CourierService
                .FirstOrDefaultAsync(c => c.CourierId == courierId);
        }

        public async Task UpdateCourierAsync(CourierService courier)
        {
            _context.CourierService.Update(courier);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateCourierAccountDetailsAsync(
    int courierId,
    UpdateCourierAccountDetailDto dto)
        {
            var account = await _context.CourierAccountDetails
                .FirstOrDefaultAsync(x => x.CourierId == courierId);

            if (account == null)
            {
                account = new CourierAccountDetail
                {
                    CourierId = courierId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.CourierAccountDetails.Add(account);
            }

            account.AccountHolderName = dto.AccountHolderName;
            account.BankName = dto.BankName;
            account.AccountNumber = dto.AccountNumber;
            account.IFSCCode = dto.IFSCCode;
            account.IsTermsAccepted = dto.IsTermsAccepted;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}