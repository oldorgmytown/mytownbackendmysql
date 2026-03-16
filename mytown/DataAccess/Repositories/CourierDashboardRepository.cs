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

            // Status filter
            if (shippingStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                shippingStatus.Equals("NewOrders", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(sd =>
                    sd.ShippingStatus == "Pending" ||
                    sd.ShippingStatus == "ReadyToShip");
            }
            else
            {
                query = query.Where(sd => sd.ShippingStatus == shippingStatus);
            }

            // 🔎 Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(sd =>
                    sd.StoreOrder.Store.BusinessName.Contains(search) ||
                    sd.StoreOrder.Store.Town.Contains(search) ||
                    sd.StoreOrder.Store.BusMobileNo.Contains(search) ||
                    sd.TrackingId.Contains(search) ||
                    sd.StoreOrderId.ToString().Contains(search)
                );
            }

            return await query
                .OrderByDescending(sd => sd.StoreOrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sd => new CourierOrderDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    BranchId = sd.BranchId,

                    Orderdate = DateOnly.FromDateTime(
                        sd.StoreOrder.Order.OrderDate
                    ),

                    EstimatedDeliveryDate =
                        sd.StoreOrder.Order.OrderDate.AddDays(sd.EstimatedDays),

                    StoreName = sd.StoreOrder.Store.BusinessName,
                    StoreTown = sd.StoreOrder.Store.Town,
                    StoreContact = sd.StoreOrder.Store.BusMobileNo,

                    TrackingId = sd.TrackingId,
                    ShippingStatus = sd.ShippingStatus
                })
                .ToListAsync();
        }
        //branch orders
        public async Task<List<CourierOrderDto>> GetOrdersByBranchAsync(
     int branchId,
     string shippingStatus,
     string? search,
     int pageNumber = 1,
     int pageSize = 10)
        {
            var query = _context.ShippingDetails
                .Where(sd => sd.BranchId == branchId);

            // Status filter
            if (shippingStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                shippingStatus.Equals("NewOrders", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(sd =>
                    sd.ShippingStatus == "Pending" ||
                    sd.ShippingStatus == "ReadyToShip");
            }
            else
            {
                query = query.Where(sd => sd.ShippingStatus == shippingStatus);
            }

            // 🔍 Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(sd =>
                    sd.StoreOrder.Store.BusinessName.Contains(search) ||
                    sd.StoreOrder.Store.Town.Contains(search) ||
                    sd.TrackingId.Contains(search) ||
                    sd.StoreOrderId.ToString().Contains(search)
                );
            }

            return await query
                .OrderByDescending(sd => sd.StoreOrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sd => new CourierOrderDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    BranchId = sd.BranchId,

                    Orderdate = DateOnly.FromDateTime(
                        sd.StoreOrder.Order.OrderDate
                    ),

                    EstimatedDeliveryDate =
                        sd.StoreOrder.Order.OrderDate.AddDays(sd.EstimatedDays),

                    StoreName = sd.StoreOrder.Store.BusinessName,
                    StoreTown = sd.StoreOrder.Store.Town,
                    StoreContact = sd.StoreOrder.Store.BusMobileNo,

                    TrackingId = sd.TrackingId,
                    ShippingStatus = sd.ShippingStatus
                })
                .ToListAsync();
        }

        //public async Task UpdateTrackingAndStatusAsync(
        //    int storeOrderId,
        //    string trackingId,
        //    string newStatus)
        //{
        //    var shipment = await _context.ShippingDetails
        //        .FirstOrDefaultAsync(sd => sd.StoreOrderId == storeOrderId);

        //    if (shipment == null)
        //        throw new Exception("Shipment not found");

        //    shipment.TrackingId = trackingId;
        //    shipment.ShippingStatus = newStatus;

        //    //if (newStatus == "Complete")
        //    //{
        //    //    shipment.DeliveredDate = DateTime.UtcNow;
        //    //}

        //    await _context.SaveChangesAsync();
        //}

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<ShippingDetails?> GetByStoreOrderIdAsync(int storeOrderId)
        {
            return await _context.ShippingDetails
                .FirstOrDefaultAsync(s => s.StoreOrderId == storeOrderId);
        }
        public async Task<CourierOrderDetailDto> GetCourierOrderDetailAsync(int storeOrderId)
        {
            var storeOrder = await _context.StoreOrders
                .Include(so => so.Order)
                    .ThenInclude(o => o.ShopperRegister)
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

            int estimateDays = shipping?.EstimatedDays ?? 0;

            DateTime estimatedDeliveryDate =
                storeOrder.Order.OrderDate.AddDays(estimateDays);

            var productTotal = storeOrder.OrderDetails.Sum(od =>
                od.Price * od.Quantity);

            var shippingCost = shipping?.Cost ?? 0;

            return new CourierOrderDetailDto
            {
                StoreOrderId = storeOrder.StoreOrderId,
                OrderDate = storeOrder.Order.OrderDate,

                ShopperId = storeOrder.Order.ShopperRegId,
                ShopperName = storeOrder.Order.ShopperRegister.Username,
                ShopperPhone = storeOrder.Order.ShopperRegister.PhoneNumber,

                StoreId = storeOrder.StoreId,
                StoreName = storeOrder.Store.BusinessName,
                StoreTown = storeOrder.Store.Town,

                ShippingMethod = shipping?.ShippingType,
                ShippingCost = shippingCost,
                ShippingAddress = shipping?.DeliveryAddress,

                EstimatedDeliveryDate = estimatedDeliveryDate,

                CourierServiceName = shipping?.CourierBranch?.CourierService?.CourierServiceName,
                TrackingId = shipping?.TrackingId,

                Products = storeOrder.OrderDetails.Select(od => new CourierOrderProductDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    VariantId = od.SkuId,
                    VariantCost = od.Price,
                    Quantity = od.Quantity,

                    VariantImage = od.Variant.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.FileName)
                        .FirstOrDefault(),

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

        public async Task<CourierService> GetCourierWithBranchesAsync(int courierId)
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

        public async Task<int> GetTotalCompletedDeliveriesCountAsync(int courierId)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == "Delivered")
                .CountAsync();
        }

        public async Task<int> GetPendingTasksCountAsync(int courierId)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == "Ready to ship")
                .CountAsync();
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

            // 🔹 Filter by date ONLY if provided (Today’s Deliveries)
            if (date.HasValue)
            {
                var targetDate = date.Value.Date;

                query = query.Where(sd =>
                    sd.DeliveredDate.Value.Date == targetDate);
            }

            return await query
                .OrderByDescending(sd => sd.DeliveredDate)
                .Select(sd => new CourierCompletedDeliveryDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    DeliveredDate = sd.DeliveredDate.Value,
                    TrackingId = sd.TrackingId
                })
                .ToListAsync();
        }


        //  Get all unread notifications for courier
        public async Task<List<CourierDBNotifications>> GetUnreadNotificationsAsync(int courierId)
        {
            return await _context.CourierDBNotifications
                .Where(n => n.CourierId == courierId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        //  Mark all notifications as read (when dashboard opened)
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


        // here are apis for branches 

        public async Task<CourierBranchDto> GetBranchAsync(int branchId)
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

        public async Task<int> GetTotalCompletedDeliveriesCountByBranchAsync(int branchId)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.BranchId == branchId &&
                    sd.ShippingStatus == "Delivered")
                .CountAsync();
        }

        public async Task<int> GetPendingTasksCountByBranchAsync(int branchId)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.BranchId == branchId &&
                    (sd.ShippingStatus == "Pending" || sd.ShippingStatus == "ReadyToShip"))
                .CountAsync();
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

                query = query.Where(sd =>
                    sd.DeliveredDate.Value.Date == targetDate);
            }

            return await query
                .OrderByDescending(sd => sd.DeliveredDate)
                .Select(sd => new CourierCompletedDeliveryDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    DeliveredDate = sd.DeliveredDate.Value,
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

            // Upload to Azure Blob
            string fileName = await UploadToBlobAsync(file, "deliveryproof");

            // Save filename in DB
            shipping.DeliveryProofFileName = fileName;

            // Optional but recommended
            shipping.ShippingStatus = "Delivered";
            shipping.DeliveredDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return fileName;
        }

        public async Task<string> UploadToBlobAsync(IFormFile file, string imageType)
        {
            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];

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

            return newFileName; // return file name (store in DB)
        }
    }
}
