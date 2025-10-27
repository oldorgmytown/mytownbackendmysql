using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using System.Globalization;
using static mytown.Models.busprofilepreview;

namespace mytown.DataAccess.Repositories
{
    public class BusinessProfileRepository : IBusinessProfileRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public BusinessProfileRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        

        // Get all business profiles including related BusinessRegister data
        public async Task<IEnumerable<BusinessProfile>> GetAllBusinessProfilesAsync()
        {
            return await _context.BusinessProfiles
                .Include(bp => bp.BusinessRegister) //  load BusinessRegister
                .ToListAsync();
        }

        public async Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId)
        {
            var result = await (
                from bp in _context.BusinessProfiles
                join br in _context.BusinessRegisters
                    on bp.BusRegId equals br.BusRegId
                join bs in _context.BusinessServices
                    on bp.BusServId equals bs.BusServId into bsGroup
                from bs in bsGroup.DefaultIfEmpty()
                join bc in _context.BusinessCategories
                    on bp.BusCatId equals bc.BusCatId into bcGroup
                from bc in bcGroup.DefaultIfEmpty()
                where bp.BusRegId == busRegId
                select new busprofilepreview
                {
                    businessprofile_id = bp.BusinessProfileId,
                    BusRegId = bp.BusRegId,
                    Businessname = br.BusinessName, // From BusinessRegister table
                    Businessusername = br.BusinessUsername, // from business register table
                    business_location = bp.BusinessLocation,
                    business_about = bp.BusinessAbout,
                    banner_path = bp.BannerPath,
                    logo_path = bp.LogoPath,
                    profile_status = bp.ProfileStatus,
                    BusCatId = bp.BusCatId,
                    BusServId = bp.BusServId,
                    Businessservice_name = bs != null ? bs.BusinessServiceName : null,
                    Businesscategory_name = bc != null ? bc.BusinessCategoryName : null,
                    Currency = br.Currency // from BusinessRegister table
                }
            ).ToListAsync();

            return result;
        }




        //get products for selected category
        public IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
        {
            return _context.products
                           .Where(p => p.BusRegId == busRegId && p.prod_subcat_id == prodSubcatId)
                           .ToList();
        }

        // adding business profile data to DB
        //public async Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile)
        //{
        //    // Check if the business profile with the given BusRegId already exists
        //    var existingProfile = await _context.BusinessProfiles
        //        .FirstOrDefaultAsync(bp => bp.BusRegId == businessProfile.BusRegId);

        //    if (existingProfile != null)
        //    {
        //        // Updating an existing profile
        //        existingProfile.BusinessUsername = businessProfile.BusinessUsername;
        //        existingProfile.business_location = businessProfile.business_location;
        //        existingProfile.business_about = businessProfile.business_about;
        //        existingProfile.banner_path = businessProfile.banner_path;
        //        existingProfile.profile_status = businessProfile.profile_status;
        //        existingProfile.bus_time = businessProfile.bus_time;
        //        existingProfile.BusCatId = businessProfile.BusCatId;
        //        existingProfile.BusServId = businessProfile.BusServId;

        //        // Update image position & zoom
        //        existingProfile.image_positionx = businessProfile.image_positionx;
        //        existingProfile.image_positiony = businessProfile.image_positiony;
        //        existingProfile.zoom = businessProfile.zoom;

        //        // Mark entity as modified
        //        _context.BusinessProfiles.Update(existingProfile);
        //    }
        //    else
        //    {
        //        // Set default values if they are not provided
        //        if (businessProfile.image_positionx == 0 && businessProfile.image_positiony == 0 && businessProfile.zoom == 0)
        //        {
        //            businessProfile.image_positionx = 0;
        //            businessProfile.image_positiony = 0;
        //            businessProfile.zoom = 1; // Default zoom value
        //        }

        //        // Add a new profile
        //        await _context.BusinessProfiles.AddAsync(businessProfile);
        //    }

        //    // Save changes asynchronously
        //    await _context.SaveChangesAsync();

        //    // Return the updated or newly added profile
        //    return existingProfile ?? businessProfile;
        //}

        public async Task<BusinessProfile> AddBusinessProfileAsync(BusinessProfile businessProfile)
        {
            var existingProfile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(bp => bp.BusRegId == businessProfile.BusRegId);

            if (existingProfile != null)
            {
                // Update only if values are provided
                if (!string.IsNullOrEmpty(businessProfile.BusinessName))
                    existingProfile.BusinessName = businessProfile.BusinessName;

                if (!string.IsNullOrEmpty(businessProfile.BusinessLocation))
                    existingProfile.BusinessLocation = businessProfile.BusinessLocation;

                if (!string.IsNullOrEmpty(businessProfile.BusinessAbout))
                    existingProfile.BusinessAbout = businessProfile.BusinessAbout;

                if (!string.IsNullOrEmpty(businessProfile.BannerPath))
                {
                    // If a new banner is uploaded
                    if (!string.IsNullOrEmpty(existingProfile.BannerPath))
                    {
                        // Delete old banner from blob
                        await DeleteFromBlobAsync(existingProfile.BannerPath);
                    }
                    existingProfile.BannerPath = businessProfile.BannerPath;
                }

                if (!string.IsNullOrEmpty(businessProfile.LogoPath))
                {
                    // If a new logo is uploaded
                    if (!string.IsNullOrEmpty(existingProfile.LogoPath))
                    {
                        // Delete old logo from blob
                        await DeleteFromBlobAsync(existingProfile.LogoPath);
                    }
                    existingProfile.LogoPath = businessProfile.LogoPath;
                }

                if (!string.IsNullOrEmpty(businessProfile.ProfileStatus))
                    existingProfile.ProfileStatus = businessProfile.ProfileStatus;

                //if (!string.IsNullOrEmpty(businessProfile.bus_time))
                //    existingProfile.bus_time = businessProfile.bus_time;

                if (businessProfile.BusCatId != 0)
                    existingProfile.BusCatId = businessProfile.BusCatId;

                if (businessProfile.BusServId != 0)
                    existingProfile.BusServId = businessProfile.BusServId;

                //if (!string.IsNullOrEmpty(businessProfile.Businessservice_name))
                //    existingProfile.Businessservice_name = businessProfile.Businessservice_name;

                //if (!string.IsNullOrEmpty(businessProfile.Businesscategory_name))
                //    existingProfile.Businesscategory_name = businessProfile.Businesscategory_name;

               

                _context.BusinessProfiles.Update(existingProfile);
            }
            else
            {
                // Add new profile
                await _context.BusinessProfiles.AddAsync(businessProfile);
            }

            await _context.SaveChangesAsync();
            return existingProfile ?? businessProfile;
        }


        public async Task DeleteFromBlobAsync(string fileName)
        {
            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> UploadToBlobAsync(IFormFile file, string imageType)
        {
            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var newFileName = $"{imageType}_{fileNameWithoutExtension}_{timestamp}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(newFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return newFileName; // return file name (store in DB)
        }


        public async Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath)
        {
            var business = await _context.BusinessProfiles
                                         .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            if (business == null)
                return false; // Business not found

            business.BannerPath = bannerPath;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath)
        {
            var business = await _context.BusinessProfiles
                                         .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            if (business == null)
                return false; // Business not found

            business.LogoPath = logoPath;
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all product subcategories
        public async Task<IEnumerable<product_sub_categories>> GetAllSubCategoriesAsync()
        {
            return await _context.product_sub_categories.ToListAsync();
        }
        //get categories ofproducts for a businessid
        public List<product_sub_categories> GetProductSubCategoriesByBusRegId(int busRegId)
        {
            var result = (from product in _context.products
                          join subCategory in _context.product_sub_categories
                          on product.prod_subcat_id equals subCategory.prod_subcat_id
                          join subCatImage in _context.Subcategoryimages_Busregids
                          on new { product.BusRegId, ProdSubCatId = subCategory.prod_subcat_id }
                          equals new { subCatImage.BusRegId, ProdSubCatId = subCatImage.Prod_subcat_id }
                          into subCatImageGroup
                          from subCatImage in subCatImageGroup.DefaultIfEmpty()
                          where product.BusRegId == busRegId
                          select new product_sub_categories
                          {
                              prod_subcat_id = subCategory.prod_subcat_id,
                              prod_subcat_name = subCategory.prod_subcat_name,
                              prod_subcat_image = subCatImage != null ? subCatImage.Prod_subcat_image : null
                          })
                          .Distinct()
                          .ToList();

            return result;
        }

        // //get product category details like type, fabric,design on add product form

        public async Task<ProductDetailsDto> GetDetailsBySubCategoryAsync(int prodSubcatId)
        {

            //var subcat = await _context.product_sub_categories
            //                  .Where(sc => sc.prod_subcat_id == prodSubcatId)
            //                  .Select(sc => new ProductSubCategories
            //                  {
            //                      ProdSubcatId = sc.prod_subcat_id,
            //                      ProdSubcatName = sc.prod_subcat_name
            //                  })
            //                  .FirstOrDefaultAsync();
            var types = await _context.Product_Types
                                      .Where(pt => pt.prod_subcat_id == prodSubcatId)
                                      .OrderBy(pt => pt.prod_type_name)
                                      .ToListAsync();

            var fabrics = await _context.Fabrics
                                        .Where(f => f.prod_subcat_id == prodSubcatId)
                                        .OrderBy(f => f.fabric_name)
                                        .ToListAsync();

            var designs = await _context.Designs
                                        .Where(d => d.prod_subcat_id == prodSubcatId)
                                        .OrderBy(d => d.design_name)
                                        .ToListAsync();


            var sizes = await _context.Product_Sizes      
                                      .Where(s => s.prod_subcat_id == prodSubcatId)
                                      .OrderBy(s => s.SizeName)
                                      .ToListAsync();

            return new ProductDetailsDto
            {
                ProdSubcatId = prodSubcatId,
                ProductTypes = types,
                Fabrics = fabrics,
                Designs = designs,
                Sizes = sizes          
            };
        }
    


        // businessprofiels with discount products
        //public async Task<IEnumerable<businessprofile>> GetBusinessProfilesWithDiscountedProductsAsync()
        //{
        //    // Step 1: Get distinct business ids from products having discounts
        //    var businessIdsWithDiscounts = await _context.products
        //        .Where(p => p.discount.HasValue && p.discount > 0) // only products with valid discount
        //        .Select(p => p.BusRegId)
        //        .Distinct()
        //        .ToListAsync();

        //    // Step 2: Fetch business profiles for those business ids
        //    var businessProfiles = await _context.BusinessProfiles
        //        .Where(bp => businessIdsWithDiscounts.Contains(bp.BusRegId))
        //        .ToListAsync();

        //    return businessProfiles;
        //}

        public async Task<IEnumerable<BusinessProfileWithDiscountDto>> GetBusinessProfilesWithDiscountedProductsAsync()
        {
            // Step 1: Get max discount per business from SKU variants
            var businessDiscounts = await _context.Sku_ProductVariants
                .Where(v => v.Discount.HasValue && v.Discount > 0)
                .Include(v => v.Product) // Include product to access BusRegId
                .GroupBy(v => v.Product.BusRegId)
                .Select(g => new
                {
                    BusRegId = g.Key,
                    MaxDiscount = g.Max(v => v.Discount.Value)
                })
                .OrderByDescending(g => g.MaxDiscount) // highest discount first
                .ToListAsync();

            // Step 2: Fetch profiles
            var businessIds = businessDiscounts.Select(b => b.BusRegId).ToList();

            var businessProfiles = await _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId))
                .ToListAsync();

            // Step 3: Merge results (preserve discount order)
            var result = businessDiscounts
                .Join(businessProfiles,
                      bd => bd.BusRegId,
                      bp => bp.BusRegId,
                      (bd, bp) => new BusinessProfileWithDiscountDto
                      {
                          Profile = bp,
                          MaxDiscount = bd.MaxDiscount
                      })
                .ToList();

            return result;
        }

        //Get countries having profil on mytown
        public async Task<List<string>> GetUniqueCountriesAsync()
        {
            // Step 1: Get raw locations from DB
            var locations = await _context.BusinessProfiles
                .Where(b => b.BusinessLocation != null && b.BusinessLocation.Contains(","))
                .Select(b => b.BusinessLocation)
                .ToListAsync();

            // Step 2: Process in memory (C# side)
            var countries = locations
                .Select(loc => loc.Substring(loc.LastIndexOf(',') + 1).Trim()) // take last part
                .Where(c => !string.IsNullOrWhiteSpace(c) && c != "0000")
                .Select(c => c.ToLower()) // normalize case
                .Distinct()
                .Select(c => char.ToUpper(c[0]) + c.Substring(1)) // TitleCase
                .ToList();

            return countries;
        }

    }
}
