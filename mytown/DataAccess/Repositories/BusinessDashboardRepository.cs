using mytown.DataAccess.Interfaces;
using mytown.Models.mytown.DataAccess;
using Microsoft.EntityFrameworkCore;
using mytown.Models.DTO_s;


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



    public async Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId)
    {
        var query = from od in _context.OrderDetails
                    join o in _context.Orders on od.OrderId equals o.OrderId
                    join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
                    join p in _context.products on od.ProductId equals p.ProductId
                    join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
                    from payment in payJoin.DefaultIfEmpty()
                    join sd in _context.ShippingDetails on od.OrderDetailId equals sd.OrderDetailId into sdJoin
                    from shipping in sdJoin.DefaultIfEmpty()
                    where od.StoreId == storeId && o.OrderStatus == "Paid"
                    select new BusinessDashboardDto
                    {
                        OrderId = o.OrderId,
                        OrderDetailId = od.OrderDetailId,
                        OrderDate = o.OrderDate,
                        CustomerName = s.Username,
                        ShopperId = s.ShopperRegId,
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Quantity = od.Quantity,
                        Amount = od.Quantity * od.Price,
                        PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
                        TransactionId = payment.PaymentId,
                        Address = s.Address,
                        Town = s.Town,
                        City = s.City,
                        State = s.State,
                        Country = s.Country,
                        DeliveryType = shipping != null ? shipping.ShippingType : "Standard",
                        ShippingStatus = shipping != null ? shipping.ShippingStatus : "Not Shipped"
                    };

        var result = await query.ToListAsync();

        // Categorize paid orders based on shipping status and date
        foreach (var order in result)
        {
            order.OrderCategory =
                (order.OrderDate >= DateTime.UtcNow.AddDays(-2) && order.ShippingStatus == "Not Shipped") ? "New" :
                (order.ShippingStatus == "Not Shipped") ? "Pending" :
                (order.ShippingStatus == "In Transit") ? "In Progress" :
                (order.ShippingStatus == "Delivered") ? "Completed" :
                "Other";
        }

        return result;
    }

    // orders sales history with sort and search
    public async Task<List<BusinessDashboardDto>> GetStoreOrdersReportsortsearch(
     int storeId,
     string? search = null,
     string? sortBy = null,
     bool descending = false)
    {
        var rawQuery = from od in _context.OrderDetails
                       join o in _context.Orders on od.OrderId equals o.OrderId
                       join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
                       join p in _context.products on od.ProductId equals p.ProductId
                       join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
                       from payment in payJoin.DefaultIfEmpty()
                       join sd in _context.ShippingDetails on od.OrderDetailId equals sd.OrderDetailId into sdJoin
                       from shipping in sdJoin.DefaultIfEmpty()
                       where od.StoreId == storeId && o.OrderStatus == "Paid"
                       select new BusinessDashboardDto
                       {
                           OrderId = o.OrderId,
                           OrderDetailId = od.OrderDetailId,
                           OrderDate = o.OrderDate,
                           CustomerName = s.Username,
                           ShopperId = s.ShopperRegId,
                           ProductId = p.ProductId,
                           ProductName = p.ProductName,
                           Quantity = od.Quantity,
                           Amount = od.Quantity * od.Price,
                           PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
                           TransactionId = payment.PaymentId,
                           Address = s.Address,
                           Town = s.Town,
                           City = s.City,
                           State = s.State,
                           Country = s.Country,
                           DeliveryType = shipping != null ? shipping.ShippingType : "Standard",
                           ShippingStatus = shipping != null ? shipping.ShippingStatus : "Not Shipped",

                       };

        // Materialize data first (to memory)
        var result = await rawQuery.ToListAsync();

        // In-memory search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            result = result.Where(q =>
                q.OrderId.ToString().Contains(search) ||
                q.OrderDetailId.ToString().Contains(search) ||
                q.OrderDate.ToString("yyyy-MM-dd").Contains(search) ||
                q.CustomerName.ToLower().Contains(search) ||
                q.ShopperId.ToString().Contains(search) ||
                q.ProductId.ToString().Contains(search) ||
                q.ProductName.ToLower().Contains(search) ||
                q.Quantity.ToString().Contains(search) ||
                q.Amount.ToString().Contains(search) ||
                q.PaymentStatus.ToLower().Contains(search) ||
                (q.TransactionId != null && q.TransactionId.ToString().Contains(search)) ||
                q.Address.ToLower().Contains(search) ||
                q.Town.ToLower().Contains(search) ||
                q.City.ToLower().Contains(search) ||
                q.State.ToLower().Contains(search) ||
                q.Country.ToLower().Contains(search) ||
                q.DeliveryType.ToLower().Contains(search) ||
                q.ShippingStatus.ToLower().Contains(search)
            ).ToList();
        }

        // In-memory sort
        // In-memory sort
        result = sortBy?.ToLower() switch
        {
            "orderid" => descending ? result.OrderByDescending(q => q.OrderId).ToList() : result.OrderBy(q => q.OrderId).ToList(),
            "orderdate" => descending ? result.OrderByDescending(q => q.OrderDate).ToList() : result.OrderBy(q => q.OrderDate).ToList(),
            "customername" => descending ? result.OrderByDescending(q => q.CustomerName).ToList() : result.OrderBy(q => q.CustomerName).ToList(),
            "productname" => descending ? result.OrderByDescending(q => q.ProductName).ToList() : result.OrderBy(q => q.ProductName).ToList(),
            "quantity" => descending ? result.OrderByDescending(q => q.Quantity).ToList() : result.OrderBy(q => q.Quantity).ToList(),
            "amount" => descending ? result.OrderByDescending(q => q.Amount).ToList() : result.OrderBy(q => q.Amount).ToList(),
            "transactionid" => descending ? result.OrderByDescending(q => q.TransactionId).ToList() : result.OrderBy(q => q.TransactionId).ToList(),
            "productid" => descending ? result.OrderByDescending(q => q.ProductId).ToList() : result.OrderBy(q => q.ProductId).ToList(),
            "orderdetailid" => descending ? result.OrderByDescending(q => q.OrderDetailId).ToList() : result.OrderBy(q => q.OrderDetailId).ToList(),
            "shopperid" => descending ? result.OrderByDescending(q => q.ShopperId).ToList() : result.OrderBy(q => q.ShopperId).ToList(),
            _ => result
        };

        // Categorize orders
        foreach (var order in result)
        {
            order.OrderCategory =
                (order.OrderDate >= DateTime.UtcNow.AddDays(-2) && order.ShippingStatus == "Not Shipped") ? "New" :
                (order.ShippingStatus == "Not Shipped") ? "Pending" :
                (order.ShippingStatus == "In Transit") ? "In Progress" :
                (order.ShippingStatus == "Delivered") ? "Completed" :
                "Other";
        }

        return result;
    }





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



}


