using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;


namespace mytown.DataAccess.Repositories
{
    public class ShopperDashboardRepository : IShopperDashboardRepository
    {
        private readonly AppDbContext _context;

        public ShopperDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CurrentOrderDto>> GetCurrentOrdersByShopperAsync(int shopperRegId)
        {
            var query =
                from o in _context.Orders
                join so in _context.StoreOrders
                    on o.OrderId equals so.OrderId
                join sd in _context.ShippingDetails
                    on so.StoreOrderId equals sd.StoreOrderId
                where o.ShopperRegId == shopperRegId
                      && sd.ShippingStatus != "In Progress"
                orderby o.OrderDate descending
                select new CurrentOrderDto
                {
                    StoreOrderId = so.StoreOrderId,
                    ExpectedDeliveryDate = o.OrderDate.AddDays(sd.EstimatedDays),
                    ShippingStatus = sd.ShippingStatus
                };

            return await query.ToListAsync();
        }

        public async Task<ShopperOrderDetailsDto> GetShopperOrderDetailsAsync(int storeOrderId)
        {
            var orderData = await (
                from so in _context.StoreOrders
                join o in _context.Orders on so.OrderId equals o.OrderId
                join sd in _context.ShippingDetails on so.StoreOrderId equals sd.StoreOrderId
                join p in _context.Payments on o.OrderId equals p.OrderId
                join s in _context.BusinessRegisters on so.StoreId equals s.BusRegId
                where so.StoreOrderId == storeOrderId
                select new
                {
                    so.StoreOrderId,
                    p.PaymentId,
                    o.OrderDate,
                    o.ShopperRegId,
                    so.StoreId,
                    StoreName = s.BusinessName,
                    StoreTown = s.Town,
                    o.ShippingType,
                    sd.Cost,
                    sd.EstimatedDays,
                    sd.ShippingStatus,
                    sd.TrackingId,
                    sd.DeliveryAddress
                }
            ).FirstOrDefaultAsync();

            if (orderData == null)
                return null;

            var products = await (
                from od in _context.OrderDetails
                join pr in _context.products on od.ProductId equals pr.ProductId
                where od.StoreOrderId == storeOrderId
                select new OrderProductItemDto
                {
                    ProductId = pr.ProductId,
                    ProductName = pr.ProductName
                }
            ).ToListAsync();

            var productAmount = await _context.OrderDetails
                .Where(x => x.StoreOrderId == storeOrderId)
                .SumAsync(x => x.Price * x.Quantity);

            return new ShopperOrderDetailsDto
            {
                StoreOrderId = orderData.StoreOrderId,
                TransactionId = orderData.PaymentId,
                OrderDate = orderData.OrderDate,
                ShopperId = orderData.ShopperRegId,

                StoreId = orderData.StoreId,
                StoreName = orderData.StoreName,
                StoreTown = orderData.StoreTown,

                Products = products,

                ProductAmount = productAmount,
                CourierAmount = orderData.Cost,

                ShippingMethod = orderData.ShippingType,
                ExpectedDeliveryDate = orderData.OrderDate.AddDays(orderData.EstimatedDays),

                CourierService = orderData.ShippingType,
                TrackingId = orderData.TrackingId,

                ShippingAddress = orderData.DeliveryAddress // plug your logic here
            };
        }

