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

        public async Task<IEnumerable<BusinessProfile>> GetAllBusinessProfilesAsync()
        {
            return await _context.BusinessProfiles
                .Where(bp => bp.ProfileStatus == "approved")
                .Include(bp => bp.BusinessRegister)
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
                    Businessname = br.BusinessName,
                    Businessusername = br.BusinessUsername,
                    business_location = bp.BusinessLocation,
                   // business_tagline = bp.BusinessTagline,
                    business_about = bp.BusinessAbout,
                    banner_path = bp.BannerPath,
                    logo_path = bp.LogoPath,
                    profile_status = bp.ProfileStatus,
                    BusCatId = bp.BusCatId,
                    BusServId = bp.BusServId,
                    // Left-joined rows may be null — fall back to empty string
                    Businessservice_name = bs != null ? bs.BusinessServiceName : "",
                    Businesscategory_name = bc != null ? bc.BusinessCategoryName : "",
                    Currency = br.Currency,
                    BusEmail = br.BusEmail,
                    BusPhone = br.BusMobileNo
                }
            ).ToListAsync();

            return result;
        }

        public IEnumerable<Products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
        {
            return _context.products
                .Where(p => p.BusRegId == busRegId && p.ProdSubcatId == prodSubcatId)
                .ToList();
        }

        public async Task<BusinessProfile> AddBusinessProfileAsync(BusinessProfile businessProfile)
        {
            var existingProfile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(bp => bp.BusRegId == businessProfile.BusRegId);

            if (existingProfile != null)
            {
                if (!string.IsNullOrEmpty(businessProfile.BusinessName))
                    existingProfile.BusinessName = businessProfile.BusinessName;

                if (!string.IsNullOrEmpty(businessProfile.BusinessLocation))
                    existingProfile.BusinessLocation = businessProfile.BusinessLocation;

                //if (!string.IsNullOrEmpty(businessProfile.BusinessTagline))
                //    existingProfile.BusinessTagline = businessProfile.BusinessTagline;

                if (!string.IsNullOrEmpty(businessProfile.BusinessAbout))
                    existingProfile.BusinessAbout = businessProfile.BusinessAbout;

                if (!string.IsNullOrEmpty(businessProfile.BannerPath))
                {
                    if (!string.IsNullOrEmpty(existingProfile.BannerPath))
                        await DeleteFromBlobAsync(existingProfile.BannerPath);

                    existingProfile.BannerPath = businessProfile.BannerPath;
                }

                if (!string.IsNullOrEmpty(businessProfile.LogoPath))
                {
                    if (!string.IsNullOrEmpty(existingProfile.LogoPath))
                        await DeleteFromBlobAsync(existingProfile.LogoPath);

                    existingProfile.LogoPath = businessProfile.LogoPath;
                }

                if (!string.IsNullOrEmpty(businessProfile.ProfileStatus))
                    existingProfile.ProfileStatus = businessProfile.ProfileStatus;

                if (businessProfile.BusCatId != 0)
                    existingProfile.BusCatId = businessProfile.BusCatId;

                if (businessProfile.BusServId != 0)
                    existingProfile.BusServId = businessProfile.BusServId;

                _context.BusinessProfiles.Update(existingProfile);
            }
            else
            {
                await _context.BusinessProfiles.AddAsync(businessProfile);
            }

            await _context.SaveChangesAsync();
            return existingProfile ?? businessProfile;
        }

        public async Task DeleteFromBlobAsync(string fileName)
        {
            // Fall back to empty string if config is missing — BlobServiceClient
            // will throw a meaningful exception rather than a NullReferenceException
            var containerName = _configuration["AzureBlobStorage:ContainerName"] ?? "";
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"] ?? "";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> UploadToBlobAsync(IFormFile file, string imageType)
        {
            var containerName = _configuration["AzureBlobStorage:ContainerName"] ?? "";
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"] ?? "";

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

            return newFileName;
        }

        public async Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath)
        {
            var business = await _context.BusinessProfiles
                .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            if (business == null)
                return false;

            business.BannerPath = bannerPath;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath)
        {
            var business = await _context.BusinessProfiles
                .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            if (business == null)
                return false;

            business.LogoPath = logoPath;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductSubCategory>> GetAllSubCategoriesAsync()
        {
            return await _context.product_sub_categories.ToListAsync();
        }

        public List<ProductSubCategory> GetProductSubCategoriesByBusRegId(int busRegId)
        {
            var result = (from product in _context.products
                          join subCategory in _context.product_sub_categories
                              on product.ProdSubcatId equals subCategory.ProdSubcatId
                          join subCatImage in _context.Subcategoryimages_Busregids
                              on new { product.BusRegId, ProdSubCatId = subCategory.ProdSubcatId }
                              equals new { subCatImage.BusRegId, ProdSubCatId = subCatImage.Prod_subcat_id }
                              into subCatImageGroup
                          from subCatImage in subCatImageGroup.DefaultIfEmpty()
                          where product.BusRegId == busRegId
                          select new ProductSubCategory
                          {
                              ProdSubcatId = subCategory.ProdSubcatId,
                              ProdSubcatName = subCategory.ProdSubcatName,
                              ProdSubcatImage = subCatImage != null ? subCatImage.Prod_subcat_image : null
                          })
                          .Distinct()
                          .ToList();

            return result;
        }

        public List<ProductSubCategory> GetProductSubCategoriesByBusCatId(int busCatId)
        {
            return _context.product_sub_categories
                .Where(x => x.BuscatId == busCatId)
                .ToList();
        }

        public async Task<ProductDetailsDto> GetDetailsBySubCategoryAsync(int prodSubcatId)
        {
            var types = await _context.Product_Types
                .Where(pt => pt.ProdSubcatId == prodSubcatId)
                .OrderBy(pt => pt.ProdTypeName)
                .ToListAsync();

            var fabrics = await _context.Fabrics
                .Where(f => f.ProdSubcatId == prodSubcatId)
                .OrderBy(f => f.FabricName)
                .ToListAsync();

            var designs = await _context.Designs
                .Where(d => d.ProdSubcatId == prodSubcatId)
                .OrderBy(d => d.DesignName)
                .ToListAsync();

            var sizes = await _context.ProductSizes
                .Where(s => s.ProdSubcatId == prodSubcatId)
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

        public async Task<IEnumerable<BusinessProfileWithDiscountDto>> GetBusinessProfilesWithDiscountedProductsAsync()
        {
            // Step 1: Get max discount per business from SKU variants
            var businessDiscounts = await _context.Sku_ProductVariants
                .Where(v => v.Discount.HasValue && v.Discount > 0)
                .Include(v => v.Product)
                .GroupBy(v => v.Product.BusRegId)
                .Select(g => new
                {
                    BusRegId = g.Key,
                    // .Value is safe here — the Where above guarantees Discount.HasValue
                    MaxDiscount = g.Max(v => v.Discount!.Value)
                })
                .OrderByDescending(g => g.MaxDiscount)
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

        public async Task<List<string>> GetUniqueCountriesAsync()
        {
            // Step 1: Pull raw locations from DB — filter nulls and those with commas server-side
            var locations = await _context.BusinessProfiles
                .Where(b => b.BusinessLocation != null && b.BusinessLocation.Contains(","))
                .Select(b => b.BusinessLocation)
                .ToListAsync();

            // Step 2: Process in memory
            // loc is string? from EF; the Where above guarantees non-null but NRT
            // analysis doesn't track that, so use the null-forgiving operator (loc!)
            var countries = locations
                .Select(loc => loc!.Substring(loc.LastIndexOf(',') + 1).Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c) && c != "0000")
                .Select(c => c.ToLower())
                .Distinct()
                .Select(c => char.ToUpper(c[0]) + c.Substring(1))
                .ToList();

            return countries;
        }
    }
}