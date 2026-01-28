using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;


public class BusinessDashboardRepository : IBusinessDashboardRepository
{
    private readonly AppDbContext _context;

    public BusinessDashboardRepository(AppDbContext context)
    {
        _context = context;
    }


    // get sales history
    //public async Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId)
    //{
    //    var query = from od in _context.OrderDetails
    //                join o in _context.Orders on od.OrderId equals o.OrderId
    //                join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
    //                join p in _context.products on od.ProductId equals p.product_id
    //                join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
    //                from payment in payJoin.DefaultIfEmpty() // LEFT JOIN
    //                join sd in _context.ShippingDetails on od.OrderDetailId equals sd.OrderDetailId into sdJoin
    //                from shipping in sdJoin.DefaultIfEmpty() // LEFT JOIN
    //                where od.StoreId == storeId
    //                select new BusinessDashboardDto
    //                {
    //                    OrderId = o.OrderId,
    //                    OrderDetailId = od.OrderDetailId,
    //                    OrderDate = o.OrderDate,
    //                    CustomerName = s.Username,
    //                    ProductName = p.product_name,
    //                    Quantity = od.Quantity,
    //                    Amount = od.Quantity * od.Price,
    //                    PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
    //                    Address = s.Address,
    //                    Town = s.Town,
    //                    City = s.City,
    //                    State = s.State,
    //                    Country = s.Country,
    //                    DeliveryType = shipping != null ? shipping.Shipping_type : "Not Shipped",
    //                    DeliveryStatus = o.OrderStatus
    //                };

    //    return await query.ToListAsync();
    //}



    //public async Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId)
    //{
    //    var query = from od in _context.OrderDetails
    //                join o in _context.Orders on od.OrderId equals o.OrderId
    //                join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
    //                join p in _context.products on od.ProductId equals p.ProductId
    //                join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
    //                from payment in payJoin.DefaultIfEmpty()
    //                join sd in _context.ShippingDetails  on od.OrderDetailId equals sd.OrderDetailId into sdJoin
    //                from shipping in sdJoin.DefaultIfEmpty()
    //                where od.StoreId == storeId && o.OrderStatus == "Paid"
    //                select new BusinessDashboardDto
    //                {
    //                    OrderId = o.OrderId,
    //                    OrderDetailId = od.OrderDetailId,
    //                    OrderDate = o.OrderDate,
    //                    CustomerName = s.Username,
    //                    ShopperId = s.ShopperRegId,
    //                    ProductId = p.ProductId,
    //                    ProductName = p.ProductName,
    //                    Quantity = od.Quantity,
    //                    Amount = od.Quantity * od.Price,
    //                    PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
    //                    TransactionId = payment.PaymentId,
    //                    Address = s.Address,
    //                    Town = s.Town,
    //                    City = s.City,
    //                    State = s.State,
    //                    Country = s.Country,
    //                    DeliveryType = shipping != null ? shipping.ShippingType : "Standard",
    //                    ShippingStatus = shipping != null ? shipping.ShippingStatus : "Not Shipped"
    //                };

    //    var result = await query.ToListAsync();

    //    // Categorize paid orders based on shipping status and date
    //    foreach (var order in result)
    //    {
    //        order.OrderCategory =
    //            (order.OrderDate >= DateTime.UtcNow.AddDays(-2) && order.ShippingStatus == "Not Shipped") ? "New" :
    //            (order.ShippingStatus == "Not Shipped") ? "Pending" :
    //            (order.ShippingStatus == "In Transit") ? "In Progress" :
    //            (order.ShippingStatus == "Delivered") ? "Completed" :
    //            "Other";
    //    }

    //    return result;
    //}

