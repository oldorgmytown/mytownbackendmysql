using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class MobileAppRepository : IMobileAppRepository
    {
        private readonly AppDbContext _context;

        public MobileAppRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PopularProductDto>> GetPopularProductsAsync()
        {
            // Step 1: Get Top 18 products by quantity sold
            var topProducts = await _context.OrderDetails
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOrders = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalOrders)
                .Take(18)
                .ToListAsync();

            if (!topProducts.Any())
                return new List<PopularProductDto>();

            var topProductIds = topProducts
                .Select(x => x.ProductId)
                .ToList();

            // Step 2: Find best-selling variant for each product
            var bestVariants = await _context.OrderDetails
                .Where(x => topProductIds.Contains(x.ProductId))
                .GroupBy(x => new { x.ProductId, x.SkuId })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.SkuId,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            var bestVariantLookup = bestVariants
                .GroupBy(x => x.ProductId)
                .Select(g => g.OrderByDescending(x => x.TotalSold).First())
                .ToDictionary(x => x.ProductId, x => x.SkuId);

            // Step 3: Load products + store + variant data
            var products = await (
                from p in _context.products
                join b in _context.BusinessRegisters
                    on p.BusRegId equals b.BusRegId
                join v in _context.Sku_ProductVariants
                    .Include(x => x.Images)
                    on p.ProductId equals v.ProductId
                where topProductIds.Contains(p.ProductId)
                select new
                {
                    Product = p,
                    Store = b,
                    Variant = v
                })
                .ToListAsync();

            // Step 4: Filter best variant in memory and map DTO
            var result = products
                .Where(x =>
                    bestVariantLookup.ContainsKey(x.Product.ProductId) &&
                    bestVariantLookup[x.Product.ProductId] == x.Variant.SkuId)
                .Select(x => new PopularProductDto
                {
                    ProductId = x.Product.ProductId,
                    ProductName = x.Product.ProductName,

                    BusRegId = x.Store.BusRegId,
                    StoreName = x.Store.BusinessName,
                    StoreCity = x.Store.BusinessCity,

                    SkuId = x.Variant.SkuId,

                    Cost = x.Variant.Sku_Cost,
                    DiscountPrice = x.Variant.DiscountPrice,
                    DiscountPercent = x.Variant.Discount,

                    ImageName = x.Variant.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.FileName)
                        .FirstOrDefault(),

                    TotalOrders = topProducts
                        .First(tp => tp.ProductId == x.Product.ProductId)
                        .TotalOrders
                })
                .OrderByDescending(x => x.TotalOrders)
                .ToList();

            return result;
        }

        public async Task<List<PopularStoresDto>> GetPopularStoresAsync()
        {
            // Top stores by quantity sold
            var topStores = await _context.OrderDetails
                .GroupBy(x => x.StoreId)
                .Select(g => new
                {
                    StoreId = g.Key,
                    TotalOrders = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalOrders)
                .Take(12)
                .ToListAsync();

            if (!topStores.Any())
                return new List<PopularStoresDto>();

            var storeIds = topStores
                .Select(x => x.StoreId)
                .ToList();

            var stores = await (
                from br in _context.BusinessRegisters
                join bp in _context.BusinessProfiles
                    on br.BusRegId equals bp.BusRegId
                join bc in _context.BusinessCategories
                    on bp.BusCatId equals bc.BusCatId
                where storeIds.Contains(br.BusRegId)
                select new
                {
                    br.BusRegId,
                    br.BusinessName,
                    bc.BusCatId,
                    bc.BusinessCategoryName,
                    bp.LogoPath,
                    bp.BannerPath,
                    bp.BusinessLocation
                })
                .ToListAsync();

            var result = stores
                .Select(x => new PopularStoresDto
                {
                    BusRegId = x.BusRegId,
                    StoreName = x.BusinessName,

                    BuscatId = x.BusCatId,
                    CategoryName = x.BusinessCategoryName,

                    StoreLogo = x.LogoPath,
                    StoreBanner = x.BannerPath,

                    Location = x.BusinessLocation,

                    TotalOrders = topStores
                        .FirstOrDefault(t => t.StoreId == x.BusRegId)?.TotalOrders ?? 0
                })
                .OrderByDescending(x => x.TotalOrders)
                .ToList();

            return result;
        }

        public async Task<List<TownStoreCountDto>> GetExploreTownsAsync()
        {
            var result = await _context.BusinessRegisters
                .Where(x => !string.IsNullOrEmpty(x.Town))
                .GroupBy(x => new
                {
                    x.Town,
                    x.BusinessCountry
                })
                .Select(g => new TownStoreCountDto
                {
                    TownName = g.Key.Town,
                    CountryName = g.Key.BusinessCountry,
                    StoreCount = g.Count()
                })
                .OrderByDescending(x => x.StoreCount)
                .ToListAsync();

            return result;
        }

        public async Task<List<AvailableTransporterDto>> GetAvailableTransportersAsync(
     string startTown,
     string startCity,
     string destinationTown,
     string destinationCity)
        {
            return await (
                from tp in _context.TransporterTravelPlans
                join tr in _context.TransporterRegisters
                    on tp.TransporterRegId equals tr.TransporterRegId
                where tp.IsActive
                      && tp.PlanStatus == "Available"
                      && tp.StartTown == startTown
                      && tp.StartCity == startCity
                      && tp.DestinationTown == destinationTown
                      && tp.DestinationCity == destinationCity
                orderby tp.StartDate
                select new AvailableTransporterDto
                {
                    PlanId = tp.PlanId,
                    TransporterRegId = tp.TransporterRegId,

                    TransporterName = tr.TransporterName,

                    VehicleType = tp.VehicleType,
                    VehicleName = tp.VehicleName,

                    StartTown = tp.StartTown,
                    StartCity = tp.StartCity,
                    StartState = tp.StartState,
                    StartCountry = tp.StartCountry,

                    DestinationTown = tp.DestinationTown,
                    DestinationCity = tp.DestinationCity,
                    DestinationState = tp.DestinationState,
                    DestinationCountry = tp.DestinationCountry,
                    StartDate = tp.StartDate,
                    ArrivalDate = tp.ArrivalDate,
                    
                    MaxWeightKg = tp.MaxWeightKg,

                    PreferredContact = tp.PreferredContact,
                    PreferredRoute = tp.PreferredRoute
                })
                .ToListAsync();
        }

        public async Task<List<TownListDto>> GetTownListByCityAsync(string city)
        {
        var stores = await _context.BusinessRegisters
            .Where(x =>
                x.BusinessCity == city &&
                !string.IsNullOrEmpty(x.Town))
            .Select(x => new
            {
                x.Town,
                x.BusinessName
            })
            .ToListAsync();

        var result = stores
            .GroupBy(x => x.Town)
            .Select(g => new TownListDto
            {
                TownName = g.Key,

                ActiveStoreCount = g.Count(),

                PopularStores = g
                    .Select(x => x.BusinessName)
                    .Distinct()
                    .Take(3)
                    .ToList()
            })
            .OrderByDescending(x => x.ActiveStoreCount)
            .ToList();

        return result;
}
        public async Task<List<AllProductsDto>> GetAllProductsAsync()
{
    var result = await (
        from p in _context.products
        join b in _context.BusinessRegisters
            on p.BusRegId equals b.BusRegId
        join v in _context.Sku_ProductVariants
            .Include(x => x.Images)
            on p.ProductId equals v.ProductId

        select new AllProductsDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,

            BusRegId = b.BusRegId,
            StoreName = b.BusinessName,
            StoreCity = b.BusinessCity,

            SkuId = v.SkuId,

            Cost = v.Sku_Cost,
            DiscountPrice = v.DiscountPrice,
            DiscountPercent = v.Discount,

            ImageName = v.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => i.FileName)
                .FirstOrDefault(),

            ProductDescription = p.ProductDescription,
            AvailableQuantity = v.Quantity
        })
        .ToListAsync();

    return result;
}


    public async Task<List<AllProductsDto>> GetProductsBySubCategoryAsync(int subCategoryId)
{
    var result = await (
        from p in _context.products
        join b in _context.BusinessRegisters
            on p.BusRegId equals b.BusRegId
        join v in _context.Sku_ProductVariants
            .Include(x => x.Images)
            on p.ProductId equals v.ProductId
        where p.ProdSubcatId == subCategoryId
              && p.IsActive == true
        select new AllProductsDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,

            BusRegId = b.BusRegId,
            StoreName = b.BusinessName,
            StoreCity = b.BusinessCity,

            SkuId = v.SkuId,

            Cost = v.Sku_Cost,
            DiscountPrice = v.DiscountPrice,
            DiscountPercent = v.Discount,

            ImageName = v.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => i.FileName)
                .FirstOrDefault(),

            ProductDescription = p.ProductDescription,

            AvailableQuantity = (int)v.Quantity
        })
        .ToListAsync();

    return result;
}


        public async Task<List<StoreBySubCategoryDto>> GetStoresBySubCategoryAsync(int prodSubcatId)
{
    var result = await (
        from p in _context.products
        join br in _context.BusinessRegisters
            on p.BusRegId equals br.BusRegId
        join bp in _context.BusinessProfiles
            on br.BusRegId equals bp.BusRegId
        where p.ProdSubcatId == prodSubcatId
              && p.IsActive
        select new StoreBySubCategoryDto
        {
            BusRegId = br.BusRegId,

            StoreName = br.BusinessName,
            StoreCity = br.BusinessCity,
            StoreState = br.BusinessState,
            StoreCountry = br.BusinessCountry,

            StorePhone = br.BusMobileNo,
            StoreEmail = br.BusEmail,

            StoreLogo = bp.LogoPath,
            StoreBanner = bp.BannerPath,

            StoreDescription = bp.BusinessAbout,
            StoreLocation = bp.BusinessLocation
        })
        .Distinct()
        .ToListAsync();

    return result;
}