        public async Task<List<BuyAgainProductDto>> GetBuyAgainProductsAsync(int shopperRegId)
        {
            var query =
                from o in _context.Orders
                join pay in _context.Payments
                    on o.OrderId equals pay.OrderId
                join od in _context.OrderDetails
                    on o.OrderId equals od.OrderId
                join pr in _context.products
                    on od.ProductId equals pr.ProductId
                join sku in _context.Sku_ProductVariants
                    on od.SkuId equals sku.SkuId
                join s in _context.BusinessRegisters
                    on od.StoreId equals s.BusRegId
                where o.ShopperRegId == shopperRegId
                      && pay.PaymentStatus == "Paid"
                group new { o, od, pr, sku, s } by new
                {
                    od.ProductId,
                    od.SkuId,
                    od.StoreId,
                    pr.ProductName,
                    pr.ProductImage,
                    s.BusinessName
                }
                into g
                select new BuyAgainProductDto
                {
                    ProductId = g.Key.ProductId,
                    SkuId = g.Key.SkuId,

                    ProductName = g.Key.ProductName,

                    // ✅ FIRST IMAGE LOGIC
                    VariantImage =
                        g.SelectMany(x => x.sku.Images)
                         .OrderBy(i => i.SortOrder)
                         .ThenBy(i => i.ImageId)
                         .Select(i => i.FileName)
                         .FirstOrDefault()
                        ?? g.Key.ProductImage,

                    StoreId = g.Key.StoreId,
                    StoreName = g.Key.BusinessName,

                    LastOrderedOn = g.Max(x => x.o.OrderDate),

                    Price = g.OrderByDescending(x => x.o.OrderDate)
                             .Select(x => x.od.Price)
                             .First(),

                    Quantity = g.OrderByDescending(x => x.o.OrderDate)
                                .Select(x => x.od.Quantity)
                                .First()
                };

            return await query
                .OrderByDescending(x => x.LastOrderedOn)
                .ToListAsync();
        }


        public async Task<List<WishlistItemDto>> GetWishlistAsync(int shopperId)
        {
            return await _context.OrderDetails
                .Where(od =>
                    od.Order.ShopperRegId == shopperId &&
                    od.Order.OrderStatus == "Wishlist")
                .Select(od => new WishlistItemDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    VariantImageUrl = od.Product.ProductImage,
                    Price = od.Price,
                    StoreId = od.StoreId,
                    StoreName = od.Store.BusinessName,
                    
                })
                .OrderByDescending(x => x.AddedOn)
                .ToListAsync();
        }


        // 1️⃣ Wishlist (Saved items) count
        public async Task<int> GetWishlistCountAsync(int shopperRegId)
        {
            return await _context.OrderDetails
                .Where(od =>
                    od.Order.ShopperRegId == shopperRegId &&
                    od.Order.OrderStatus == "Wishlist")
                .CountAsync();
        }

        // 2️⃣ Current orders count (distinct orders)
        public async Task<int> GetCurrentOrdersCountAsync(int shopperRegId)
        {
            return await (
                from o in _context.Orders
                join so in _context.StoreOrders
                    on o.OrderId equals so.OrderId
                join sd in _context.ShippingDetails
                    on so.StoreOrderId equals sd.StoreOrderId
                where o.ShopperRegId == shopperRegId
                      && sd.ShippingStatus != "In Progress"
                select o.OrderId
            )
            .Distinct()
            .CountAsync();
        }

        // 3️⃣ Total orders till date (excluding wishlist)
        public async Task<int> GetTotalOrdersCountAsync(int shopperRegId)
        {
            return await _context.Orders
                .Where(o =>
                    o.ShopperRegId == shopperRegId &&
                    o.OrderStatus != "Wishlist")
                .CountAsync();
        }


        public async Task<List<ShopperDBOrderHistoryDto>> GetOrderHistoryByShopperAsync(int shopperRegId)
        {
            return await (
                from o in _context.Orders
                join so in _context.StoreOrders
                    on o.OrderId equals so.OrderId
                join sd in _context.ShippingDetails
                    on so.StoreOrderId equals sd.StoreOrderId
                where o.ShopperRegId == shopperRegId
                      && sd.DeliveredDate != null
                orderby sd.DeliveredDate descending
                select new ShopperDBOrderHistoryDto
                {
                    StoreOrderId = so.StoreOrderId,
                    DeliveredDate = sd.DeliveredDate,
                    ShippingStatus = sd.ShippingStatus
                }
            ).ToListAsync();
        }

        //Get shopper profile details
        public async Task<ShopperDetailsDto?> GetShopperDetailsAsync(int shopperRegId)
        {
            return await _context.ShopperRegisters
                .Where(s => s.ShopperRegId == shopperRegId)
                .Select(s => new ShopperDetailsDto
                {
                    ShopperRegId = s.ShopperRegId,
                    Username = s.Username,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,

                    Address = s.Address,
                    Town = s.Town,
                    City = s.City,
                    State = s.State,
                    Country = s.Country,
                    PostalCode = s.PostalCode,

                    Status = s.Status,
                    ShopperRegDate = s.ShopperRegDate
                })
                .FirstOrDefaultAsync();
        }

    }
}