    // orders sales history with sort and search
    //public async Task<List<BusinessDashboardDto>> GetStoreOrdersReportsortsearch(
    // int storeId,
    // string? search = null,
    // string? sortBy = null,
    // bool descending = false)
    //{
    //    var rawQuery = from od in _context.OrderDetails
    //                   join o in _context.Orders on od.OrderId equals o.OrderId
    //                   join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
    //                   join p in _context.products on od.ProductId equals p.ProductId
    //                   join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
    //                   from payment in payJoin.DefaultIfEmpty()
    //                   join sd in _context.ShippingDetails on od.OrderDetailId equals sd.OrderDetailId into sdJoin
    //                   from shipping in sdJoin.DefaultIfEmpty()
    //                   where od.StoreId == storeId && o.OrderStatus == "Paid"
    //                   select new BusinessDashboardDto
    //                   {
    //                       OrderId = o.OrderId,
    //                       OrderDetailId = od.OrderDetailId,
    //                       OrderDate = o.OrderDate,
    //                       CustomerName = s.Username,
    //                       ShopperId = s.ShopperRegId,
    //                       ProductId = p.ProductId,
    //                       ProductName = p.ProductName,
    //                       Quantity = od.Quantity,
    //                       Amount = od.Quantity * od.Price,
    //                       PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
    //                       TransactionId = payment.PaymentId,
    //                       Address = s.Address,
    //                       Town = s.Town,
    //                       City = s.City,
    //                       State = s.State,
    //                       Country = s.Country,
    //                       DeliveryType = shipping != null ? shipping.ShippingType : "Standard",
    //                       ShippingStatus = shipping != null ? shipping.ShippingStatus : "Not Shipped",

    //                   };

    //    // Materialize data first (to memory)
    //    var result = await rawQuery.ToListAsync();

    //    // In-memory search
    //    if (!string.IsNullOrWhiteSpace(search))
    //    {
    //        search = search.ToLower();
    //        result = result.Where(q =>
    //            q.OrderId.ToString().Contains(search) ||
    //            q.OrderDetailId.ToString().Contains(search) ||
    //            q.OrderDate.ToString("yyyy-MM-dd").Contains(search) ||
    //            q.CustomerName.ToLower().Contains(search) ||
    //            q.ShopperId.ToString().Contains(search) ||
    //            q.ProductId.ToString().Contains(search) ||
    //            q.ProductName.ToLower().Contains(search) ||
    //            q.Quantity.ToString().Contains(search) ||
    //            q.Amount.ToString().Contains(search) ||
    //            q.PaymentStatus.ToLower().Contains(search) ||
    //            (q.TransactionId != null && q.TransactionId.ToString().Contains(search)) ||
    //            q.Address.ToLower().Contains(search) ||
    //            q.Town.ToLower().Contains(search) ||
    //            q.City.ToLower().Contains(search) ||
    //            q.State.ToLower().Contains(search) ||
    //            q.Country.ToLower().Contains(search) ||
    //            q.DeliveryType.ToLower().Contains(search) ||
    //            q.ShippingStatus.ToLower().Contains(search)
    //        ).ToList();
    //    }

    //    // In-memory sort
    //    // In-memory sort
    //    result = sortBy?.ToLower() switch
    //    {
    //        "orderid" => descending ? result.OrderByDescending(q => q.OrderId).ToList() : result.OrderBy(q => q.OrderId).ToList(),
    //        "orderdate" => descending ? result.OrderByDescending(q => q.OrderDate).ToList() : result.OrderBy(q => q.OrderDate).ToList(),
    //        "customername" => descending ? result.OrderByDescending(q => q.CustomerName).ToList() : result.OrderBy(q => q.CustomerName).ToList(),
    //        "productname" => descending ? result.OrderByDescending(q => q.ProductName).ToList() : result.OrderBy(q => q.ProductName).ToList(),
    //        "quantity" => descending ? result.OrderByDescending(q => q.Quantity).ToList() : result.OrderBy(q => q.Quantity).ToList(),
    //        "amount" => descending ? result.OrderByDescending(q => q.Amount).ToList() : result.OrderBy(q => q.Amount).ToList(),
    //        "transactionid" => descending ? result.OrderByDescending(q => q.TransactionId).ToList() : result.OrderBy(q => q.TransactionId).ToList(),
    //        "productid" => descending ? result.OrderByDescending(q => q.ProductId).ToList() : result.OrderBy(q => q.ProductId).ToList(),
    //        "orderdetailid" => descending ? result.OrderByDescending(q => q.OrderDetailId).ToList() : result.OrderBy(q => q.OrderDetailId).ToList(),
    //        "shopperid" => descending ? result.OrderByDescending(q => q.ShopperId).ToList() : result.OrderBy(q => q.ShopperId).ToList(),
    //        _ => result
    //    };

