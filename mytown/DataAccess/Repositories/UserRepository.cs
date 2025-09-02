using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using Stripe;
using Stripe.Climate;
using System.Threading.Tasks;
using static mytown.Models.busprofilepreview;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<(int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync();
    }
    public class UserRepository
    {
        private readonly AppDbContext _context;
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }


        public UserRepository(AppDbContext context)
        {
            _context = context;
        }



        //public async Task<RegistrationWithBusinessRegId> LoginAsync(User loginUser)
        //{
        //    // Check in Users table for a matching email and password
        //    var user = await _context.Registrations
        //        .FirstOrDefaultAsync(u => u.Email == loginUser.Username && u.CnfPassword == loginUser.Password);

        //    if (user == null)
        //    {
        //        return null; // User not found
        //    }

        //    // Check if this user's regid exists in the BusinessRegister table
        //    var businessReg = await _context.BusinessRegisters
        //        .FirstOrDefaultAsync(br => br.RegId == user.RegId);

        //    // Create a response object including the business registration details (if any)
        //    return new RegistrationWithBusinessRegId
        //    {
        //        RegId = user.RegId,
        //        Username = user.Username,
        //        BusRegId = businessReg?.BusRegId // Return the busregid if it exists, otherwise null
        //    };
        //}

        //public async Task<object> LoginAsync(string email, string password)
        //{
        //    // 🔹 Business login
        //    var businessUser = await _context.BusinessRegisters.FirstOrDefaultAsync(r => r.BusEmail == email);
        //    if (businessUser != null)
        //    {
        //        if (BCrypt.Net.BCrypt.Verify(password, businessUser.Password))
        //        {
        //            var businessProfile = await _context.BusinessProfiles
        //    .FirstOrDefaultAsync(bp => bp.BusRegId == businessUser.BusRegId);

        //            return new
        //            {
        //                userType = "Business",
        //                user = businessUser,       // all fields from BusinessRegisters
        //                businessProfile = businessProfile, // null if not found
        //                shopper = (object)null,
        //                courier = (object)null
        //            };
        //        }

        //        return "WrongPassword"; // ⬅ indicate failure reason
        //    }

        //    // 🔹 Shopper login
        //    var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == email);
        //    if (shopper != null)
        //    {
        //        if (BCrypt.Net.BCrypt.Verify(password, shopper.Password))
        //        {
        //            return new
        //            {
        //                userType = "Shopper",
        //                user = (object)null,
        //                businessProfile = (object)null,
        //                shopper,
        //                courier = (object)null
        //            };
        //        }

        //        return "WrongPassword";
        //    }

        //    // 🔹 Courier login
        //    var courier = await _context.CourierService.FirstOrDefaultAsync(c => c.CourierEmail == email);
        //    if (courier != null)
        //    {
        //        if (BCrypt.Net.BCrypt.Verify(password, courier.Password))
        //        {
        //            return new
        //            {
        //                userType = "Courier",
        //                user = (object)null,
        //                businessProfile = (object)null,
        //                shopper = (object)null,
        //                courier
        //            };
        //        }

        //        return "WrongPassword";
        //    }

        //    return "EmailNotFound"; // ⬅ clearly mark email not found
        //}

        public async Task<object> LoginAsync(string email, string password)
        {
            // 🔹 Business login
            var businessUser = await _context.BusinessRegisters.FirstOrDefaultAsync(r => r.BusEmail == email);
            if (businessUser != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, businessUser.Password))
                {
                    var businessProfile = await _context.BusinessProfiles
                        .Where(bp => bp.BusRegId == businessUser.BusRegId)
                        .Select(bp => new
                        {
                            bp.businessprofile_id,
                            bp.BusinessUsername,
                            bp.business_location,
                            bp.business_about,
                            bp.banner_path,
                            bp.logo_path,
                            bp.profile_status,
                            bp.bus_time,
                            bp.BusCatId,
                            bp.BusServId,
                            bp.Businessservice_name,
                            bp.Businesscategory_name,
                            bp.approved_date
                           
                        })
                        .FirstOrDefaultAsync();

                    return new
                    {
                        userType = "Business",
                        user = new BusinessRegisterDto
                        {
                            BusRegId = businessUser.BusRegId,
                            BusinessUsername = businessUser.BusinessUsername,
                            Businessname = businessUser.Businessname,
                            LicenseType = businessUser.LicenseType,
                            Gstin = businessUser.Gstin,
                            BusservId = businessUser.BusservId,
                            BuscatId = businessUser.BuscatId,
                            Town = businessUser.Town,
                            BusMobileNo = businessUser.BusMobileNo,
                            BusEmail = businessUser.BusEmail,
                            Address1 = businessUser.Address1,
                            Address2 = businessUser.Address2,
                            businessCity = businessUser.businessCity,
                            businessState = businessUser.businessState,
                            businessCountry = businessUser.businessCountry,
                            postalCode = businessUser.postalCode,
                            isEmailVerified = businessUser.IsEmailVerified,
                            BusinessRegDate = businessUser.BusinessRegDate,
                            ProfileStatus = businessProfile?.profile_status ?? "Incomplete"
                        },
                        businessProfile = businessProfile // will be null if no profile exists
                    };
                }

                return null; // invalid password
            }


            // 🔹 Shopper login
            var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == email);
            if (shopper != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, shopper.Password))
                {
                    return new
                    {
                        userType = "Shopper",
                        shopper = new ShopperRegisterDto
                        {
                            ShopperRegId = shopper.ShopperRegId,
                            Username = shopper.Username,
                            Email = shopper.Email,
                            IsEmailVerified = shopper.IsEmailVerified,
                            Address = shopper.Address,
                            Town = shopper.Town,
                            City = shopper.City,
                            State = shopper.State,
                            Country = shopper.Country,
                            PostalCode = shopper.PostalCode,
                            PhoneNumber = shopper.PhoneNumber,
                            PhotoName = shopper.PhotoName,
                            Status = shopper.status,
                            ShopperRegDate = shopper.ShopperRegDate
                        }
                    };
                }
                return null;
            }

            // 🔹 Courier login
            var courier = await _context.CourierService.FirstOrDefaultAsync(c => c.CourierEmail == email);
            if (courier != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, courier.Password))
                {
                    return new
                    {
                        userType = "Courier",
                        courier = new CourierServiceDto
                        {
                            CourierServiceName = courier.CourierServiceName,
                            CourierContactName = courier.CourierContactName,
                            CourierPhone = courier.CourierPhone,
                            CourierEmail = courier.CourierEmail,
                            IsLocal = courier.IsLocal,
                            IsState = courier.IsState,
                            IsNational = courier.IsNational,
                            IsInternational = courier.IsInternational
                        }
                    };
                }
                return null;
            }

            return null; // email not found
        }



        public async Task<bool> UserExists(Registration regDetails)
        {
            // Check for existing username or email in the Registrations table
            return await _context.Registrations.AnyAsync(r => r.Username == regDetails.Username || r.Email == regDetails.Email);
        }


        public async Task<Registration> AddUserAsync(Registration newUser)
        {
            await _context.Registrations.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        #region Forgotpassword
        public bool EmailExists(string email)
        {
            return _context.ShopperRegisters.Any(u => u.Email == email) ||
                   _context.BusinessRegisters.Any(u => u.BusEmail == email);
        }

        public bool ResetPassword(string email, string newPassword)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                var shopper = _context.ShopperRegisters.FirstOrDefault(u => u.Email == email);
                var business = _context.BusinessRegisters.FirstOrDefault(u => u.BusEmail == email);

                if (shopper == null && business == null)
                    return false;

                if (shopper != null) shopper.Password = hashedPassword;
                if (business != null) business.Password = hashedPassword;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }



        #endregion

        public async Task<BusinessRegister> AddBusinessRegisterAsync(BusinessRegister newBusiness)
        {
            await _context.BusinessRegisters.AddAsync(newBusiness);
            await _context.SaveChangesAsync();
            return newBusiness;
        }
        public async Task<BusinessRegister> GetBusinessByEmailAsync(string email)
        {
            return await _context.BusinessRegisters
                .FirstOrDefaultAsync(b => b.BusEmail == email);
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
        public async Task<IEnumerable<product_sub_categories>> GetSubCategoriesByBuscatIdAsync(int buscatId)
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
        public async Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile)
        {
            // Check if the business profile with the given BusRegId already exists
            var existingProfile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(bp => bp.BusRegId == businessProfile.BusRegId);

            if (existingProfile != null)
            {
                // Updating an existing profile
                existingProfile.BusinessUsername = businessProfile.BusinessUsername;
                existingProfile.business_location = businessProfile.business_location;
                existingProfile.business_about = businessProfile.business_about;
                existingProfile.banner_path = businessProfile.banner_path;
                existingProfile.profile_status = businessProfile.profile_status;
                existingProfile.bus_time = businessProfile.bus_time;
                existingProfile.BusCatId = businessProfile.BusCatId;
                existingProfile.BusServId = businessProfile.BusServId;

                //// Update image position & zoom
                //existingProfile.image_positionx = businessProfile.image_positionx;
                //existingProfile.image_positiony = businessProfile.image_positiony;
                //existingProfile.zoom = businessProfile.zoom;

                // Mark entity as modified
                _context.BusinessProfiles.Update(existingProfile);
            }
            else
            {
                // Set default values if they are not provided
                //if (businessProfile.image_positionx == 0 && businessProfile.image_positiony == 0 && businessProfile.zoom == 0)
                //{
                //    businessProfile.image_positionx = 0;
                //    businessProfile.image_positiony = 0;
                //    businessProfile.zoom = 1; // Default zoom value
                //}

                // Add a new profile
                await _context.BusinessProfiles.AddAsync(businessProfile);
            }

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            // Return the updated or newly added profile
            return existingProfile ?? businessProfile;
        }

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

        //ADMIN PANEL

        //to get all business profiles with status
        public async Task<(IEnumerable<BusinessRegister> records, int totalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.BusinessRegisters.CountAsync();
            var records = await _context.BusinessRegisters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }




        public async Task<(int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync()
        {
            // Fetch unique cities from both tables
            var uniqueCities = await _context.BusinessRegisters
                .Select(b => b.businessCity)
                .Where(city => !string.IsNullOrEmpty(city))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.City)
                        .Where(city => !string.IsNullOrEmpty(city))
                )
                .Distinct()
                .CountAsync();

            // Fetch unique states from both tables
            var uniqueStates = await _context.BusinessRegisters
                .Select(b => b.businessState)
                .Where(state => !string.IsNullOrEmpty(state))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.State)
                        .Where(state => !string.IsNullOrEmpty(state))
                )
                .Distinct()
                .CountAsync();

            // Fetch unique countries from both tables
            var uniqueCountries = await _context.BusinessRegisters
                .Select(b => b.businessCountry)
                .Where(country => !string.IsNullOrEmpty(country))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.Country)
                        .Where(country => !string.IsNullOrEmpty(country))
                )
                .Distinct()
                .CountAsync();

            return (uniqueCities, uniqueStates, uniqueCountries);
        }

        public async Task<int> GetBusinessRegisterCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.BusinessRegisters.CountAsync();
            return count;
        }

        #region Shopper Registration

        public async Task<ShopperRegister> AddShopperRegisterAsync(ShopperRegister shopperDetails)
        {
            _context.Set<ShopperRegister>().Add(shopperDetails);
            await _context.SaveChangesAsync();
            return shopperDetails;
        }
        public async Task<ShopperRegister> GetShopperByEmailAsync(string email)
        {
            return await _context.Set<ShopperRegister>().FirstOrDefaultAsync(s => s.Email == email);
        }
        public async Task<int> GetShoppersRegisterCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.ShopperRegisters.CountAsync();
            return count;
        }

        //public async Task<IEnumerable<shopperregister>> GetAllShopperRegistersAsync()
        //{
        //    // Retrieve all rows from the ShopperRegisters table asynchronously
        //    return await _context.ShopperRegisters.ToListAsync();
        //}
        public async Task<(IEnumerable<ShopperRegister> records, int totalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.ShopperRegisters.CountAsync();
            var records = await _context.ShopperRegisters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }
        public async Task<ShopperRegister> GetShopperDetails(int shopperRegId)
        {
            return await _context.ShopperRegisters.FindAsync(shopperRegId);
        }
        #endregion

        #region shopper emailverificatiom
        public async Task<ShopperRegister> RegisterShopper(ShopperRegister shopper)
        {
            //if (await IsEmailTaken(shopper.Email))
            //    throw new Exception("Email is already in use.");

            shopper.Password = BCrypt.Net.BCrypt.HashPassword(shopper.Password);
            shopper.IsEmailVerified = false;

            _context.ShopperRegisters.Add(shopper);
            await _context.SaveChangesAsync();

            //   await GenerateEmailVerification(shopper.Email);

            return shopper;
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.ShopperRegisters.AnyAsync(s => s.Email == email);
        }

        //public async Task<ShopperVerification> GenerateEmailVerification(string email)
        //{
        //    var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == email);
        //    if (shopper == null) throw new Exception("User not found.");

        //    var token = Guid.NewGuid().ToString();
        //    var expiryDate = DateTime.UtcNow.AddHours(24);

        //    var verification = new ShopperVerification
        //    {
        //        Email = email,
        //        VerificationToken = token,
        //        ExpiryDate = expiryDate,
        //        IsVerified = false
        //    };

        //    _context.ShopperVerification.Add(verification);
        //    await _context.SaveChangesAsync();

        //    return verification;
        //}


        #endregion

        #region service

        public async Task AddServiceAsync(services service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Service cannot be null");
            }

            try
            {
                _context.services.Add(service);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions
                throw new Exception("An error occurred while adding the service to the database", ex);
            }
        }
        #endregion

        #region landingpage search


        //api to get list of products based on location and category, subcategory and prodcut search
        public List<products> SearchBusinessesWithProducts(string locationQuery, string productQuery)
        {
            if (string.IsNullOrEmpty(productQuery))
                return new List<products>(); // If no productQuery, return empty list

            List<int> filteredBusinesses = new List<int>();

            // **Step 1: Filter Businesses by Location if locationQuery is provided**
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
                    return new List<products>(); // No matching businesses found for the location
            }

            // **Step 2: Check if productQuery matches Business Categories**
            var matchingCategories = _context.BusinessCategories
                .Where(c => c.Businesscategory_name.Contains(productQuery))
                .Select(c => c.BuscatId)
                .ToList();

            // **Step 3: Check if productQuery matches Product Subcategories**
            var matchingSubCategories = _context.product_sub_categories
                .Where(sc => sc.prod_subcat_name.Contains(productQuery))
                .Select(sc => sc.prod_subcat_id)
                .ToList();

            // **Step 4: Get Products Based on Location and Product Matches**
            var productsQuery = _context.products.AsQueryable();

            if (!string.IsNullOrEmpty(locationQuery))
            {
                // **Filter products that belong to businesses in the selected location**
                productsQuery = productsQuery.Where(p => filteredBusinesses.Contains(p.BusRegId));
            }

            // **Apply Product Search Filters**
            productsQuery = productsQuery.Where(p =>
                matchingCategories.Contains(p.BuscatId) ||
                matchingSubCategories.Contains(p.prod_subcat_id) ||
                p.product_name.Contains(productQuery) ||
                p.product_subject.Contains(productQuery) ||
                p.product_description.Contains(productQuery)
            );

            return productsQuery.ToList();
        }


        // search from location and prodcut/category and get the matching store details
        public async Task<List<businessprofile>> SearchBusinessesAsync(string location, string categoryProduct)
        {
            var productResults = new List<products>();
            var busRegIds = new List<int>();
            var businessesByLocation = new List<BusinessRegister>();
            var businessesByCategoryProduct = new List<BusinessRegister>();

            // Step 1: If categoryProduct is provided, search through ProductSubCategories, BusinessCategories, and Products
            if (!string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
            {
                // Search ProductSubCategories
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
                    // Search BusinessCategories
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
                        // Search directly in Products table if no category or subcategory found
                        productResults = await _context.products
                            .Where(p => p.product_name.Contains(categoryProduct) || p.product_subject.Contains(categoryProduct) || p.product_description.Contains(categoryProduct))
                            .ToListAsync();

                        busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                    }
                }
            }

            // Step 2: If location is provided, filter by location in BusinessRegister
            if (!string.IsNullOrEmpty(location) && location != "null")
            {
                businessesByLocation = await _context.BusinessRegisters
                    .Where(br => br.Town.Contains(location) || br.businessCity.Contains(location) || br.businessState.Contains(location) || br.businessCountry.Contains(location))
                    .ToListAsync();
            }

            // Step 3: Filter BusinessRegister by matching BusRegIds for product search
            if (busRegIds.Any())
            {
                businessesByCategoryProduct = await _context.BusinessRegisters
                    .Where(br => busRegIds.Contains(br.BusRegId))
                    .ToListAsync();
            }

            // Step 4: Handle search results based on available criteria

            List<BusinessRegister> combinedResults;

            if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
            {
                // If both location and categoryProduct are provided, find intersection
                combinedResults = businessesByLocation
                    .Where(loc => businessesByCategoryProduct
                        .Any(cat => cat.BusRegId == loc.BusRegId))
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(location) && (string.IsNullOrEmpty(categoryProduct) || categoryProduct == "null"))
            {
                // If only location is provided, return results matching location
                combinedResults = businessesByLocation;
            }
            else if (string.IsNullOrEmpty(location) || location == "null")
            {
                // If location is not provided or "null", use categoryProduct to filter
                combinedResults = businessesByCategoryProduct;
            }
            else
            {
                // Default case if neither location nor categoryProduct is provided
                combinedResults = new List<BusinessRegister>();
            }

            // Step 5: Retrieve BusinessProfile details for each matching BusinessRegister
            var result = new List<businessprofile>();

            foreach (var business in combinedResults)
            {
                var profile = await _context.BusinessProfiles
                    .FirstOrDefaultAsync(bp => bp.BusRegId == business.BusRegId);

                if (profile != null)
                {
                    result.Add(profile); // Only add BusinessProfile to result
                }
            }

            return result;
        }





        //** get location search mathcing on business profiles 

        //get profiles based on location search 
        public List<businessprofile> GetBusinessProfilesByLocation(string location)
        {
            return _context.BusinessProfiles
        .Where(bp => bp.business_location.Contains(location))
        .ToList();
        }


        //search for prodcut name in category, subcategory and prodcuts tables
        public List<businessprofile> GetBusinessProfilesBySearchTerm(string searchTerm)
        {
            // Step 1: Search in businesscategoriescs table
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.Businesscategory_name.Contains(searchTerm))
                .Select(bc => bc.BuscatId)
                .ToList();

            // Step 2: Use found BuscatIds to search in products table
            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct()
                .ToList();

            // Step 3: If no match in categories, search in product_sub_categories table
            if (!businessIds.Any())
            {
                var matchingSubcatIds = _context.product_sub_categories
                    .Where(sc => sc.prod_subcat_name.Contains(searchTerm))
                    .Select(sc => sc.prod_subcat_id)
                    .ToList();

                businessIds = _context.products
                    .Where(p => matchingSubcatIds.Contains(p.prod_subcat_id))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            // Step 4: If still no match, search directly in products table
            if (!businessIds.Any())
            {
                businessIds = _context.products
                    .Where(p =>
                        p.product_name.Contains(searchTerm) ||
                        p.product_subject.Contains(searchTerm) ||
                        p.product_description.Contains(searchTerm))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            // Step 5: Fetch Full Business Profiles
            return _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId))
                .ToList();
        }

        // Fetch Business Profiles based on Product & Location Search Terms
        public List<businessprofile> GetBusinessProfilesByProductAndLocation(string productSearchTerm, string locationSearchTerm)
        {
            // Step 1: Get BusRegIds matching the product search term
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.Businesscategory_name.Contains(productSearchTerm))
                .Select(bc => bc.BuscatId)
                .ToList();

            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct()
                .ToList();

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

            // Step 2: Get full business profiles
            var businessProfiles = _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId))
                .ToList();

            // Step 3: Filter profiles by location search term
            var filteredProfiles = businessProfiles
                .Where(bp => bp.business_location.Contains(locationSearchTerm))
                .ToList();

            return filteredProfiles;
        }
        #endregion

        #region Shopping Cart
        public async Task<addtocart> AddToCart(addtocart cartItem)
        {
            var existingCartItem = await _context.addtocart
         .FirstOrDefaultAsync(c =>
             c.product_id == cartItem.product_id &&
             c.BusRegId == cartItem.BusRegId && // Check if the product is from the same store
             c.ShopperRegId == cartItem.ShopperRegId &&
             c.orderstatus == "cart"); // Only check active cart items


            if (existingCartItem != null)
            {
                //Product exists, so increase quantity by 1
                existingCartItem.prod_qty += 1;
                await _context.SaveChangesAsync();
                return existingCartItem;
            }
            else
            {
                //New product, insert into cart
                cartItem.prod_qty = 1; // Ensure quantity starts at 1
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return cartItem;
                //}
            }
        }




        // Get all cart items for a specific user
        public async Task<IEnumerable<CartItemDto>> GetCartItems(int shopperRegId)
        {
            var cartItems = await (from cart in _context.addtocart
                                   join product in _context.products
                                   on cart.product_id equals product.product_id
                                   join business in _context.BusinessProfiles
                                   on product.BusRegId equals business.BusRegId
                                   where cart.ShopperRegId == shopperRegId && cart.orderstatus == "cart"
                                   select new CartItemDto
                                   {
                                       CartId = cart.CartId,
                                       ShopperRegId = cart.ShopperRegId,
                                       prod_qty = cart.prod_qty,
                                       orderstatus = cart.orderstatus,
                                       product_id = product.product_id,
                                       product_name = product.product_name,
                                       product_subject = product.product_subject,
                                       product_description = product.product_description,
                                       product_image = product.product_image,
                                       product_cost = product.product_cost,
                                       StoreName = business.BusinessUsername, // Store name
                                       StoreLocation = business.business_location // Store location
                                   }).ToListAsync();

            return cartItems;
        }



        // Remove an item from cart
        public async Task<bool> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.addtocart.FindAsync(cartId);
            if (cartItem == null) return false;

            _context.addtocart.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DecreaseCartItemQty(int cartId)
        {
            var cartItem = await _context.addtocart.FindAsync(cartId);

            if (cartItem == null)
            {
                return false; // Item not found
            }

            if (cartItem.prod_qty > 1)
            {
                cartItem.prod_qty -= 1; // Decrease quantity by 1
            }
            else
            {
                _context.addtocart.Remove(cartItem); // Remove item if quantity reaches 0
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncreaseCartItemQty(int cartId)
        {
            var cartItem = await _context.addtocart.FindAsync(cartId);

            if (cartItem == null)
            {
                return false; // Item not found
            }

            cartItem.prod_qty += 1; // Increase quantity by 1

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveToWishlist(int cartId)
        {
            var item = await _context.addtocart.FindAsync(cartId);
            if (item != null)
            {
                item.orderstatus = "wishlist";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> MoveBackToCart(int cartId)
        {
            var item = await _context.addtocart.FindAsync(cartId);
            if (item != null)
            {
                item.orderstatus = "cart";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<int> CreateOrderAsync(int shopperRegId, string shippingType)
        {
            // Calculate total amount from cart
            var totalAmount = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .SumAsync(c => c.product_price * c.prod_qty);

            if (totalAmount == 0)
            {
                return 0; // No items in cart
            }

            // Create new order
            var newOrder = new mytown.Models.Order
            {
                ShopperRegId = shopperRegId,
                TotalAmount = totalAmount,
                ShippingType = shippingType,
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Move cart items to OrderDetails table
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .ToListAsync();

            foreach (var item in cartItems)
            {
                var orderDetail = new orderdetails
                {
                    OrderId = newOrder.OrderId,
                    ProductId = item.product_id,
                    StoreId = item.BusRegId,
                    Quantity = item.prod_qty,
                    Price = item.product_price
                };
                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();

            //// Remove items from cart after placing order
            //_context.addtocart.RemoveRange(cartItems);
            //await _context.SaveChangesAsync();

            return newOrder.OrderId; // Return the created OrderId
        }
        public Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod)
        {
            // Check if the order exists
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            // Create new payment entry
            var payment = new Payments
            {
                OrderId = orderId,
                AmountPaid = amountPaid,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Completed", // Assuming successful payment
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Update order status to "Paid"
            order.OrderStatus = "Paid";
            _context.Orders.Update(order);
            _context.SaveChanges();



            return payment;
        }

        public async Task<bool> UpdateCartStatusAsync(int orderId)
        {
            // Find the order using OrderId
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false; // Order not found

            // Get cart items for the shopper related to the order
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == order.ShopperRegId && c.orderstatus == "Cart")
                .ToListAsync();

            if (!cartItems.Any()) return false; // No cart items to update

            // Update cart status
            foreach (var item in cartItems)
            {
                item.orderstatus = "Ordered";
            }

            // Save changes
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}

//result.Add(new BusinessSearchResult
//{
//    //BusinessRegister = business,
//    BusinessProfile = profile
//});


