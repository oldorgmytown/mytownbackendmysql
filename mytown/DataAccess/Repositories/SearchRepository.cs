using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;

namespace mytown.DataAccess.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly AppDbContext _context;

        public SearchRepository(AppDbContext context)
        {
            _context = context;
        }

        // API to get list of products based on location and category, subcategory and product search
        public List<Products> SearchBusinessesWithProducts(string locationQuery, string productQuery)
        {
            if (string.IsNullOrEmpty(productQuery))
                return new List<Products>();

            List<int> filteredBusinesses = new List<int>();

            if (!string.IsNullOrEmpty(locationQuery))
            {
                filteredBusinesses = _context.BusinessRegisters
                    .Where(b =>
                        b.Town.Contains(locationQuery) ||
                        b.BusinessCity.Contains(locationQuery) ||
                        b.BusinessState.Contains(locationQuery) ||
                        b.BusinessCountry.Contains(locationQuery) ||
                        b.Address1.Contains(locationQuery) ||
                        b.Address2.Contains(locationQuery)
                    )
                    .Select(b => b.BusRegId)
                    .ToList();

                if (!filteredBusinesses.Any())
                    return new List<Products>();
            }

            var matchingCategories = _context.BusinessCategories
                .Where(c => c.BusinessCategoryName.Contains(productQuery))
                .Select(c => c.BusCatId)
                .ToList();

            var matchingSubCategories = _context.product_sub_categories
                .Where(sc => sc.ProdSubcatName.Contains(productQuery))
                .Select(sc => sc.ProdSubcatId)
                .ToList();

            var productsQuery = _context.products.AsQueryable();

            if (!string.IsNullOrEmpty(locationQuery))
            {
                productsQuery = productsQuery.Where(p => filteredBusinesses.Contains(p.BusRegId));
            }

            productsQuery = productsQuery.Where(p =>
                matchingCategories.Contains(p.BuscatId) ||
                matchingSubCategories.Contains(p.ProdSubcatId) ||
                p.ProductName.Contains(productQuery) ||
                p.ProductSubject.Contains(productQuery) ||
                p.ProductDescription.Contains(productQuery)
            );

            return productsQuery.ToList();
        }

        // Search from location and product/category and get the matching store  details
        public async Task<List<BusinessProfile>> SearchBusinessesAsync(string location, string categoryProduct)
        {
            var productResults = new List<Products>();
            var busRegIds = new List<int>();
            var businessesByLocation = new List<BusinessRegister>();
            var businessesByCategoryProduct = new List<BusinessRegister>();

            if (!string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
            {
                var subCategory = await _context.product_sub_categories
                    .Where(psc => psc.ProdSubcatName.Contains(categoryProduct))
                    .FirstOrDefaultAsync();

                if (subCategory != null)
                {
                    productResults = await _context.products
                        .Where(p => p.ProdSubcatId == subCategory.ProdSubcatId)
                        .ToListAsync();

                    busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                }
                else
                {
                    var category = await _context.BusinessCategories
                        .Where(c => c.BusinessCategoryName.Contains(categoryProduct))
                        .FirstOrDefaultAsync();

                    if (category != null)
                    {
                        productResults = await _context.products
                            .Where(p => p.BuscatId == category.BusCatId)
                            .ToListAsync();

                        busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                    }
                    else
                    {
                        productResults = await _context.products
                            .Where(p => p.ProductName.Contains(categoryProduct) ||
                                        p.ProductSubject.Contains(categoryProduct) ||
                                        p.ProductDescription.Contains(categoryProduct))
                                      //  p.ProductType.Contains(categoryProduct)
                                       
                            .ToListAsync();

                        busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                    }
                }
            }

            if (!string.IsNullOrEmpty(location) && location != "null")
            {
                businessesByLocation = await _context.BusinessRegisters
                    .Where(br => br.Town.Contains(location) ||
                                 br.BusinessCity.Contains(location) ||
                                 br.BusinessState.Contains(location) ||
                                 br.BusinessCountry.Contains(location))
                    .ToListAsync();
            }

            if (busRegIds.Any())
            {
                businessesByCategoryProduct = await _context.BusinessRegisters
                    .Where(br => busRegIds.Contains(br.BusRegId))
                    .ToListAsync();
            }

            List<BusinessRegister> combinedResults;

            if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
            {
                combinedResults = businessesByLocation
                    .Where(loc => businessesByCategoryProduct.Any(cat => cat.BusRegId == loc.BusRegId))
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(location) && (string.IsNullOrEmpty(categoryProduct) || categoryProduct == "null"))
            {
                combinedResults = businessesByLocation;
            }
            else if (string.IsNullOrEmpty(location) || location == "null")
            {
                combinedResults = businessesByCategoryProduct;
            }
            else
            {
                combinedResults = new List<BusinessRegister>();
            }

            var result = new List<BusinessProfile>();

            foreach (var business in combinedResults)
            {
                var profile = await _context.BusinessProfiles
                    .FirstOrDefaultAsync(bp => bp.BusRegId == business.BusRegId);

                if (profile != null)
                {
                    result.Add(profile);
                }
            }

            return result;
        }

        // Get profiles based on location search
        public List<BusinessProfile> GetBusinessProfilesByLocation(string location)
        {
            var query =
        from bp in _context.BusinessProfiles
        where bp.BusinessLocation.Contains(location)
        select new
        {
            BusinessProfile = bp,
            TotalPurchases = (from o in _context.OrderDetails
                              join pr in _context.products on o.ProductId equals pr.ProductId
                              where pr.BusRegId == bp.BusRegId
                              select (int?)o.Quantity).Sum() ?? 0
        };

            var orderedProfiles = query
                .OrderByDescending(x => x.TotalPurchases)
                .Select(x => x.BusinessProfile)
                .ToList();

            return orderedProfiles;

        }



        public async Task<IEnumerable<ProductSubCategory>> GetProductSubCategoriesByLocationAsync(string location)
        {
            // Step 1: Get all business profiles matching location
            var busCatIds = await _context.BusinessProfiles
            .Where(bp => bp.BusinessLocation != null &&
                         bp.BusinessLocation.Contains(location)) // property name matches model
            .Select(bp => bp.BusCatId)
            .Distinct()
            .ToListAsync();

            if (!busCatIds.Any())
                return new List<ProductSubCategory>();

            // Step 2: Get all product_sub_categories for those categories
            var subCategories = await _context.product_sub_categories
                .Where(sc => busCatIds.Contains(sc.BuscatId))
                .Select(sc => new ProductSubCategory
                {
                    ProdSubcatId = sc.ProdSubcatId,
                    ProdSubcatName = sc.ProdSubcatName,
                    ProdSubcatImage = sc.ProdSubcatImage,
                    BuscatId = sc.BuscatId
                })
                .ToListAsync();

            return subCategories;
        }


        public async Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByLocationAsync(string location)
        {
            // Step 1: Get all business profiles matching the location
            var busCatIds = await _context.BusinessProfiles
                .Where(bp => bp.BusinessLocation != null &&
                             bp.BusinessLocation.Contains(location))
                .Select(bp => bp.BusCatId)
                .Distinct()
                .ToListAsync();

            if (!busCatIds.Any())
                return new List<BusinessCategory>();

            // Step 2: Get all business categories for those BusCatIds
            var categories = await _context.BusinessCategories
                .Where(bc => busCatIds.Contains(bc.BusCatId))
                .Select(bc => new BusinessCategory
                {
                    BusCatId = bc.BusCatId,
                    BusinessCategoryName = bc.BusinessCategoryName
                })
                .ToListAsync();

            return categories;
        }


        //search by both location and store or product
        public SearchResultDto SearchByLocationAndProduct(string searchTerm, string locationQuery)
        {
            // 1. Get stores in given location
            var locationBusinessIds = _context.BusinessRegisters
                .Where(b =>
                    (b.Town != null && b.Town.Contains(locationQuery)) ||
                    (b.BusinessCity != null && b.BusinessCity.Contains(locationQuery)) ||
                    (b.BusinessState != null && b.BusinessState.Contains(locationQuery)) ||
                    (b.BusinessCountry != null && b.BusinessCountry.Contains(locationQuery)) ||
                    (b.Address1 != null && b.Address1.Contains(locationQuery)) ||
                    (b.Address2 != null && b.Address2.Contains(locationQuery)))
                .Select(b => b.BusRegId)
                .ToList();

            if (!locationBusinessIds.Any())
            {
                return new SearchResultDto
                {
                    Stores = new List<BusinessProfile>(),
                    Products = new List<ProdcVariantforShopperDto>(),
                    Colors = new List<string>(),
                    StoreCount = 0,
                    ProductCount = 0
                };
            }

            // 2. Filter stores by store name OR products/categories inside those stores
            var storeMatches = _context.BusinessProfiles
                .Where(bp => locationBusinessIds.Contains(bp.BusRegId) &&
                             (bp.BusinessName.Contains(searchTerm) ||
                              bp.BusinessAbout.Contains(searchTerm)))
                .ToList();


            var products = _context.products
               .Where(p => locationBusinessIds.Contains(p.BusRegId) &&
                   (p.ProductName.Contains(searchTerm) ||
                    p.ProductSubject.Contains(searchTerm) ||
                    p.ProductDescription.Contains(searchTerm) ||
                    p.Sku_ProductVariants.Any(v =>
                        v.Color.Contains(searchTerm) ||
                        (v.Size != null && v.Size.SizeName.Contains(searchTerm)))))
               .Include(p => p.Sku_ProductVariants)
                   .ThenInclude(v => v.Images)
               .Select(p => new ProdcVariantforShopperDto
               {
                   ProductId = p.ProductId,
                   BusRegId = p.BusRegId,
                   BusinessName = p.BusinessRegister.BusinessName,
                   BuscatId = p.BuscatId,
                   //  BuscatName = p.BusinessCategory != null ? p.BusinessCategory.Businesscategory_name : null,
                   ProdcatId = p.ProdSubcatId,
                   //  ProdcatName = p.ProductSubCategory != null ? p.ProductSubCategory.prod_subcat_name : null,
                   ProductName = p.ProductName,
                   ProductDescription = p.ProductDescription,
                   SupplierName = p.SupplierName,

                   Variants = p.Sku_ProductVariants
                       .Select(v => new Sku_ProductVariantDto
                       {
                           SkuId_Productvariant = v.SkuId,
                           ProductId = v.ProductId,
                           Color = v.Color,
                           SizeId = v.SizeId,
                           SizeName = v.Size != null ? v.Size.SizeName : null,
                           Sku_Cost = v.Sku_Cost,
                           DiscountPrice = v.DiscountPrice,
                           Quantity = v.Quantity,
                           Length = v.Length,
                           Width = v.Width,
                           Height = v.Height,
                           Weight = v.Weight,
                           Discount = v.Discount,
                           Images = v.Images
                               .OrderBy(i => i.SortOrder)
                               .Select(i => new ProductImageDto
                               {
                                   FileName = i.FileName,
                                   SortOrder = i.SortOrder
                               })
                               .ToList()
                       })
                       .ToList()
               })
               .ToList();

            // Collect colors only from the returned products/variants
            var colors = products
                .SelectMany(p => p.Variants)        // flatten all variants
                .Where(v => !string.IsNullOrEmpty(v.Color)) // filter null/empty
                .Select(v => v.Color!)
                .Distinct()
                .ToList();

            // 4. Return combined result
            return new SearchResultDto
            {
                Stores = storeMatches,
                Products = products,
                Colors = colors,
                StoreCount = storeMatches.Count,
                ProductCount = products.Count
            };
        }



    }
}