    //    // Categorize orders
    //    foreach (var order in result)
    //    {
    //        order.OrderCategory =
    //            (order.OrderDate >= DateTime.UtcNow.AddDays(-2) && order.ShippingStatus == "Not Shipped") ? "New" :
    //            (order.ShippingStatus == "Not Shipped") ? "Pending" :
    //            (order.ShippingStatus == "In Transit") ? "In Progress" :
    //            (order.ShippingStatus == "Delivered") ? "Completed" :
    //            "Other";
    //    }

    //    return result;
    //}





    // get order, sales, product, customer count
    public async Task<SalesReportDTO> GetSalesReportByStoreId(int storeId)
    {
        var reportData = await (from od in _context.OrderDetails
                                join o in _context.Orders on od.OrderId equals o.OrderId
                                where od.StoreId == storeId
                                select new
                                {
                                    od.Quantity,
                                    od.Price,
                                    od.OrderId,
                                    o.ShopperRegId
                                }).ToListAsync();

        if (!reportData.Any())
        {
            return new SalesReportDTO
            {
                TotalSales = 0,
                TotalProductsSold = 0,
                UniqueOrdersCount = 0,
                UniqueShoppersCount = 0
            };
        }

        return new SalesReportDTO
        {
            TotalSales = reportData.Sum(x => x.Quantity * x.Price),
            TotalProductsSold = reportData.Sum(x => x.Quantity),
            UniqueOrdersCount = reportData.Select(x => x.OrderId).Distinct().Count(),
            UniqueShoppersCount = reportData.Select(x => x.ShopperRegId).Distinct().Count()
        };
    }

    // to get location counts - tiwns, cities, states, country
    public async Task<LocationStatsDto> GetLocationCountsByStoreIdAsync(int storeId)
    {
        var query = from od in _context.OrderDetails
                    where od.StoreId == storeId
                    join o in _context.Orders on od.OrderId equals o.OrderId
                    join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
                    select s;

        var uniqueShoppers = await query.Distinct().ToListAsync();

        var result = new LocationStatsDto
        {
            TownCount = uniqueShoppers.Select(x => x.Town).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count(),
            CityCount = uniqueShoppers.Select(x => x.City).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count(),
            StateCount = uniqueShoppers.Select(x => x.State).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count(),
            CountryCount = uniqueShoppers.Select(x => x.Country).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count()
        };

        return result;
    }


    // show products tab data
    //public async Task<List<ProductDto>> GetProductsWithPurchasedCountAsync(int busRegId, string searchText = null, string sortBy = "id", string sortDirection = "asc", int page = 1, int pageSize = 10)
    //{
    //    var query = from p in _context.products
    //                where p.BusRegId == busRegId
    //                // Left join orderdetails on product id and store id
    //                join od in _context.OrderDetails.Where(od => od.StoreId == busRegId)
    //                    on p.ProductId equals od.ProductId into odGroup
    //                select new
    //                {
    //                    Product = p,
    //                    PurchasedCount = odGroup.Sum(x => (int?)x.Quantity) ?? 0
    //                };

    //    // Apply search filter
    //    if (!string.IsNullOrEmpty(searchText))
    //    {
    //        query = query.Where(x =>
    //            x.Product.product_name.Contains(searchText) ||
    //            x.Product.product_subject.Contains(searchText) ||
    //            x.Product.product_id.ToString().Contains(searchText)
    //        );
    //    }

