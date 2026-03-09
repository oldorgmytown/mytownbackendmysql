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

        public async Task<List<CurrentOrderDto>> GetCurrentOrdersByShopperAsync(
      int shopperRegId,
      string? search,
      int pageNumber,
      int pageSize)
        {
            var query =
                from o in _context.Orders
                join so in _context.StoreOrders on o.OrderId equals so.OrderId
                join sd in _context.ShippingDetails on so.StoreOrderId equals sd.StoreOrderId
                where o.ShopperRegId == shopperRegId
                      && sd.ShippingStatus == "In Progress"
                select new CurrentOrderDto
                {
                    StoreOrderId = so.StoreOrderId,
                    ExpectedDeliveryDate = o.OrderDate.AddDays(sd.EstimatedDays),
                    ShippingStatus = sd.ShippingStatus
                };

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.StoreOrderId.ToString().Contains(search) ||
                    x.ShippingStatus.Contains(search));
            }

            return await query
                .OrderByDescending(x => x.StoreOrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<ShopperOrderDetailsDto?> GetShopperOrderDetailsAsync(
       int storeOrderId,
       string? search,
       int pageNumber,
       int pageSize)
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
                    sd.TrackingId,
                    sd.DeliveryAddress
                }
            ).FirstOrDefaultAsync();

            if (orderData == null)
                return null;


            // ===============================
            // PRODUCTS QUERY
            // ===============================
            var productQuery =
                from od in _context.OrderDetails
                join pr in _context.products on od.ProductId equals pr.ProductId

                // Variant
                join v in _context.Sku_ProductVariants
                    on od.SkuId equals v.SkuId

                // SKU image
                join skuImg in _context.ProductImages
                    .Where(i => i.SortOrder == 1)
                    on od.SkuId equals skuImg.SkuId into skuImages
                from skuImg in skuImages.DefaultIfEmpty()

                    // Product image fallback
                join prodImg in _context.ProductImages
                    .Where(i => i.SortOrder == 1)
                    on pr.ProductId equals prodImg.ProductId into prodImages
                from prodImg in prodImages.DefaultIfEmpty()

                where od.StoreOrderId == storeOrderId

                select new OrderProductItemDto
                {
                    ProductId = pr.ProductId,
                    ProductName = pr.ProductName!,
                    SkuId = od.SkuId,

                    UnitPrice = od.Price,
                    Quantity = od.Quantity,

                    Length = v.Length,
                    Width = v.Width,
                    Height = v.Height,
                    Weight = v.Weight,

                    ProductImage = skuImg != null
                        ? skuImg.FileName
                        : prodImg != null
                            ? prodImg.FileName
                            : null
                };


            // 🔎 SEARCH (Product Name)
            if (!string.IsNullOrEmpty(search))
            {
                productQuery = productQuery.Where(p =>
                    p.ProductName.Contains(search));
            }


            // 📄 PAGINATION (Products only)
            var products = await productQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            // ===============================
            // RETURN DTO
            // ===============================
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

                ProductAmount = products.Sum(p => p.UnitPrice * p.Quantity),
                CourierAmount = orderData.Cost,

                ShippingMethod = orderData.ShippingType,
                ExpectedDeliveryDate = orderData.OrderDate.AddDays(orderData.EstimatedDays),

                CourierService = orderData.ShippingType,
                TrackingId = orderData.TrackingId,

                ShippingAddress = orderData.DeliveryAddress
            };
        }
        public async Task<List<BuyAgainProductDto>> GetBuyAgainProductsAsync(
     int shopperRegId,
     string? search,
     int pageNumber,
     int pageSize)
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
                      && pr.ProductStatus == "Approved"
                      && pr.IsActive == true
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


            // 🔎 SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.ProductName.Contains(search) ||
                    x.StoreName.Contains(search));
            }


            return await query
                .OrderByDescending(x => x.LastOrderedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<WishlistItemDto>> GetWishlistAsync(
     int shopperId,
     string? search,
     int pageNumber,
     int pageSize)
        {
            var query =
                from w in _context.Wishlist

                join p in _context.products
                    on w.ProductId equals p.ProductId

                join s in _context.BusinessRegisters
                    on w.BusRegId equals s.BusRegId

                join sku in _context.Sku_ProductVariants
                    on w.SkuId equals sku.SkuId

                join skuImg in _context.ProductImages
                    .Where(i => i.SortOrder == 1)
                    on w.SkuId equals skuImg.SkuId into skuImgJoin
                from skuImg in skuImgJoin.DefaultIfEmpty()

                join prodImg in _context.ProductImages
                    .Where(i => i.SortOrder == 1 && i.SkuId == null)
                    on p.ProductId equals prodImg.ProductId into prodImgJoin
                from prodImg in prodImgJoin.DefaultIfEmpty()

                where w.ShopperRegId == shopperId

                select new WishlistItemDto
                {
                    WishlistId = w.WishlistId,
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    SkuId = w.SkuId,
                    Buscatid = p.BuscatId,
                    prod_sub_catid = p.ProdSubcatId,

                    VariantImageUrl = skuImg.FileName ?? prodImg.FileName,

                    Price = sku.DiscountPrice ?? sku.Sku_Cost,

                    StoreId = s.BusRegId,
                    StoreName = s.BusinessName,

                    IsProductAvailable =
                        p.ProductStatus == "Approved"
                        && p.IsActive == true
                };


            // 🔎 SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.ProductName.Contains(search) ||
                    x.StoreName.Contains(search));
            }


            return await query
                .OrderByDescending(x => x.WishlistId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        //Remove from wishlist


        public async Task<bool> RemoveFromWishlistAsync(int shopperId, int productId, int skuId)
        {
            var item = await _context.Wishlist
        .FirstOrDefaultAsync(w =>
            w.ShopperRegId == shopperId &&
            w.ProductId == productId &&
            w.SkuId == skuId);

            if (item == null)
                return false;

            _context.Wishlist.Remove(item);
            await _context.SaveChangesAsync();

            return true;
        }

        // 1️⃣ Wishlist (Saved items) count
        public async Task<int> GetWishlistCountAsync(int shopperRegId)
        {
            return await _context.Wishlist
                .Where(w => w.ShopperRegId == shopperRegId)
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
                    o.OrderStatus != "Pending")
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

        public async Task<bool> UpdateShopperDetailsAsync(UpdateShopperDetailsDto dto)
        {
            var shopper = await _context.ShopperRegisters
                .FirstOrDefaultAsync(s => s.ShopperRegId == dto.ShopperRegId);

            if (shopper == null)
                return false;

            // ✅ Update ONLY edited fields
            if (!string.IsNullOrWhiteSpace(dto.Username))
                shopper.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                shopper.PhoneNumber = dto.PhoneNumber;

            if (dto.Address != null)
                shopper.Address = dto.Address;

            if (dto.Town != null)
                shopper.Town = dto.Town;

            if (dto.City != null)
                shopper.City = dto.City;

            if (dto.State != null)
                shopper.State = dto.State;

            if (dto.Country != null)
                shopper.Country = dto.Country;

            if (dto.PostalCode != null)
                shopper.PostalCode = dto.PostalCode;

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
