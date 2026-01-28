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

        public CourierDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CourierOrderDto>> GetOrdersAsync(
        int courierId,
        string shippingStatus)
        {
            return await _context.ShippingDetails
                .Include(sd => sd.CourierBranch)
                .Include(sd => sd.Order)
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == shippingStatus
                )
                .Select(sd => new CourierOrderDto
                {
                    StoreOrderId = sd.StoreOrderId,
                    TrackingId = sd.TrackingId,
                    ShippingStatus = sd.ShippingStatus,
                    EstimatedDeliveryDate =
                        sd.Order.OrderDate.AddDays(sd.EstimatedDays)
                })
                .OrderByDescending(x => x.StoreOrderId)
                .ToListAsync();
        }

        public async Task UpdateTrackingAndStatusAsync(
            int storeOrderId,
            string trackingId,
            string newStatus)
        {
            var shipment = await _context.ShippingDetails
                .FirstOrDefaultAsync(sd => sd.StoreOrderId == storeOrderId);

            if (shipment == null)
                throw new Exception("Shipment not found");

            shipment.TrackingId = trackingId;
            shipment.ShippingStatus = newStatus;

            //if (newStatus == "Complete")
            //{
            //    shipment.DeliveredDate = DateTime.UtcNow;
            //}

            await _context.SaveChangesAsync();
        }

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
                .Include(so => so.Order.ShippingDetails)
                    .ThenInclude(sd => sd.CourierBranch)
                        .ThenInclude(cb => cb.CourierService)
                .FirstOrDefaultAsync(so => so.StoreOrderId == storeOrderId);

            if (storeOrder == null)
                return null;

            var shipping = storeOrder.Order.ShippingDetails
                .FirstOrDefault(sd => sd.StoreOrderId == storeOrderId);

            // 🔹 Estimate days logic
            int estimateDays =
                shipping?.CourierBranch?.EstimateDays
                ?? shipping?.EstimatedDays
                ?? 0;

            // 🔹 Estimated delivery date
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

                // ✅ CALCULATED
                EstimatedDeliveryDate = estimatedDeliveryDate,

                // ✅ FROM COURIER SERVICE
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
                        .Where(i => i.SortOrder == 1)
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
                    sd.ShippingStatus == "Complete" &&
                    sd.DeliveredDate.HasValue &&
                    sd.DeliveredDate.Value.Date == date)
                .CountAsync();
        }

        public async Task<int> GetTotalCompletedDeliveriesCountAsync(int courierId)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == "Complete")
                .CountAsync();
        }

        public async Task<int> GetPendingTasksCountAsync(int courierId)
        {
            return await _context.ShippingDetails
                .Where(sd =>
                    sd.CourierBranch.CourierId == courierId &&
                    sd.ShippingStatus == "Need to be shipped")
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
                    sd.ShippingStatus == "Complete" &&
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
    }
}