    //    // Sorting
    //    bool isAsc = sortDirection.ToLower() == "asc";
    //    query = sortBy?.ToLower() switch
    //    {
    //        "price" => isAsc ? query.OrderBy(x => x.Product.product_cost) : query.OrderByDescending(x => x.Product.product_cost),
    //        "quantity" => isAsc ? query.OrderBy(x => x.Product.product_quantity) : query.OrderByDescending(x => x.Product.product_quantity),
    //        "purchasedcount" => isAsc ? query.OrderBy(x => x.PurchasedCount) : query.OrderByDescending(x => x.PurchasedCount),
    //        "name" => isAsc ? query.OrderBy(x => x.Product.product_name) : query.OrderByDescending(x => x.Product.product_name),
    //        "id" => isAsc ? query.OrderBy(x => x.Product.product_id) : query.OrderByDescending(x => x.Product.product_id),
    //        _ => query.OrderBy(x => x.Product.product_id)
    //    };

    //    // Pagination
    //    int skip = (page - 1) * pageSize;
    //    query = query.Skip(skip).Take(pageSize);

    //    // Project to DTO
    //    var result = await query.Select(x => new ProductDto
    //    {
    //        ProductId = x.Product.product_id,
    //        ProductType = x.Product.prod_subcat_id,
    //        ProductName = x.Product.product_name,
    //        ProductAmount = x.Product.product_cost??0,
    //        Quantity = x.Product.product_quantity??0,
    //        PurchasedCount = x.PurchasedCount,
    //        ProductImage = x.Product.product_image,
    //        // You can add Rating & Review here if you have that data
    //    }).ToListAsync();

    //    return result;
    //}

    //Show customer data analtics

    public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(
    int storeId,
    string? search = null,
    string? sortBy = null,
    bool descending = false)
    {
        //  Visited and Purchased
        var purchasedCustomers = await _context.OrderDetails
            .Where(od => od.StoreId == storeId &&
                         (od.Order.Payments.Any() || od.Order.ShippingDetails.Any()))
            .Select(od => od.Order.ShopperRegId)
            .Distinct()
            .ToListAsync();

        //  Visited but Not Purchased
        var notPurchasedCustomers = await _context.OrderDetails
            .Where(od => od.StoreId == storeId &&
                         !od.Order.Payments.Any() &&
                         !od.Order.ShippingDetails.Any())
            .Select(od => od.Order.ShopperRegId)
            .Distinct()
            .ToListAsync();

        // Frequent Customers - Get ShopperRegId + count
        var frequentCustomersRaw = await _context.OrderDetails
            .Where(od => od.StoreId == storeId && od.Order.Payments.Any())
            .GroupBy(od => od.Order.ShopperRegId)
            .Where(g => g.Count() > 1)
            .Select(g => new { ShopperRegId = g.Key, PurchaseCount = g.Count() })
            .ToListAsync();

        // Join to get name/phone
        var shopperIds = frequentCustomersRaw.Select(x => x.ShopperRegId).ToList();
        var shopperDetails = await _context.ShopperRegisters
            .Where(s => shopperIds.Contains(s.ShopperRegId))
            .ToListAsync();

        var frequentCustomers = frequentCustomersRaw
            .Join(shopperDetails,
                raw => raw.ShopperRegId,
                shopper => shopper.ShopperRegId,
                (raw, shopper) => new FrequentCustomerDto
                {
                    Name = shopper.Username,
                    PhoneNumber = shopper.PhoneNumber,
                    PurchaseCount = raw.PurchaseCount
                })
            .AsQueryable();

        // Apply search and sort
        if (!string.IsNullOrEmpty(search))
            frequentCustomers = frequentCustomers.Where(fc => fc.Name.Contains(search));

        if (!string.IsNullOrEmpty(sortBy))
        {
            frequentCustomers = sortBy.ToLower() switch
            {
                "name" => descending
                    ? frequentCustomers.OrderByDescending(fc => fc.Name)
                    : frequentCustomers.OrderBy(fc => fc.Name),
                "count" => descending
                    ? frequentCustomers.OrderByDescending(fc => fc.PurchaseCount)
                    : frequentCustomers.OrderBy(fc => fc.PurchaseCount),
                _ => frequentCustomers
            };
        }

        var finalFrequentCustomers = frequentCustomers.ToList();

        //Customers Who Purchased (Names and Phones)
        var customersWhoPurchasedQuery = _context.OrderDetails
            .Where(od => od.StoreId == storeId &&
                         (od.Order.Payments.Any() || od.Order.ShippingDetails.Any()))
            .Select(od => new
            {
                od.Order.ShopperRegister.Username,
                od.Order.ShopperRegister.PhoneNumber
            })
            .Distinct();

        if (!string.IsNullOrEmpty(search))
            customersWhoPurchasedQuery = customersWhoPurchasedQuery
                .Where(c => c.Username.Contains(search));

        if (!string.IsNullOrEmpty(sortBy))
        {
            customersWhoPurchasedQuery = sortBy.ToLower() switch
            {
                "name" => descending
                    ? customersWhoPurchasedQuery.OrderByDescending(c => c.Username)
                    : customersWhoPurchasedQuery.OrderBy(c => c.Username),
                _ => customersWhoPurchasedQuery
            };
        }

        var customersWhoPurchased = await customersWhoPurchasedQuery
            .Select(c => new CustomerDto
            {
                Name = c.Username,
                PhoneNumber = c.PhoneNumber
            })
            .ToListAsync();

        return new CustomerAnalyticsDto
        {
            CustomersVisitedAndPurchased = purchasedCustomers.Count,
            CustomersVisitedButNotPurchased = notPurchasedCustomers.Count,
            FrequentCustomers = finalFrequentCustomers,
            CustomersWhoPurchased = customersWhoPurchased
        };
    }