public async Task<List<PopularCityDto>> GetPopularCitiesAsync()
{
    var cities = await _context.BusinessRegisters
        .Where(x => !string.IsNullOrEmpty(x.BusinessCity))
        .GroupBy(x => new { x.BusinessCity, x.BusinessCountry })
        .Select(g => new
        {
            City = g.Key.BusinessCity,
            Country = g.Key.BusinessCountry,
            StoreCount = g.Count()
        })
        .OrderByDescending(x => x.StoreCount)
        .Take(12)
        .ToListAsync();

    // Left join with CityImages in memory
    var cityNames = cities.Select(c => c.City).ToList();
    var images = await _context.CityImages
        .Where(ci => cityNames.Contains(ci.City))
        .ToListAsync();

    var result = cities.Select(c => new PopularCityDto
    {
        City = c.City,
        Country = c.Country,
        StoreCount = c.StoreCount,
        ImageFileName = images
            .FirstOrDefault(i => i.City == c.City && i.Country == c.Country)
            ?.ImageFileName
    }).ToList();

    return result;
}

public async Task<List<LocationImageDto>> GetLocationImagesAsync()
{
    return await _context.LocationImages
        .Where(x => x.IsActive)
        .Select(x => new LocationImageDto
        {
            Id = x.Id,
            Country = x.Country,
            StateName = x.StateName,
            City = x.City,
            Image = x.Image
        })
        .ToListAsync();
}

public async Task<List<LocationImageDto>> GetLocationImageCountriesAsync()
{
    return await _context.LocationImages
        .Where(x => x.IsActive)
        .GroupBy(x => x.Country)
        .Select(g => new LocationImageDto
        {
            Country = g.Key,
            Image = g.FirstOrDefault().Image
        })
        .ToListAsync();
}

public async Task<List<LocationImageDto>> GetLocationImageCitiesAsync()
{
    return await _context.LocationImages
        .Where(x => x.IsActive)
        .GroupBy(x => x.City)
        .Select(g => new LocationImageDto
        {
            City = g.Key,
            Image = g.FirstOrDefault().Image
        })
        .ToListAsync();
}

public async Task<List<CountryDto>> GetAllCountriesAsync()
{
    return await _context.LocationImages
        .Where(x => x.IsActive)
        .GroupBy(x => x.Country)
        .Select(g => new CountryDto
        {
            Country = g.Key
        })
        .ToListAsync();
}

    

    }
}