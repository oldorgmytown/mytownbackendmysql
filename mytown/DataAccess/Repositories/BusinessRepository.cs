using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using static mytown.Models.busprofilepreview;


namespace mytown.DataAccess.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly AppDbContext _context;

        public BusinessRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.BusinessRegisters.AnyAsync(b => b.BusEmail == email);
        }


        public async Task SavePendingVerification(PendingBusinessVerification pending)
        {
            _context.PendingBusinessVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingBusinessVerification> FindPendingVerificationByToken(string token)
        {
            return await _context.PendingBusinessVerifications.FirstOrDefaultAsync(p => p.Token == token);
        }

        public async Task DeletePendingVerification(string token)
        {
            var record = await _context.PendingBusinessVerifications.FirstOrDefaultAsync(p => p.Token == token);
            if (record != null)
            {
                _context.PendingBusinessVerifications.Remove(record);
                await _context.SaveChangesAsync();
            }
        }


        public async Task RegisterBusiness(BusinessRegister business)
        {
            _context.BusinessRegisters.Add(business);
            await _context.SaveChangesAsync();
        }

        // resend email verfication
        public async Task<BusinessVerification> FindPendingVerificationByEmail(string email)
        {
            return await _context.BusinessVerification
                .Include(bv => bv.Business)
                .Where(bv => bv.Business.BusEmail == email && !bv.IsUsed && bv.ExpiryDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }


        public async Task RemoveVerification(BusinessVerification verification)
        {
            _context.BusinessVerification.Remove(verification);
            await _context.SaveChangesAsync();
        }

      

        //public async Task<BusinessRegister> AddBusinessRegisterAsync(BusinessRegister newBusiness)
        //    {
        //        await _context.BusinessRegisters.AddAsync(newBusiness);
        //        await _context.SaveChangesAsync();
        //        return newBusiness;
        //    }
        //    public async Task<BusinessRegister> GetBusinessByEmailAsync(string email)
        //    {
        //        return await _context.BusinessRegisters
        //            .FirstOrDefaultAsync(b => b.BusEmail == email);
        //    }

        //get business owner home page with busregid
        public async Task<BusinessRegister> GetBusinessByIdAsync(int busRegId)
        {
            return await _context.BusinessRegisters
                                .FirstOrDefaultAsync(b => b.BusRegId == busRegId);
        }


        //get business store types
        public async Task<ActionResult<IEnumerable<businesscategoriescs>>> GetBusinessCategories()
        {
            return await _context.BusinessCategories.ToListAsync();
        }

        //get business services types
        public async Task<ActionResult<IEnumerable<businessservices>>> GetBusinessServices()
        {
            return await _context.BusinessServices.ToListAsync();
        }

        // Fetch subcategories by BuscatId
        public async Task<IEnumerable<product_sub_categories>> BusinessSubCategoriesforStores(int buscatId)
        {
            return await _context.product_sub_categories
                                 .Where(p => p.BuscatId == buscatId)
                                 .ToListAsync();
        }

        //add categoryimages for busregid
        // Add a new subcategory image
        public async Task AddOrUpdateSubcategoryImageAsync(subcategoryimages_busregid subcategoryImage)
        {
            // Check if a record with the same BusRegId and Prod_subcat_id exists
            var existingRecord = await _context.Subcategoryimages_Busregids
                                               .FirstOrDefaultAsync(x => x.BusRegId == subcategoryImage.BusRegId &&
                                                                         x.Prod_subcat_id == subcategoryImage.Prod_subcat_id);

            if (existingRecord != null)
            {
                // Update the existing record
                existingRecord.Prod_subcat_name = subcategoryImage.Prod_subcat_name;
                existingRecord.Prod_subcat_image = subcategoryImage.Prod_subcat_image;
            }
            else
            {
                // Add a new record
                _context.Subcategoryimages_Busregids.Add(subcategoryImage);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }


        // Get all subcategory images by BusRegId
        public async Task<List<subcategoryimages_busregid>> GetSubcategoryImagesByBusRegIdAsync(int busRegId)
        {
            return await _context.Subcategoryimages_Busregids
                                 .Where(image => image.BusRegId == busRegId)
                                 .ToListAsync();
        }
        // add products to db
        public async Task<products> CreateProductAsync(products product)
        {
            await _context.products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<products> GetProductById(int productId)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == productId);

            return product ?? new products(); // Return an empty product if not found
        }



        // Get all products from the database based on busregid
        public async Task<IEnumerable<products>> GetAllProductsAsync(int BusRegId)
        {
            return await _context.products
                                 .Where(p => p.BusRegId == BusRegId) // Filter by BusRegId
                                 .ToListAsync(); // Fetch matching products
        }


        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == productId);
            if (product != null)
            {
                _context.products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateProductAsync(products product)
        {
            _context.products.Update(product);
            await _context.SaveChangesAsync();


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

        public async Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath)
        {
            var business = await _context.BusinessProfiles
                                         .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            if (business == null)
                return false; // Business not found

            business.banner_path = bannerPath;
            await _context.SaveChangesAsync();
            return true;
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






        //edit or update product details 
        public bool UpdateProduct(products updatedProduct)
        {
            var existingProduct = _context.products.FirstOrDefault(p => p.product_id == updatedProduct.product_id);

            if (existingProduct == null)
                return false;

            // Update fields
            existingProduct.product_name = updatedProduct.product_name;
            existingProduct.product_subject = updatedProduct.product_subject;
            existingProduct.product_description = updatedProduct.product_description;
            existingProduct.product_image = updatedProduct.product_image;
            existingProduct.product_cost = updatedProduct.product_cost;
            existingProduct.product_length = updatedProduct.product_length;
            existingProduct.product_width = updatedProduct.product_width;
            existingProduct.product_weight = updatedProduct.product_weight;
            existingProduct.product_quantity = updatedProduct.product_quantity;

            _context.SaveChanges(); // Commit changes to the database
            return true;
        }


        //get profile detailsusing busregid
        //public async Task<List<businessprofile>> GetBusinessProfilesByBusRegIdAsync(int busRegId)
        //{
        //    return await _context.BusinessProfiles
        //                         .Where(bp => bp.BusRegId == busRegId) // Filter by BusRegId
        //                         .ToListAsync(); // Fetch all matching records
        //}

       



        //get products for selected category
        public IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
        {
            return _context.products
                           .Where(p => p.BusRegId == busRegId && p.prod_subcat_id == prodSubcatId)
                           .ToList();
        }
    }

}
