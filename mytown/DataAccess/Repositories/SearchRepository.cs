using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;
using System.Diagnostics.Metrics;

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

        // Search from location and product/category and get the matching store details
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
                && bp.ProfileStatus == "approved"
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

        // stores and profiles based on both search bars - location (optional), search term for product n store
        public SearchResultDto GetBusinessProfilesAndProductsBySearchTerm(string searchTerm, string? locationQuery = null)
        {
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.BusinessCategoryName.Contains(searchTerm))
                .Select(bc => bc.BusCatId);

            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct();

            if (!businessIds.Any())
            {
                var matchingSubcatIds = _context.product_sub_categories
                    .Where(sc => sc.ProdSubcatName.Contains(searchTerm))
                    .Select(sc => sc.ProdSubcatId);

                businessIds = _context.products
                    .Where(p => matchingSubcatIds.Contains(p.ProdSubcatId))
                    .Select(p => p.BusRegId)
                    .Distinct();
            }

            if (!businessIds.Any())
            {
                businessIds = _context.products
                    .Where(p =>
                        p.ProductName.Contains(searchTerm) ||
                        p.ProductSubject.Contains(searchTerm) ||
                        p.ProductDescription.Contains(searchTerm))
                    .Select(p => p.BusRegId)
                    .Distinct();
            }

            var skuBusinessIds = _context.Sku_ProductVariants
                .Include(v => v.Product)
                .Where(v =>
                    v.Color.Contains(searchTerm) ||
                    (v.Size != null && v.Size.SizeName.Contains(searchTerm)))
                .Select(v => v.Product.BusRegId)
                .Distinct();

            businessIds = businessIds.Union(skuBusinessIds);

            if (!string.IsNullOrEmpty(locationQuery))
            {
                var locationBusinessIds = _context.BusinessRegisters
                    .Where(b =>
                        (b.Town != null && b.Town.Contains(locationQuery)) ||
                        (b.BusinessCity != null && b.BusinessCity.Contains(locationQuery)) ||
                        (b.BusinessState != null && b.BusinessState.Contains(locationQuery)) ||
                        (b.BusinessCountry != null && b.BusinessCountry.Contains(locationQuery)) ||
                        (b.Address1 != null && b.Address1.Contains(locationQuery)) ||
                        (b.Address2 != null && b.Address2.Contains(locationQuery)))
                    .Select(b => b.BusRegId);

                businessIds = businessIds.Intersect(locationBusinessIds);
            }

            var businessIdList = businessIds.ToList();

            var stores = _context.BusinessProfiles
                .Where(bp => businessIdList.Contains(bp.BusRegId))
                .ToList();

            var products = _context.products
                .Where(p => businessIdList.Contains(p.BusRegId) &&
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
                    ProdcatId = p.ProdSubcatId,
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

            var colors = products
                .SelectMany(p => p.Variants)
                .Where(v => !string.IsNullOrEmpty(v.Color))
                .Select(v => v.Color!)
                .Distinct()
                .ToList();

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
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.BusinessCategoryName.Contains(productSearchTerm))
                .Select(bc => bc.BusCatId)
                .ToList();

            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct()
                .ToList();

            if (!businessIds.Any())
            {
                var matchingSubcatIds = _context.product_sub_categories
                    .Where(sc => sc.ProdSubcatName.Contains(productSearchTerm))
                    .Select(sc => sc.ProdSubcatId)
                    .ToList();

                businessIds = _context.products
                    .Where(p => matchingSubcatIds.Contains(p.ProdSubcatId))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            if (!businessIds.Any())
            {
                businessIds = _context.products
                    .Where(p =>
                        p.ProductName.Contains(productSearchTerm) ||
                        p.ProductSubject.Contains(productSearchTerm) ||
                        p.ProductDescription.Contains(productSearchTerm))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            var finalBusinessIds = _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId) &&
                             bp.BusinessLocation.Contains(locationSearchTerm))
                .Select(bp => bp.BusRegId)
                .ToList();

            var stores = _context.BusinessProfiles
                .Where(bp => finalBusinessIds.Contains(bp.BusRegId) && bp.ProfileStatus == "approved")
                .ToList();

            var products = _context.products
                .Where(p => finalBusinessIds.Contains(p.BusRegId) &&
                    (p.ProductName.Contains(productSearchTerm) ||
                     p.ProductSubject.Contains(productSearchTerm) ||
                     p.ProductDescription.Contains(productSearchTerm) ||
                     p.Sku_ProductVariants.Any(v =>
                         v.Color.Contains(productSearchTerm) ||
                         (v.Size != null && v.Size.SizeName.Contains(productSearchTerm)))))
                .Include(p => p.Sku_ProductVariants)
                    .ThenInclude(v => v.Images)
                .Select(p => new ProdcVariantforShopperDto
                {
                    ProductId = p.ProductId,
                    BusRegId = p.BusRegId,
                    BusinessName = p.BusinessRegister.BusinessName,
                    BuscatId = p.BuscatId,
                    ProdcatId = p.ProdSubcatId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductName,
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

            var colors = products
                .SelectMany(p => p.Variants)
                .Where(v => !string.IsNullOrEmpty(v.Color))
                .Select(v => v.Color!)
                .Distinct()
                .ToList();

            return new SearchResultDto
            {
                Stores = stores,
                Products = products,
                Colors = colors,
                StoreCount = stores.Count,
                ProductCount = products.Count
            };
        }

        public async Task<IEnumerable<ProductSubCategory>> GetProductSubCategoriesByLocationAsync(string location)
        {
            var busCatIds = await _context.BusinessProfiles
                .Where(bp => bp.BusinessLocation != null &&
                             bp.BusinessLocation.Contains(location))
                .Select(bp => bp.BusCatId)
                .Distinct()
                .ToListAsync();

            if (!busCatIds.Any())
                return new List<ProductSubCategory>();

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
            var busCatIds = await _context.BusinessProfiles
                .Where(bp => bp.BusinessLocation != null &&
                             bp.BusinessLocation.Contains(location))
                .Select(bp => bp.BusCatId)
                .Distinct()
                .ToListAsync();

            if (!busCatIds.Any())
                return new List<BusinessCategory>();

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

        public List<BusinessProfile> GetBusinessProfilesByFilters(string? searchTerm, string? locationQuery)
        {
            IQueryable<int> businessIds = Enumerable.Empty<int>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                var storeNameIds = _context.BusinessRegisters
                    .Where(b => b.BusinessName.Contains(searchTerm))
                    .Select(b => b.BusRegId);

                var categoryIds = _context.BusinessCategories
                    .Where(c => c.BusinessCategoryName.Contains(searchTerm))
                    .Select(c => c.BusCatId);

                var categoryBusinessIds = _context.products
                    .Where(p => categoryIds.Contains(p.BuscatId))
                    .Select(p => p.BusRegId);

                var subcategoryIds = _context.product_sub_categories
                    .Where(sc => sc.ProdSubcatName.Contains(searchTerm))
                    .Select(sc => sc.ProdSubcatId);

                var subcategoryBusinessIds = _context.products
                    .Where(p => subcategoryIds.Contains(p.ProdSubcatId))
                    .Select(p => p.BusRegId);

                var productFieldBusinessIds = _context.products
                    .Where(p =>
                        p.ProductName.Contains(searchTerm) ||
                        p.ProductSubject.Contains(searchTerm) ||
                        p.ProductDescription.Contains(searchTerm))
                    .Select(p => p.BusRegId);

                var skuBusinessIds = _context.Sku_ProductVariants
                    .Where(v =>
                        v.Color.Contains(searchTerm) ||
                        (v.Size != null && v.Size.SizeName.Contains(searchTerm)))
                    .Select(v => v.Product.BusRegId);

                businessIds = storeNameIds
                    .Union(categoryBusinessIds)
                    .Union(subcategoryBusinessIds)
                    .Union(productFieldBusinessIds)
                    .Union(skuBusinessIds)
                    .Distinct();
            }

            if (!string.IsNullOrWhiteSpace(locationQuery))
            {
                locationQuery = locationQuery.Trim();

                var locationBusinessIds = _context.BusinessRegisters
                    .Where(b =>
                            b.Town.ToLower().Contains(locationQuery.ToLower()) ||
                            b.BusinessCity.ToLower().Contains(locationQuery.ToLower()) ||
                            b.BusinessState.ToLower().Contains(locationQuery.ToLower()) ||
                            b.BusinessCountry.ToLower().Contains(locationQuery.ToLower()) ||
                        (b.Address1 != null && b.Address1.Contains(locationQuery)) ||
                        (b.Address2 != null && b.Address2.Contains(locationQuery)))
                    .Select(b => b.BusRegId);

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    businessIds = locationBusinessIds;
                }
                else
                {
                    businessIds = businessIds.Intersect(locationBusinessIds);
                }
            }

            return _context.BusinessProfiles
                .Where(bp =>
                    businessIds.Contains(bp.BusRegId) &&
                     bp.ProfileStatus.ToLower() == "approved")
                .ToList();
        }

        public async Task<IEnumerable<BusinessCategory>> GetBusinessCategoriesByProductAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<BusinessCategory>();

            var busCatIds = await _context.products
                .Where(p => p.ProductName.Contains(productName) ||
                            p.ProductSubject.Contains(productName))
                .Select(p => p.BusRegId)
                .Distinct()
                .Join(_context.BusinessProfiles,
                      productStoreId => productStoreId,
                      store => store.BusRegId,
                      (productStoreId, store) => store.BusCatId)
                .Distinct()
                .ToListAsync();

            if (!busCatIds.Any())
                return new List<BusinessCategory>();

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

        // 27-05-26
        // get both business profiles and service profiles
        public async Task<BusinessAndServiceSearchResultsDto>
 GetBusinessAndServiceSearchResults(
     string? searchTerm,
     string? town,
     string? city,
     string? state,
     string? country)
        {
            IQueryable<int> businessIds = Enumerable.Empty<int>().AsQueryable();
            IQueryable<int> serviceBusinessIds = Enumerable.Empty<int>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                var storeNameIds = _context.BusinessRegisters
                    .Where(b => EF.Functions.Like(b.BusinessName, $"%{searchTerm}%"))
                    .Select(b => b.BusRegId);

                var categoryIds = _context.BusinessCategories
                    .Where(c => EF.Functions.Like(c.BusinessCategoryName, $"%{searchTerm}%"))
                    .Select(c => c.BusCatId);

                var categoryBusinessIds = _context.products
                    .Where(p => categoryIds.Contains(p.BuscatId))
                    .Select(p => p.BusRegId);

                var subcategoryIds = _context.product_sub_categories
                    .Where(sc => EF.Functions.Like(sc.ProdSubcatName, $"%{searchTerm}%"))
                    .Select(sc => sc.ProdSubcatId);

                var subcategoryBusinessIds = _context.products
                    .Where(p => subcategoryIds.Contains(p.ProdSubcatId))
                    .Select(p => p.BusRegId);

                var productFieldBusinessIds = _context.products
                    .Where(p =>
                        EF.Functions.Like(p.ProductName, $"%{searchTerm}%") ||
                        EF.Functions.Like(p.ProductSubject, $"%{searchTerm}%") ||
                        EF.Functions.Like(p.ProductDescription, $"%{searchTerm}%"))
                    .Select(p => p.BusRegId);

                var skuBusinessIds = _context.Sku_ProductVariants
                    .Where(v =>
                        EF.Functions.Like(v.Color, $"%{searchTerm}%") ||
                        (v.Size != null && EF.Functions.Like(v.Size.SizeName, $"%{searchTerm}%")))
                    .Select(v => v.Product.BusRegId);

                businessIds = storeNameIds
                    .Union(categoryBusinessIds)
                    .Union(subcategoryBusinessIds)
                    .Union(productFieldBusinessIds)
                    .Union(skuBusinessIds)
                    .Distinct();

                var serviceBusinessNameIds = _context.BusinessRegisters
                    .Where(b => EF.Functions.Like(b.BusinessName, $"%{searchTerm}%"))
                    .Select(b => b.BusRegId);

                var serviceCategoryIds = _context.BusinessServices
                    .Where(bs => EF.Functions.Like(bs.BusinessServiceName, $"%{searchTerm}%"))
                    .Select(bs => bs.BusServId);

                var serviceCategoryBusinessIds = _context.Service
                    .Where(s => serviceCategoryIds.Contains(s.BusServId))
                    .Select(s => s.BusRegId);

                var serviceSubcategoryIds = _context.ServiceSubCategory
                    .Where(ss => EF.Functions.Like(ss.ServiceTypeName, $"%{searchTerm}%"))
                    .Select(ss => ss.ServSubcatId);

                var serviceSubcategoryBusinessIds = _context.Service
                    .Where(s => serviceSubcategoryIds.Contains(s.ServSubcatId))
                    .Select(s => s.BusRegId);

                var serviceLocationBusinessIds = _context.ServiceProfiles
                    .Where(sp =>
                        sp.ServiceAvailableLocations != null &&
                        EF.Functions.Like(sp.ServiceAvailableLocations, $"%{searchTerm}%"))
                    .Select(sp => sp.BusRegId);

                serviceBusinessIds = serviceBusinessNameIds
                    .Union(serviceCategoryBusinessIds)
                    .Union(serviceSubcategoryBusinessIds)
                    .Union(serviceLocationBusinessIds)
                    .Distinct();
            }

            if (!string.IsNullOrWhiteSpace(town) ||
                !string.IsNullOrWhiteSpace(city) ||
                !string.IsNullOrWhiteSpace(state) ||
                !string.IsNullOrWhiteSpace(country))
            {
                var locationBusinessIds = _context.BusinessRegisters
                    .Where(b =>
                        (string.IsNullOrWhiteSpace(town) || b.Town == town) &&
                        (string.IsNullOrWhiteSpace(city) || b.BusinessCity == city) &&
                        (string.IsNullOrWhiteSpace(state) || b.BusinessState == state) &&
                        (string.IsNullOrWhiteSpace(country) || b.BusinessCountry == country))
                    .Select(b => b.BusRegId);

                businessIds = string.IsNullOrWhiteSpace(searchTerm)
                    ? locationBusinessIds
                    : businessIds.Intersect(locationBusinessIds);

                serviceBusinessIds = string.IsNullOrWhiteSpace(searchTerm)
                    ? locationBusinessIds
                    : serviceBusinessIds.Intersect(locationBusinessIds);
            }

            var businessProfiles = await _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId) && bp.ProfileStatus == "Approved")
                .ToListAsync();

            var serviceProfiles = await (
                from sp in _context.ServiceProfiles
                join br in _context.BusinessRegisters on sp.BusRegId equals br.BusRegId
                where serviceBusinessIds.Contains(sp.BusRegId) && sp.Status == "Approved"
                select new ServiceProfile
                {
                    ServiceProfileId = sp.ServiceProfileId,
                    BusRegId = sp.BusRegId,
                    BusServId = sp.BusServId,
                    YearsOfExperience = sp.YearsOfExperience,
                    GovtIdDocument = sp.GovtIdDocument,
                    ProfessionalLicense = sp.ProfessionalLicense,
                    ServiceAvailableLocations = sp.ServiceAvailableLocations,
                    WorkingDays = sp.WorkingDays,
                    WorkingStartTime = sp.WorkingStartTime,
                    WorkingEndTime = sp.WorkingEndTime,
                    CreatedDate = sp.CreatedDate,
                    ServiceLogo = sp.ServiceLogo,
                    ServiceBanner = sp.ServiceBanner,
                    Status = sp.Status,
                    BusinessName = br.BusinessName,
                    BusinessLocation =
                        (br.Address1 ?? "") +
                        (!string.IsNullOrEmpty(br.Address2) ? ", " + br.Address2 : "") +
                        (!string.IsNullOrEmpty(br.Town) ? ", " + br.Town : "") +
                        (!string.IsNullOrEmpty(br.BusinessCity) ? ", " + br.BusinessCity : "") +
                        (!string.IsNullOrEmpty(br.BusinessState) ? ", " + br.BusinessState : "") +
                        (!string.IsNullOrEmpty(br.BusinessCountry) ? ", " + br.BusinessCountry : "")
                })
                .ToListAsync();

            return new BusinessAndServiceSearchResultsDto
            {
                BusinessProfiles = businessProfiles,
                ServiceProfiles = serviceProfiles
            };
        }

        // Track order by tracking ID
       public async Task<TrackingResultDto> TrackOrderByTrackingIdAsync(string trackingId)
{
    var shipping = await _context.ShippingDetails
        .Include(s => s.Order)
            .ThenInclude(o => o.ShopperRegister)
        .Include(s => s.Order)
            .ThenInclude(o => o.GuestRegister)
        .FirstOrDefaultAsync(s => s.TrackingId == trackingId);

    if (shipping == null)
        return null;

    var order = shipping.Order;
    string customerName, customerEmail, customerPhone;

    if (order.IsGuestOrder && order.GuestRegister != null)
    {
        customerName = order.GuestRegister.Username;
        customerEmail = order.GuestRegister.Email;
        customerPhone = order.GuestRegister.PhoneNumber;
    }
    else if (order.ShopperRegister != null)
    {
        customerName = order.ShopperRegister.Username;
        customerEmail = order.ShopperRegister.Email;
        customerPhone = order.ShopperRegister.PhoneNumber;
    }
    else
    {
        customerName = "Unknown";
        customerEmail = "Unknown";
        customerPhone = "Unknown";
    }

    var products = await (
        from od in _context.OrderDetails
        join p in _context.products on od.ProductId equals p.ProductId

        join sku in _context.Sku_ProductVariants
            on od.SkuId equals sku.SkuId into skuGroup
        from sku in skuGroup.DefaultIfEmpty()

        join img in _context.ProductImages
            on sku.SkuId equals img.SkuId into imgGroup
        from img in imgGroup
            .OrderBy(i => i.SortOrder)
            .Take(1)
            .DefaultIfEmpty()

        where od.OrderId == order.OrderId

        select new TrackingProductDto
        {
            ProductId = p.ProductId,
            SkuId = od.SkuId,
            ProductName = p.ProductName,
            Quantity = od.Quantity,
            ProductCost = od.Price,
            ProductImage = img != null ? img.FileName : p.ProductImage
        }
    ).ToListAsync();

    // Store Details
    var firstOrderDetail = await _context.OrderDetails
        .FirstOrDefaultAsync(x => x.OrderId == order.OrderId);

    BusinessRegister? store = null;
    BusinessProfile? storeProfile = null;

    if (firstOrderDetail != null)
    {
        store = await _context.BusinessRegisters
            .FirstOrDefaultAsync(x => x.BusRegId == firstOrderDetail.StoreId);

        if (store != null)
        {
            storeProfile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(x => x.BusRegId == store.BusRegId);
        }
    }

    // Transporter Details
    TransporterRegister? transporter = null;

    if (shipping.TransporterRegId.HasValue)
    {
        transporter = await _context.TransporterRegisters
            .FirstOrDefaultAsync(x =>
                x.TransporterRegId == shipping.TransporterRegId.Value);
    }

    // Courier Branch Details
    CourierBranch? courierBranch = null;

    if (shipping.BranchId.HasValue)
    {
        courierBranch = await _context.CourierBranches
            .FirstOrDefaultAsync(x => x.BranchId == shipping.BranchId.Value);
    }

        // Travel Plan Details
        TransporterTravelPlan? transporterPlan = null;

        if (shipping.TransporterPlanId.HasValue)
        {
            transporterPlan = await _context.TransporterTravelPlans
                .FirstOrDefaultAsync(x => x.PlanId == shipping.TransporterPlanId.Value);
        }

    return new TrackingResultDto
    {
        TrackingId = shipping.TrackingId,
        ShippingStatus = shipping.ShippingStatus,
        ShippingType = shipping.ShippingType,
        EstimatedDays = shipping.EstimatedDays,
        DeliveredDate = shipping.DeliveredDate,
        DeliveryAddress = shipping.DeliveryAddress,

        OrderId = order.OrderId,
        OrderStatus = order.OrderStatus,
        TotalAmount = order.TotalAmount,
        OrderDate = order.OrderDate,
        IsGuestOrder = order.IsGuestOrder,

        CustomerName = customerName,
        CustomerEmail = customerEmail,
        CustomerPhone = customerPhone,

        // Store Details
        StoreId = store?.BusRegId,
        StoreName = store?.BusinessName,
        StorePhone = store?.BusMobileNo,
        StoreEmail = store?.BusEmail,
        StoreAddress = storeProfile?.BusinessLocation,
        StoreLogo = storeProfile?.LogoPath,
        StoreBanner = storeProfile?.BannerPath,
        StoreDescription = storeProfile?.BusinessAbout,


    TransporterAddress = transporter != null
    ? $"{transporter.Address}, {transporter.Town}, {transporter.City}, {transporter.State}, {transporter.Country}"
    : null,

    // Courier Details
    CourierName = courierBranch?.CourierServiceName,
    BranchContactPerson = courierBranch?.BranchContactPerson,
    BranchEmail = courierBranch?.BranchEmailId,
    BranchPhoneNumber = courierBranch?.BranchPhoneNumber,

    // Travel Plan Details
    VehicleType = transporterPlan?.VehicleType,
    VehicleName = transporterPlan?.VehicleName,
    PreferredRoute = transporterPlan?.PreferredRoute,

    Products = products
    };
}

        //  New method - Get popular cities from different countries
        public async Task<IEnumerable<PopularCityDto>> GetPopularCitiesAsync()
        {
            return await _context.BusinessRegisters
                .Where(br => br.BusinessCity != null
                          && br.BusinessCountry != null)
                .GroupBy(br => new { br.BusinessCity, br.BusinessCountry })
                .Select(g => new PopularCityDto
                {
                    City = g.Key.BusinessCity,
                    Country = g.Key.BusinessCountry,
                    StoreCount = g.Count()
                })
                .OrderByDescending(x => x.StoreCount)
                .ToListAsync();
        }

        public async Task<SenderOrderTrackingDto?> GetSenderOrderTrackingAsync(string trackingId)
{
    var order = await _context.SenderOrders
        .FirstOrDefaultAsync(x => x.TrackingId == trackingId);

    if (order == null)
        return null;

    TransporterRegister? transporter = null;

    if (order.TransporterRegId.HasValue)
    {
        transporter = await _context.TransporterRegisters
            .FirstOrDefaultAsync(x =>
                x.TransporterRegId == order.TransporterRegId.Value);
    }

    TransporterTravelPlan? plan = null;

    if (order.TransporterPlanId.HasValue)
    {
        plan = await _context.TransporterTravelPlans
            .FirstOrDefaultAsync(x =>
                x.PlanId == order.TransporterPlanId.Value);
    }

    return new SenderOrderTrackingDto
    {
        SenderOrderId = order.SenderOrderId,
        TrackingId = order.TrackingId,

        ProductName = order.ProductName,
        ProductCost = order.ProductCost,

        PackageLength = order.PackageLength,
        PackageWidth = order.PackageWidth,
        PackageHeight = order.PackageHeight,
        PackageWeight = order.PackageWeight,

        IsFragile = order.IsFragile,
        IsPerishable = order.IsPerishable,
        SpecialInstructions = order.SpecialInstructions,

        PickupAddress = order.PickupAddress,
        PickupTown = order.PickupTown,
        PickupCity = order.PickupCity,
        PickupState = order.PickupState,
        PickupCountry = order.PickupCountry,
        PickupPincode = order.PickupPincode,

        PickupDate = order.PickupDate,
        PickupTime = order.PickupTime,

        ReceiverName = order.ReceiverName,
        ReceiverPhone = order.ReceiverPhone,
        ReceiverAddress = order.ReceiverAddress,
        ReceiverTown = order.ReceiverTown,
        ReceiverCity = order.ReceiverCity,
        ReceiverState = order.ReceiverState,
        ReceiverCountry = order.ReceiverCountry,
        ReceiverPincode = order.ReceiverPincode,

        OrderStatus = order.OrderStatus,
        DeliveryStatus = order.DeliveryStatus,

        TransporterName = transporter?.TransporterName,
        TransporterPhone = transporter?.PhoneNumber,
        TransporterEmail = transporter?.Email,

        VehicleType = plan?.VehicleType,
        VehicleName = plan?.VehicleName,
        PreferredRoute = plan?.PreferredRoute
    };
}


    }
}