    //latest 05-01-26 Orders - new, pending,in progress, complete

    private class StoreOrderJoin
    {
        public StoreOrder StoreOrder { get; set; }
        public Order Order { get; set; }
        public ShippingDetails Shipping { get; set; }
    }
    private IQueryable<StoreOrderJoin> BaseQuery()
    {
        return from so in _context.StoreOrders
               join o in _context.Orders on so.OrderId equals o.OrderId
               join sd in _context.ShippingDetails
                    on so.StoreOrderId equals sd.StoreOrderId
                    into shipping
               from sd in shipping.DefaultIfEmpty()
               select new StoreOrderJoin
               {
                   StoreOrder = so,
                   Order = o,
                   Shipping = sd
               };
    }

    //New Orders

    public async Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId)
    {
        var today = DateTime.Today;

        return await BaseQuery()
            .Where(x =>
                x.StoreOrder.StoreId == storeId &&
                x.Order.OrderDate >= today &&
                x.Order.OrderDate < today.AddDays(1) &&
                x.Order.OrderStatus == "In Progress")
            .Select(x => new BusinessOrderListDto
            {
                StoreOrderId = x.StoreOrder.StoreOrderId,
                Status = "New",
                EstimatedDeliveryDate =
                    x.Shipping != null
                        ? x.Order.OrderDate.AddDays(x.Shipping.EstimatedDays)
                        : null
            })
            .ToListAsync();
    }
    // pending orders

    public async Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId)
    {
        var today = DateTime.Today;

        return await BaseQuery()
            .Where(x =>
                x.StoreOrder.StoreId == storeId &&
                x.Order.OrderDate < today &&
                x.Order.OrderStatus == "In Progress")
            .Select(x => new BusinessOrderListDto
            {
                StoreOrderId = x.StoreOrder.StoreOrderId,
                Status = "Pending",
                EstimatedDeliveryDate =
                    x.Shipping != null
                        ? x.Order.OrderDate.AddDays(x.Shipping.EstimatedDays)
                        : null
            })
            .ToListAsync();
    }


    // In progress / Shipped

    public async Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId)
    {
        return await BaseQuery()
            .Where(x =>
                x.StoreOrder.StoreId == storeId &&
                x.Shipping != null &&
                x.Shipping.ShippingStatus == "Shipped")
            .Select(x => new BusinessOrderListDto
            {
                StoreOrderId = x.StoreOrder.StoreOrderId,
                Status = "Shipped",
                EstimatedDeliveryDate =
                    x.Order.OrderDate.AddDays(x.Shipping.EstimatedDays)
            })
            .ToListAsync();
    }

    // Completed orders

    public async Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId)
    {
        return await BaseQuery()
            .Where(x =>
                x.StoreOrder.StoreId == storeId &&
                x.Shipping != null &&
                x.Shipping.ShippingStatus == "Delivered")
            .Select(x => new BusinessOrderListDto
            {
                StoreOrderId = x.StoreOrder.StoreOrderId,
                Status = "Delivered",
                DeliveredDate = x.Shipping.DeliveredDate
            })
            .ToListAsync();
    }


    // Show storeorderid detailsor orders on dashboard

    public async Task<BusinessOrderDetailsDto> GetBusinessOrderDetailsAsync(int storeOrderId)
    {
        // 🔹 Get order + shopper + store + shipping info
        var orderData = await (
            from so in _context.StoreOrders
            join o in _context.Orders on so.OrderId equals o.OrderId
            join sd in _context.ShippingDetails on so.StoreOrderId equals sd.StoreOrderId
            join p in _context.Payments on o.OrderId equals p.OrderId
            join s in _context.BusinessRegisters on so.StoreId equals s.BusRegId
            join sh in _context.ShopperRegisters on o.ShopperRegId equals sh.ShopperRegId
            where so.StoreOrderId == storeOrderId
            select new
            {
                so.StoreOrderId,
                o.OrderDate,
                p.PaymentId,
                o.ShopperRegId,
                ShopperName = sh.Username,
                ShopperPhone = sh.PhoneNumber,
                so.StoreId,
                StoreName = s.BusinessName,
                StoreTown = s.Town,
                o.ShippingType,
                sd.DeliveryAddress,
                sd.EstimatedDays,
                sd.CourierBranch,
                sd.TrackingId
            }
        ).FirstOrDefaultAsync();

        if (orderData == null)
            return null;

        // 🔹 Get products with image (SKU image preferred)
        var products = await (
            from od in _context.OrderDetails
            join pr in _context.products on od.ProductId equals pr.ProductId
            join sku in _context.Sku_ProductVariants on od.SkuId equals sku.SkuId into skuJoin
            from sku in skuJoin.DefaultIfEmpty()

            join skuImg in _context.ProductImages
                .Where(i => i.SortOrder == 1)
                on sku.SkuId equals skuImg.SkuId into skuImgJoin
            from skuImg in skuImgJoin.DefaultIfEmpty()

            join prodImg in _context.ProductImages
                .Where(i => i.SortOrder == 1 && i.SkuId == null)
                on pr.ProductId equals prodImg.ProductId into prodImgJoin
            from prodImg in prodImgJoin.DefaultIfEmpty()

            where od.StoreOrderId == storeOrderId
            select new BusinessOrderProductDto
            {
                ProductId = pr.ProductId,
                ProductName = pr.ProductName,
                Quantity = od.Quantity,
                UnitPrice = od.Price,
                Amount = od.Price * od.Quantity,
                ProductImage = skuImg.FileName ?? prodImg.FileName
            }
        ).ToListAsync();

        var productAmount = products.Sum(p => p.Amount);

        // 🔹 Combine everything in a single DTO
        return new BusinessOrderDetailsDto
        {
            StoreOrderId = orderData.StoreOrderId,
            OrderDate = orderData.OrderDate,
            TransactionId = orderData.PaymentId,
            ShopperId = orderData.ShopperRegId,
            ShopperName = orderData.ShopperName,
            ShopperPhone = orderData.ShopperPhone,

            StoreId = orderData.StoreId,
            StoreName = orderData.StoreName,
            StoreTown = orderData.StoreTown,

            Products = products,
            ProductAmount = productAmount,

            ShippingMethod = orderData.ShippingType,
            ShippingAddress = orderData.DeliveryAddress,
            EstimatedDeliveryDate = orderData.OrderDate.AddDays(orderData.EstimatedDays),
         //   CourierService = orderData.CourierBranch,
            TrackingId = orderData.TrackingId
        };
    }



