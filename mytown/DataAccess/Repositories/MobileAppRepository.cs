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
    }
}
