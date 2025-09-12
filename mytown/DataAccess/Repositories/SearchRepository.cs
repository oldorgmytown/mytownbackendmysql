using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

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
        public List<products> SearchBusinessesWithProducts(string locationQuery, string productQuery)
        {
            if (string.IsNullOrEmpty(productQuery))
                return new List<products>();

            List<int> filteredBusinesses = new List<int>();

            if (!string.IsNullOrEmpty(locationQuery))
            {
                filteredBusinesses = _context.BusinessRegisters
                    .Where(b =>
                        b.Town.Contains(locationQuery) ||
                        b.businessCity.Contains(locationQuery) ||
                        b.businessState.Contains(locationQuery) ||
                        b.businessCountry.Contains(locationQuery) ||
                        b.Address1.Contains(locationQuery) ||
                        b.Address2.Contains(locationQuery)
                    )
                    .Select(b => b.BusRegId)
                    .ToList();

                if (!filteredBusinesses.Any())
                    return new List<products>();
            }

            var matchingCategories = _context.BusinessCategories
                .Where(c => c.Businesscategory_name.Contains(productQuery))
                .Select(c => c.BuscatId)
                .ToList();

            var matchingSubCategories = _context.product_sub_categories
                .Where(sc => sc.prod_subcat_name.Contains(productQuery))
                .Select(sc => sc.prod_subcat_id)
                .ToList();

            var productsQuery = _context.products.AsQueryable();

            if (!string.IsNullOrEmpty(locationQuery))
            {
                productsQuery = productsQuery.Where(p => filteredBusinesses.Contains(p.BusRegId));
            }

            productsQuery = productsQuery.Where(p =>
                matchingCategories.Contains(p.BuscatId) ||
                matchingSubCategories.Contains(p.prod_subcat_id) ||
                p.product_name.Contains(productQuery) ||
                p.product_subject.Contains(productQuery) ||
                p.product_description.Contains(productQuery)
            );

            return productsQuery.ToList();
        }

        // Search from location and product/category and get the matching store  details
        public async Task<List<businessprofile>> SearchBusinessesAsync(string location, string categoryProduct)
        {
            var productResults = new List<products>();
            var busRegIds = new List<int>();
            var businessesByLocation = new List<BusinessRegister>();
            var businessesByCategoryProduct = new List<BusinessRegister>();

            if (!string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
            {
                var subCategory = await _context.product_sub_categories
                    .Where(psc => psc.prod_subcat_name.Contains(categoryProduct))
                    .FirstOrDefaultAsync();

                if (subCategory != null)
                {
                    productResults = await _context.products
                        .Where(p => p.prod_subcat_id == subCategory.prod_subcat_id)
                        .ToListAsync();

                    busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                }
                else
                {
                    var category = await _context.BusinessCategories
                        .Where(c => c.Businesscategory_name.Contains(categoryProduct))
                        .FirstOrDefaultAsync();

                    if (category != null)
                    {
                        productResults = await _context.products
                            .Where(p => p.BuscatId == category.BuscatId)
                            .ToListAsync();

                        busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                    }
                    else
                    {
                        productResults = await _context.products
                            .Where(p => p.product_name.Contains(categoryProduct) ||
                                        p.product_subject.Contains(categoryProduct) ||
                                        p.product_description.Contains(categoryProduct))
                            .ToListAsync();

                        busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                    }
                }
            }

            if (!string.IsNullOrEmpty(location) && location != "null")
            {
                businessesByLocation = await _context.BusinessRegisters
                    .Where(br => br.Town.Contains(location) ||
                                 br.businessCity.Contains(location) ||
                                 br.businessState.Contains(location) ||
                                 br.businessCountry.Contains(location))
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

            var result = new List<businessprofile>();

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
        public List<businessprofile> GetBusinessProfilesByLocation(string location)
        {
            var query =
        from bp in _context.BusinessProfiles
        where bp.business_location.Contains(location)
        select new
        {
            BusinessProfile = bp,
            TotalPurchases = (from o in _context.OrderDetails
                              join pr in _context.products on o.ProductId equals pr.product_id
                              where pr.BusRegId == bp.BusRegId
                              select (int?)o.Quantity).Sum() ?? 0
        };

            var orderedProfiles = query
                .OrderByDescending(x => x.TotalPurchases)
                .Select(x => x.BusinessProfile)
                .ToList();

            return orderedProfiles;

        }

        // stores and profiles based on both search bars - location (optional), search term for product n store
        public SearchResultDto GetBusinessProfilesAndProductsBySearchTerm(string searchTerm, string? locationQuery = null)
        {
            // 1. Find matching categories
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.Businesscategory_name.Contains(searchTerm))
                .Select(bc => bc.BuscatId);

            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct();

            // 2. If no businesses, check subcategories
            if (!businessIds.Any())
            {
                var matchingSubcatIds = _context.product_sub_categories
                    .Where(sc => sc.prod_subcat_name.Contains(searchTerm))
                    .Select(sc => sc.prod_subcat_id);

                businessIds = _context.products
                    .Where(p => matchingSubcatIds.Contains(p.prod_subcat_id))
                    .Select(p => p.BusRegId)
                    .Distinct();
            }

            // 3. If still no businesses, check products directly
            if (!businessIds.Any())
            {
                businessIds = _context.products
                    .Where(p =>
                        p.product_name.Contains(searchTerm) ||
                        p.product_subject.Contains(searchTerm) ||
                        p.product_description.Contains(searchTerm))
                    .Select(p => p.BusRegId)
                    .Distinct();
            }

            // 4. Apply location filtering if provided
            if (!string.IsNullOrEmpty(locationQuery))
            {
                var locationBusinessIds = _context.BusinessRegisters
                    .Where(b =>
                        (b.Town != null && b.Town.Contains(locationQuery)) ||
                        (b.businessCity != null && b.businessCity.Contains(locationQuery)) ||
                        (b.businessState != null && b.businessState.Contains(locationQuery)) ||
                        (b.businessCountry != null && b.businessCountry.Contains(locationQuery)) ||
                        (b.Address1 != null && b.Address1.Contains(locationQuery)) ||
                        (b.Address2 != null && b.Address2.Contains(locationQuery)))
                    .Select(b => b.BusRegId);

                businessIds = businessIds.Intersect(locationBusinessIds);
            }

            var businessIdList = businessIds.ToList();

            // 5. Get matching business profiles
            var stores = _context.BusinessProfiles
                .Where(bp => businessIdList.Contains(bp.BusRegId))
                .ToList();

            // 6. Get matching products (filtered by businessIds)
            var products = _context.products
                .Where(p => businessIdList.Contains(p.BusRegId) &&
                    (p.product_name.Contains(searchTerm) ||
                     p.product_subject.Contains(searchTerm) ||
                     p.product_description.Contains(searchTerm)))
                .Include(p => p.Images)
                .Select(p => new ProductDto
                {
                    ProductId = p.product_id,
                    BusRegId = p.BusRegId,
                    BuscatId = p.BuscatId,
                    ProdSubcatId = p.prod_subcat_id,
                    ProductName = p.product_name,
                    ProductSubject = p.product_subject,
                    ProductDescription = p.product_description,
                    ProductAmount = p.product_cost,
                    Discount = p.discount,
                    DiscountPrice = p.discount_price,
                    Color = p.color,
                    Images = p.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                })
                .ToList();

            // 7. Collect distinct colors
            var colors = products
                .Where(p => !string.IsNullOrEmpty(p.Color))
                .Select(p => p.Color!)
                .Distinct()
                .ToList();

            // 8. Return result
            return new SearchResultDto
            {
                Stores = stores,
                Products = products,
                Colors = colors,
                StoreCount = stores.Count,
                ProductCount = products.Count
            };
        }



        // Fetch Business Profiles and Products based on Product & Location Search Terms
        public SearchResultDto GetBusinessProfilesAndProductsByProductAndLocation(string productSearchTerm, string locationSearchTerm)
        {
            // 1. Match categories
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.Businesscategory_name.Contains(productSearchTerm))
                .Select(bc => bc.BuscatId)
                .ToList();

            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct()
                .ToList();

            // 2. If no businesses, match subcategories
            if (!businessIds.Any())
            {
                var matchingSubcatIds = _context.product_sub_categories
                    .Where(sc => sc.prod_subcat_name.Contains(productSearchTerm))
                    .Select(sc => sc.prod_subcat_id)
                    .ToList();

                businessIds = _context.products
                    .Where(p => matchingSubcatIds.Contains(p.prod_subcat_id))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            // 3. If still no businesses, match products directly
            if (!businessIds.Any())
            {
                businessIds = _context.products
                    .Where(p =>
                        p.product_name.Contains(productSearchTerm) ||
                        p.product_subject.Contains(productSearchTerm) ||
                        p.product_description.Contains(productSearchTerm))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            // 4. Apply location filter directly on business_location
            var finalBusinessIds = _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId) &&
                             bp.business_location.Contains(locationSearchTerm))
                .Select(bp => bp.BusRegId)
                .ToList();


           // var finalBusinessIds = businessIds.Intersect(locationBusinessIds).ToList();

            // 5. Get matching business profiles
            var stores = _context.BusinessProfiles
                .Where(bp => finalBusinessIds.Contains(bp.BusRegId))
                .ToList();

            // 6. Get matching products
            var products = _context.products
                .Where(p => finalBusinessIds.Contains(p.BusRegId) &&
                    (p.product_name.Contains(productSearchTerm) ||
                     p.product_subject.Contains(productSearchTerm) ||
                     p.product_description.Contains(productSearchTerm)))
                .Include(p => p.Images)
                .Select(p => new ProductDto
                {
                    ProductId = p.product_id,
                    BusRegId = p.BusRegId,
                    BuscatId = p.BuscatId,
                    ProdSubcatId = p.prod_subcat_id,
                    ProductName = p.product_name,
                    ProductSubject = p.product_subject,
                    ProductDescription = p.product_description,
                    ProductAmount = p.product_cost,
                    Discount = p.discount,
                    DiscountPrice = p.discount_price,
                    Color = p.color,
                    Images = p.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                })
                .ToList();

            // 7. Collect distinct colors
            var colors = products
                .Where(p => !string.IsNullOrEmpty(p.Color))
                .Select(p => p.Color!)
                .Distinct()
                .ToList();

            // 8. Return both stores & products
            return new SearchResultDto
            {
                Stores = stores,
                Products = products,
                Colors = colors,
                StoreCount = stores.Count,
                ProductCount = products.Count
            };
        }


        public async Task<IEnumerable<product_sub_categories>> GetProductSubCategoriesByLocationAsync(string location)
        {
            // Step 1: Get all business profiles matching location
            var busCatIds = await _context.BusinessProfiles
            .Where(bp => bp.business_location != null &&
                         bp.business_location.Contains(location)) // property name matches model
            .Select(bp => bp.BusCatId)
            .Distinct()
            .ToListAsync();

            if (!busCatIds.Any())
                return new List<product_sub_categories>();

            // Step 2: Get all product_sub_categories for those categories
            var subCategories = await _context.product_sub_categories
                .Where(sc => busCatIds.Contains(sc.BuscatId))
                .Select(sc => new product_sub_categories
                {
                    prod_subcat_id = sc.prod_subcat_id,
                    prod_subcat_name = sc.prod_subcat_name,
                    prod_subcat_image = sc.prod_subcat_image,
                    BuscatId = sc.BuscatId
                })
                .ToListAsync();

            return subCategories;
        }


        public async Task<IEnumerable<businesscategoriescs>> GetBusinessCategoriesByLocationAsync(string location)
        {
            // Step 1: Get all business profiles matching the location
            var busCatIds = await _context.BusinessProfiles
                .Where(bp => bp.business_location != null &&
                             bp.business_location.Contains(location))
                .Select(bp => bp.BusCatId)
                .Distinct()
                .ToListAsync();

            if (!busCatIds.Any())
                return new List<businesscategoriescs>();

            // Step 2: Get all business categories for those BusCatIds
            var categories = await _context.BusinessCategories
                .Where(bc => busCatIds.Contains(bc.BuscatId))
                .Select(bc => new businesscategoriescs
                {
                    BuscatId = bc.BuscatId,
                    Businesscategory_name = bc.Businesscategory_name
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
                    (b.businessCity != null && b.businessCity.Contains(locationQuery)) ||
                    (b.businessState != null && b.businessState.Contains(locationQuery)) ||
                    (b.businessCountry != null && b.businessCountry.Contains(locationQuery)) ||
                    (b.Address1 != null && b.Address1.Contains(locationQuery)) ||
                    (b.Address2 != null && b.Address2.Contains(locationQuery)))
                .Select(b => b.BusRegId)
                .ToList();

            if (!locationBusinessIds.Any())
            {
                return new SearchResultDto
                {
                    Stores = new List<businessprofile>(),
                    Products = new List<ProductDto>(),
                    Colors = new List<string>(),
                    StoreCount = 0,
                    ProductCount = 0
                };
            }

            // 2. Filter stores by store name OR products/categories inside those stores
            var storeMatches = _context.BusinessProfiles
                .Where(bp => locationBusinessIds.Contains(bp.BusRegId) &&
                             (bp.BusinessUsername.Contains(searchTerm) ||
                              bp.business_about.Contains(searchTerm)))
                .ToList();

            var productMatches = _context.products
                .Where(p => locationBusinessIds.Contains(p.BusRegId) &&
                            (p.product_name.Contains(searchTerm) ||
                             p.product_subject.Contains(searchTerm) ||
                             p.product_description.Contains(searchTerm)))
                .Include(p => p.Images)
                .Select(p => new ProductDto
                {
                    ProductId = p.product_id,
                    BusRegId = p.BusRegId,
                    BuscatId = p.BuscatId,
                    ProdSubcatId = p.prod_subcat_id,
                    ProductName = p.product_name,
                    ProductSubject = p.product_subject,
                    ProductDescription = p.product_description,
                    ProductAmount = p.product_cost,
                    Discount = p.discount,
                    DiscountPrice = p.discount_price,
                    Color = p.color,
                    Images = p.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                })
                .ToList();

            // 3. Distinct colors
            var colors = productMatches
                .Where(p => !string.IsNullOrEmpty(p.Color))
                .Select(p => p.Color!)
                .Distinct()
                .ToList();

            // 4. Return combined result
            return new SearchResultDto
            {
                Stores = storeMatches,
                Products = productMatches,
                Colors = colors,
                StoreCount = storeMatches.Count,
                ProductCount = productMatches.Count
            };
        }

    }
}