public async Task<List<BusinessProductDashboardDto>> GetProductsForDashboardAsync(int storeId)
    {
        return await _context.products
            .Where(p => p.BusRegId == storeId)
            .Select(p => new BusinessProductDashboardDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,

                CategoryName = p.BusinessRegister.BusinessCategory.BusinessCategoryName,
                ProductType = p.ProductType != null ? p.ProductType.ProdTypeName : null,


                Fabric = p.Fabric != null ? p.Fabric.FabricName : null,
                Design = p.Design != null ? p.Design.DesignName : null,

                Supplier = p.SupplierName,
                ProductDescription = p.ProductDescription,

                //  Price = minimum SKU price
                ProductAmount = p.Sku_ProductVariants.Min(v => v.DiscountPrice ?? v.Sku_Cost),

                //  Stock = total qty of all variants
                InStock = p.Sku_ProductVariants.Sum(v => (int)v.Quantity),

                Discount = p.Sku_ProductVariants.Max(v => v.Discount),

                //  Purchased count
                NoOfPurchased = _context.OrderDetails
                    .Where(od => od.ProductId == p.ProductId)
                    .Sum(od => od.Quantity),

                //  Image priority logic
                ProductImage =
                    p.Sku_ProductVariants
                        .SelectMany(v => v.Images)
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.FileName)
                        .FirstOrDefault()
                    ??
                    p.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.FileName)
                        .FirstOrDefault()
            })
            .ToListAsync();
    }



    private IQueryable<StoreOrder> GetDeliveredPaidOrders(int storeId)
    {
        var query =
            from so in _context.StoreOrders
            join o in _context.Orders
                on so.OrderId equals o.OrderId
            join p in _context.Payments
                on o.OrderId equals p.OrderId
            join sd in _context.ShippingDetails
                on so.StoreOrderId equals sd.StoreOrderId
            where so.StoreId == storeId
                  && p.PaymentStatus == "Paid"
                  && sd.ShippingStatus == "Delivered"
            select so;

        return query;
    }

    // Daily Sales
    public async Task<BusinessSalesSummaryDto> GetDailySalesAsync(
    int storeId, DateTime date, string currency)
    {
        var query = GetDeliveredPaidOrders(storeId)
            .Where(so => so.Order.OrderDate.Date == date.Date);

        return new BusinessSalesSummaryDto
        {
            TotalOrders = await query.CountAsync(),
            TotalRevenue = await query.SumAsync(x => x.StoreTotalAmount),
            Currency = currency
        };
    }

    //Monthly sales

    public async Task<BusinessSalesSummaryDto> GetMonthlySalesAsync(
    int storeId, int year, int month, string currency)
    {
        var query = GetDeliveredPaidOrders(storeId)
            .Where(so =>
                so.Order.OrderDate.Year == year &&
                so.Order.OrderDate.Month == month);

        return new BusinessSalesSummaryDto
        {
            TotalOrders = await query.CountAsync(),
            TotalRevenue = await query.SumAsync(x => x.StoreTotalAmount),
            Currency = currency
        };
    }

    //yearly sales

    public async Task<BusinessSalesSummaryDto> GetYearlySalesAsync(
    int storeId, int year, string currency)
    {
        var query = GetDeliveredPaidOrders(storeId)
            .Where(so => so.Order.OrderDate.Year == year);

        return new BusinessSalesSummaryDto
        {
            TotalOrders = await query.CountAsync(),
            TotalRevenue = await query.SumAsync(x => x.StoreTotalAmount),
            Currency = currency
        };
    }

    //custom calenederdates
    public async Task<BusinessSalesSummaryDto> GetSalesByDateRangeAsync(
    int storeId, DateTime from, DateTime to, string currency)
    {
        var query = GetDeliveredPaidOrders(storeId)
            .Where(so =>
                so.Order.OrderDate >= from &&
                so.Order.OrderDate <= to);

        return new BusinessSalesSummaryDto
        {
            TotalOrders = await query.CountAsync(),
            TotalRevenue = await query.SumAsync(x => x.StoreTotalAmount),
            Currency = currency
        };
    }

    // Salestab_ store transaction details

    public async Task<List<Salestab_storeTransactionsDto>> GetStoreTransactionsAsync(int storeId)
    {
        var result = await (
            from so in _context.StoreOrders
            join o in _context.Orders on so.OrderId equals o.OrderId
            join p in _context.Payments on o.OrderId equals p.OrderId
            join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
            where so.StoreId == storeId
                  && p.PaymentStatus == "Paid"
            orderby p.PaymentDate descending
            select new Salestab_storeTransactionsDto
            {
                TransactionId = p.PaymentId,
                PaymentDate = p.PaymentDate,
                ShopperId = s.ShopperRegId,
                
                Amount = p.AmountPaid,
                PaymentStatus = p.PaymentStatus
            }
        ).ToListAsync();

        return result;
    }

    // get product variant details

    public async Task<List<Sku_ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
    {
        return await _context.Sku_ProductVariants
            .Where(v => v.ProductId == productId)
            .Select(v => new Sku_ProductVariantDto
            {
                SkuId_Productvariant = v.SkuId,
                Color = v.Color,
                SizeName = v.Size != null ? v.Size.SizeName : null,

                Sku_Cost = v.Sku_Cost,
                DiscountPrice = v.DiscountPrice,
                Quantity = v.Quantity,

                Length = v.Length,
                Width = v.Width,
                Height = v.Height,
                Weight = v.Weight,
                Images = v.Images
    .Where(i => i.SortOrder == 1)
    .Select(i => new ProductImageDto
    {
       // ImageId = i.ImageId,
        FileName = i.FileName,
        SortOrder = i.SortOrder
    })
    .ToList()
            })
            .ToListAsync();
    }

    //notifications to business dashboard

    public async Task<List<BusinessDBNotifications>> GetNotificationsAsync(
    int busRegId, bool onlyUnread)
    {
        var query = _context.BusinessDBNotifications
            .Where(n => n.BusRegId == busRegId);

        if (onlyUnread)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task MarkAllAsReadAsync(int busRegId)
    {
        var unreadNotifications = await _context.BusinessDBNotifications
            .Where(n => n.BusRegId == busRegId && !n.IsRead)
            .ToListAsync();

        foreach (var n in unreadNotifications)
            n.IsRead = true;

        await _context.SaveChangesAsync();
    }

    // country wise sales
    public async Task<List<CountrySalesDto>> GetCountryWiseSalesAsync(int storeId)
    {
        var result =
            from so in _context.StoreOrders
            join o in _context.Orders
                on so.OrderId equals o.OrderId
            join p in _context.Payments
                on o.OrderId equals p.OrderId
            join s in _context.ShopperRegisters
                on o.ShopperRegId equals s.ShopperRegId
            where so.StoreId == storeId
                  && p.PaymentStatus == "Paid"
            group new { o, p } by s.Country into g
            select new CountrySalesDto
            {
                Country = g.Key,
                TotalOrders = g
                    .Select(x => x.o.OrderId)
                    .Distinct()
                    .Count(),
                TotalSales = g.Sum(x => x.p.AmountPaid)
            };

        return await result
            .OrderByDescending(x => x.TotalSales)
            .ToListAsync();
    }

    // product wise sales - top 5

    public async Task<List<ProductSalesDto>> GetTopProductsAsync(int storeId, int topCount)
    {
        var query =
            from so in _context.StoreOrders
            join o in _context.Orders
                on so.OrderId equals o.OrderId
            join p in _context.Payments
                on o.OrderId equals p.OrderId
            join sd in _context.ShippingDetails
                on so.StoreOrderId equals sd.StoreOrderId
            join od in _context.OrderDetails
                on so.StoreOrderId equals od.StoreOrderId
            join pr in _context.products
                on od.ProductId equals pr.ProductId
            where so.StoreId == storeId
                  && p.PaymentStatus == "Paid"
                  && sd.ShippingStatus == "Delivered"
            group new { od, pr } by new
            {
                pr.ProductId,
                pr.ProductName
            }
            into g
            select new ProductSalesDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                TotalQuantitySold = g.Sum(x => x.od.Quantity),
                TotalRevenue = g.Sum(x => x.od.Quantity * x.od.Price)
            };

        return await query
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(topCount)
            .ToListAsync();
    }

}


