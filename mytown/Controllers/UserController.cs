//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using mytown.Models;
//using static mytown.DataAccess.Repositories.UserRepository;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.EntityFrameworkCore;
//using System.Text.RegularExpressions;
//using Microsoft.AspNetCore.Identity.Data;
//using Newtonsoft.Json.Linq;
//using System.Buffers.Text;
//using Stripe;
//using mytown.Services;
//using mytown.DataAccess.Repositories;

//namespace mytown.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [EnableCors("AllowFrontend")]
//    public class UserController : ControllerBase
//    {
//        private readonly UserRepository _userRepository;
        
//        private readonly IWebHostEnvironment _env;
//        private readonly string stripeSecretKey = "sk_test_51QtS7OFMWqb9scCuoOdpdCcEb7WultTBEDZMEF7MsjyvgbbdHsQalQyKXsDQaYKBFg4DAAQkL1VeGp6DfO6FZ0CW00hbxqjakt";
//        private readonly IEmailService _emailService;

//        public UserController(UserRepository userRepository, IWebHostEnvironment env, IEmailService emailService)
//        {
//            _userRepository = userRepository; // Inject UserRepository
//            _env = env;  // Inject IWebHostEnvironment to get access to WebRootPath
//            StripeConfiguration.ApiKey = stripeSecretKey;
//            _emailService = emailService;
//        }




//        //[HttpPost("login")]
//        //public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
//        //{
//        //    if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
//        //    {
//        //        return BadRequest(new { code = 400, message = "Invalid login request" });
//        //    }

//        //    var result = await _userRepository.LoginAsync(loginRequest.Email, loginRequest.Password);

//        //    //if (result is string message)
//        //    //{
//        //    //    return message switch
//        //    //    {
//        //    //        "EmailNotFound" => NotFound(new { code = 404, message = "Email not registered" }),
//        //    //        "WrongPassword" => Unauthorized(new { code = 401, message = "Incorrect password" }),
//        //    //        "EmailNotVerified" => StatusCode(403, new { code = 403, message = "Please verify your email before login" }),
//        //    //        _ => StatusCode(500, new { code = 500, message = "Unexpected login error" })
//        //    //    };
//        //    //}

//        //    return Ok(result); // success
//        //}

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
//        {
//            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
//            {
//                return BadRequest(new { code = 400, message = "Invalid login request" });
//            }

//            try
//            {
//                var result = await _userRepository.LoginAsync(loginRequest.Email, loginRequest.Password);

//                if (result == null)
//                    return Unauthorized(new { code = 401, message = "Invalid credentials" });

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                // Log full error for debugging
//                Console.WriteLine($"Login error: {ex}");

//                return StatusCode(500, new { code = 500, message = "An unexpected error occurred" });
//            }
//        }


//        #region Forgotpassword

//        [HttpPost("CheckEmail")]
//        public IActionResult CheckEmail([FromBody] string email)
//        {
//            if (string.IsNullOrWhiteSpace(email))
//                return BadRequest("Email is required.");

//            if (_userRepository.EmailExists(email))
//                return Ok(new { success = true });

//            return NotFound("Email not registered.");
//        }

//        [HttpPost("ResetPassword")]
//        public IActionResult ResetPassword([FromForm] string email, [FromForm] string newPassword, [FromForm] string confirmPassword)
//        {
//            if (newPassword != confirmPassword)
//                return BadRequest("Passwords do not match.");

//            bool result = _userRepository.ResetPassword(email, newPassword);
//            if (result)
//                return Ok("Password has been updated successfully.");

//            return StatusCode(500, "Failed to reset password.");
//        }

//        #endregion



//        //[HttpPost("register")]
//        //public async Task<IActionResult> Register([FromBody] Registration regDetails)
//        //{
//        //    if (regDetails == null)
//        //    {
//        //        return BadRequest("Registration details cannot be null.");
//        //    }

//        //    // Check if the username or email already exists in the Registrations table
//        //    if (await _userRepository.UserExists(regDetails))
//        //    {
//        //        return Conflict("Username or email already exists.");
//        //    }

//        //    // Add the new user to the database
//        //    var addedUser = await _userRepository.AddUserAsync(regDetails);

//        //    return CreatedAtAction(nameof(Register), new { id = addedUser.RegId }, addedUser);
//        //}

//        //[HttpPost("businessregister")]
//        //public async Task<IActionResult> RegisterBusiness([FromBody] BusinessRegister businessDetails)
//        //{
//        //    if (businessDetails == null)
//        //    {
//        //        return BadRequest("Business registration details cannot be null.");
//        //    }


//        //    // Check if the email already exists
//        //    var existingBusiness = await _userRepository.GetBusinessByEmailAsync(businessDetails.BusEmail);
//        //    if (existingBusiness != null)
//        //    {
//        //        return Conflict("A business with this email already exists.");
//        //    }
//        //    // Here, you might want to validate the businessDetails object further.

//        //    // Add the new business registration with the associated RegId
//        //    var addedBusiness = await _userRepository.AddBusinessRegisterAsync(businessDetails);

//        //    return CreatedAtAction(nameof(RegisterBusiness), new { id = addedBusiness.BusRegId }, addedBusiness);
//        //}


//        #region business Profile


//        [HttpPost("upload_profile_image")]
//        public async Task<IActionResult> upload_profile_image(IFormFile file)
//        {
             

//        //string _targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
//        string _targetFilePath = Path.Combine(_env.WebRootPath, "UploadedFiles");


//            // Ensure that the folder exists
//            if (!Directory.Exists(_targetFilePath))
//            {
//                Directory.CreateDirectory(_targetFilePath);
//            }

//            if (file == null || file.Length == 0)
//            {
//                return BadRequest("No file uploaded.");
//            }

//            // Generate a unique file name using timestamp
//            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
//            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
//            var fileExtension = Path.GetExtension(file.FileName);

//            // Create a new file name with timestamp
//            var newFileName = $"{fileNameWithoutExtension}_{timestamp}{fileExtension}";

//            var filePath = Path.Combine(_targetFilePath, newFileName);

//            // Save the file to the server
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }
//            var publicUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{newFileName}";
//            return Ok(new { FileName = newFileName, Url = publicUrl });
//            // Return the file path or any other necessary data
//           // return Ok(new { FileName = newFileName, FilePath = filePath });

//        }

//        //get buisness store types

//        //[HttpGet("BusinessCategories")]
//        //public async Task<ActionResult<IEnumerable<businesscategoriescs>>> GetBusinessCategories()
//        //{
//        //    // Call the GetBusinessCategories method in UserRepository to fetch data
//        //    var businessCategories = await _userRepository.GetBusinessCategories();

//        //    //// Check if no data is found
//        //    //if (businessCategories == null || businessCategories.Count == 0)
//        //    //{
//        //    //    return NotFound("No business categories found.");


//        //    return Ok(businessCategories);  // Return the business categories data
//        //}


//        //[HttpGet("BusinessServices")]
//        //public async Task<ActionResult<IEnumerable<businessservices>>> GetBusinessServices()
//        //{
//        //    // Call the GetBusinessCategories method in UserRepository to fetch data
//        //    var businessServices = await _userRepository.GetBusinessServices();


//        //    return Ok(businessServices);  // Return the business categories data
//        //}

//        //get categories on business owner home page based on type of store selection
//        [HttpGet("Business_Categories/{BuscatId}")]
//        public async Task<ActionResult<IEnumerable<product_sub_categories>>> GetSubCategoriesByBuscatIdAsync(int BuscatId)
//        {
//            // Pass the BuscatId to the repository method
//            var subCategories = await _userRepository.GetSubCategoriesByBuscatIdAsync(BuscatId);

//            // Check if there are no subcategories found
//            if (subCategories == null || !subCategories.Any())
//            {
//                return NotFound("No subcategories found for the provided category ID.");
//            }

//            // Return the list of subcategories
//            return Ok(subCategories);
//        }


//        // POST: api/SubcategoryImages
//        [HttpPost("SubcategoryImages")]
//        public async Task<IActionResult> AddSubcategoryImage([FromBody] subcategoryimages_busregid subcategoryImage)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            await _userRepository.AddOrUpdateSubcategoryImageAsync(subcategoryImage);
//            return Ok(new { message = "Subcategory image added successfully!" });
//        }

//        // GET: api/SubcategoryImages/BusRegId/5
//        [HttpGet("BusRegId/{busRegId}")]
//        public async Task<ActionResult<List<subcategoryimages_busregid>>> GetSubcategoryImagesByBusRegId(int busRegId)
//        {
//            var images = await _userRepository.GetSubcategoryImagesByBusRegIdAsync(busRegId);
//            if (images == null || images.Count == 0)
//            {
//                return NotFound(new { message = "No images found for the given BusRegId." });
//            }

//            return Ok(images);
//        }

//        //add products based on category, store id and businessregid
//        // [HttpPost("Add_Products")]
//        [HttpPost("Add_Products")]
//        public async Task<IActionResult> CreateProduct([FromBody] products product)
//        {
//            if (product == null)
//            {
//                return BadRequest("Product data is required.");
//            }

//            try
//            {
//                // Add product and save changes
//                await _userRepository.CreateProductAsync(product);

//                // Return product ID as a simple response
//                return Ok(new { productId = product.product_id });
//            }
//            catch (Exception ex)
//            {
//                // Handle any errors that might occur
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }


//        // GET: api/products/{id}
//        [HttpGet("GetProductById/{productId}")]
//        public async Task<ActionResult<products>> GetProductById(int productId)
//        {
//            var product = await _userRepository.GetProductById(productId);

//            if (product == null)
//            {
//                return NotFound();
//            }

//            return Ok(product);
//        }

//        // GET: api/User/GetAllProducts
//        [HttpGet("GetAllProducts/{BusRegId}")]
//        public async Task<ActionResult<products>> GetAllProducts(int BusRegId)
//        {
//            try
//            {
//                // Fetch all products from the repository
//                var products = await _userRepository.GetAllProductsAsync(BusRegId);

//                // Check if no products were found
//                if (products == null || !products.Any())
//                {
//                    return NotFound("No products found.");
//                }

//                // Return the list of products with a 200 OK status
//                return Ok(products);
//            }
//            catch (Exception ex)
//            {
//                // Handle any errors and return a 500 Internal Server Error
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        [HttpDelete("deleteProduct")]
//        public async Task<IActionResult> DeleteProductAsync(int productId)
//        {
//            try
//            {
               

//                // Use the repository to delete the product
//                await _userRepository.DeleteProductAsync(productId);

//                return Ok(new { message = "Product deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                Console.WriteLine($"Error deleting product: {ex.Message}");

//                // Return a generic error response
//                return StatusCode(500, new { message = "An error occurred while deleting the product." });
//            }
//        }

//        //[HttpPost("addBusinessProfile")]
//        //public async Task<IActionResult> AddBusinessProfile([FromBody] businessprofile businessProfile)
//        //{
//        //    if (businessProfile == null)
//        //    {
//        //        return BadRequest(new { message = "Invalid business profile data" });
//        //    }

//        //    try
//        //    {
//        //        // Add the business profile and get the saved object
//        //        var savedBusinessProfile = await _userRepository.AddBusinessProfileAsync(businessProfile);

//        //        // Return the saved business profile along with a success message
//        //        return Ok(new
//        //        {
//        //            message = "Business profile added successfully",
//        //            data = savedBusinessProfile
//        //        });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return StatusCode(500, new { message = "An error occurred while adding the business profile", error = ex.Message });
//        //    }
//        //}

//        [HttpPut("update-banner/{busRegId}")]
//        public async Task<IActionResult> UpdateBannerPath(int busRegId, [FromBody] UpdateBannerRequest request)
//        {
//            if (request == null || string.IsNullOrEmpty(request.BannerPath))
//                return BadRequest("Banner path cannot be empty");

//            bool isUpdated = await _userRepository.UpdateBannerPathAsync(busRegId, request.BannerPath);

//            if (!isUpdated)
//                return NotFound($"Business profile with BusRegId {busRegId} not found.");

//            return Ok(new { message = "Banner path updated successfully" });
//        }
//        //get sub categories of each business
//        [HttpGet("GetProductSubCategories/{BusRegId}")]
//        public IActionResult GetProductSubCategories(int busRegId)
//        {
//            var subCategories = _userRepository.GetProductSubCategoriesByBusRegId(busRegId);

//            if (subCategories == null || !subCategories.Any())
//            {
//                return NotFound(new { message = "No subcategories found for the given BusRegId." });
//            }

//            return Ok(subCategories);
//        }

//        [HttpPut("updateProduct")]
//        public IActionResult UpdateProduct([FromBody] products updatedProduct)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new { message = "Invalid product data" });

//            var isUpdated = _userRepository.UpdateProduct(updatedProduct);

//            if (!isUpdated)
//                return NotFound(new { message = "Product not found with the given ID" });

//            return Ok(new { message = "Product updated successfully" });
//        }


       

//        //get products for selected category and busregid on preview page
//        [HttpGet("by-busreg-and-subcat")]
//        public IActionResult GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
//        {
//            var products = _userRepository.GetProductsByBusRegIdAndSubcatId(busRegId, prodSubcatId);
//            if (products == null || !products.Any())
//            {
//                return NotFound("No products found for the given criteria.");
//            }

//            return Ok(products);
//        }

//        //ADMIN PANEL

//        // TO get all Business profiles with status

//        //// GET: api/User/GetAllBusinessProfiles
//        //[HttpGet("GetAllBusinessRegisters")]
//        //public async Task<IActionResult> GetAllBusinessRegisters()
//        //{
//        //    // Call the repository method to fetch all business registers
//        //    var businessProfiles = await _userRepository.GetAllBusinessRegisterAsync();

//        //    // Check if the list is empty and return appropriate response
//        //    if (businessProfiles == null || !businessProfiles.Any())
//        //    {
//        //        return Ok(new { data = new List<object>(), message = "No business profiles found." });
//        //    }

//        //    // Return the list of business profiles as JSON
//        //    return Ok(new { data = businessProfiles });
//        //}

//        [HttpGet("GetBusinessRegistersPaginated")]
//        public async Task<IActionResult> GetBusinessRegistersPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 2)
//        {
//            if (page < 1 || pageSize < 1)
//            {
//                return BadRequest(new { message = "Invalid pagination parameters." });
//            }

//            var (records, totalRecords) = await _userRepository.GetBusinessRegistersPaginatedAsync(page, pageSize);

//            return Ok(new
//            {
//                data = records,
//                totalRecords,
//                currentPage = page,
//                pageSize
//            });
//        }



//        // Add API to get unique counts for cities, states, and countries
//        [HttpGet("GetUniqueCounts")]
//        public async Task<IActionResult> GetUniqueCounts()
//        {
//            try
//            {
//                // Get unique counts from the repository
//                var (uniqueCities, uniqueStates, uniqueCountries) = await _userRepository.GetUniqueCountsAsync();

//                // Return the result
//                return Ok(new
//                {
//                    message = "Unique counts retrieved successfully",
//                    data = new
//                    {
//                        uniqueCities = uniqueCities,
//                        uniqueStates = uniqueStates,
//                        uniqueCountries = uniqueCountries
//                    }
//                });
//            }
//            catch (Exception ex)
//            {
//                // If there's an error, return a 500 status code
//                return StatusCode(500, new { message = "Error retrieving unique counts", error = ex.Message });
//            }
//        }

//        [HttpGet("getBusinessRegisterCount")]
//        public async Task<IActionResult> GetBusinessRegisterCount()
//        {
//            var count = await _userRepository.GetBusinessRegisterCountAsync();
//            return Ok(new { count });
//        }

//        #endregion

//        #region Shopper Registration

//        //[HttpPost("shopperregister")]
//        //public async Task<IActionResult> RegisterShopper([FromBody] ShopperRegister shopperDetails)
//        //{
//        //    if (shopperDetails == null)
//        //    {
//        //        return BadRequest("Shopper registration details cannot be null.");
//        //    }

//        //    // Validate model state
//        //    if (!ModelState.IsValid)
//        //    {
//        //        return BadRequest(ModelState);
//        //    }

//        //    // Ensure passwords match
//        //    if (shopperDetails.Password != shopperDetails.ConfirmPassword)
//        //    {
//        //        return BadRequest("Passwords do not match.");
//        //    }

//        //    // Check if email already exists
//        //    var existingShopper = await _userRepository.GetShopperByEmailAsync(shopperDetails.Email);
//        //    if (existingShopper != null)
//        //    {
//        //        return Conflict("Shopper with this email already exists.");
//        //    }

//        //    // Add the new shopper registration
//        //    var addedShopper = await _userRepository.AddShopperRegisterAsync(shopperDetails);

//        //    return CreatedAtAction(nameof(RegisterShopper), new { id = addedShopper.ShopperRegId }, addedShopper);
//        //}


//        [HttpGet("getShoppersRegisterCount")]
//        public async Task<IActionResult> getShoppersRegisterCount()
//        {
//            var count = await _userRepository.GetShoppersRegisterCountAsync();
//            return Ok(new { count });
//        }

      

//        [HttpGet("getShopperRegistersPaginated")]
//        public async Task<IActionResult> GetShopperRegistersPaginated(int page = 1, int pageSize = 10)
//        {
//            // Validate page and pageSize
//            if (page <= 0 || pageSize <= 0)
//            {
//                return BadRequest(new { message = "Page and page size must be greater than 0." });
//            }

//            // Call the repository method to fetch paginated shopper registers
//            var (shopperRegisters, totalRecords) = await _userRepository.GetShopperRegistersPaginatedAsync(page, pageSize);

//            // Check if there are any records
//            if (shopperRegisters == null || !shopperRegisters.Any())
//            {
//                return Ok(new { data = new List<object>(), message = "No shopper registers found.", totalRecords = 0 });
//            }

//            // Return the paginated list of shoppers as JSON
//            return Ok(new
//            {
//                data = shopperRegisters,
//                totalRecords,
//                currentPage = page,
//                pageSize
//            });
//        }

//        //}
//        [HttpGet("GetShopperDetails/{shopperRegId}")]
//        public async Task<IActionResult> GetShopperDetails(int shopperRegId)
//        {
//            var shopper = await _userRepository.GetShopperDetails(shopperRegId);

//            if (shopper == null)
//            {
//                return Ok(new { message = "No Data" }); // Return message instead of 404
//            }

//            return Ok(shopper);
//        }

//        #endregion

       

//        #region services
//        [HttpPost("AddService")]
//        public IActionResult AddService([FromBody] services service)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                _userRepository.AddServiceAsync(service);
//                return Ok(new { Message = "Service added successfully!" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { Message = "An error occurred while adding the service.", Error = ex.Message });
//            }
//        }
//        #endregion

//        #region landingpage search

//        //search to get products data based on location and category/product
//        [HttpGet("search")]
//        public IActionResult SearchBusinesses([FromQuery] string locationQuery, [FromQuery] string productQuery)
//        {
//            var filteredProducts = _userRepository.SearchBusinessesWithProducts(locationQuery, productQuery);

//            if (filteredProducts.Count == 0)
//                return NotFound("No products found matching your criteria.");

//            return Ok(filteredProducts);
//        }

//        //search to get business stores data based on location and category/product
//        [HttpGet("searchstore")]
//        public async Task<IActionResult> SearchBusinessstores([FromQuery] string location, [FromQuery] string categoryProduct)
//        {
//            // Ensure at least one search parameter is provided
//            if (string.IsNullOrEmpty(location) && string.IsNullOrEmpty(categoryProduct))
//            {
//                return BadRequest("At least one search term (location or category/product) must be provided.");
//            }

//            // Fetch search results from the repository
//            var searchResults = await _userRepository.SearchBusinessesAsync(location, categoryProduct);

//            // If no results are found, return an appropriate message
//            if (searchResults.Count == 0)
//            {
//                return NotFound("No matching business profiles found.");
//            }

//            return Ok(searchResults);
//        }

//        //[HttpGet("searchbusprofilebylocandproduct")]
//        //public IActionResult Search([FromQuery] string location, [FromQuery] string product)
//        //{
//        //    var results = _userRepository.SearchBusinessProfiles(location, product);

//        //    if (!results.Any())
//        //    {
//        //        return NotFound("No matching business profiles found.");
//        //    }

//        //    return Ok(results);
//        //}



//        ///** get profile based on location search 
//        //  [HttpGet("searchbylocation")]
//        //public ActionResult<List<businessprofile>> GetBusinessProfilesByLocation([FromQuery] string location)
//        //{
//        //    if (string.IsNullOrEmpty(location))
//        //    {
//        //        return BadRequest("Location is required.");
//        //    }

//        //    var profiles = _userRepository.GetBusinessProfilesByLocation(location);

//        //    if (profiles == null || profiles.Count == 0)
//        //    {
//        //        return NotFound("No matching business profiles found.");
//        //    }

//        //    return Ok(profiles);
//        //}


//        [HttpGet("search-business-by-product")]
//        public IActionResult SearchBusinessByCategoryOrProduct([FromQuery] string product)
//        {
//            if (string.IsNullOrEmpty(product))
//            {
//                return BadRequest("Product search string cannot be empty.");
//            }

//            var businessIds = _userRepository.GetBusinessProfilesBySearchTerm(product);

//            if (!businessIds.Any())
//            {
//                return NotFound("No businesses found for the given search term.");
//            }

//            return Ok(businessIds);
//        }

//        [HttpGet("SearchProfilesByProductAndLocation")]
//        public IActionResult SearchProfilesByProductAndLocation([FromQuery] string productSearchTerm, [FromQuery] string locationSearchTerm)
//        {
//            if (string.IsNullOrWhiteSpace(productSearchTerm) || string.IsNullOrWhiteSpace(locationSearchTerm))
//            {
//                return BadRequest(new { message = "Both product and location search terms are required." });
//            }

//            var businessProfiles = _userRepository.GetBusinessProfilesByProductAndLocation(productSearchTerm, locationSearchTerm);

//            if (!businessProfiles.Any())
//            {
//                return NotFound(new { message = "No matching business profiles found." });
//            }

//            return Ok(businessProfiles);
//        }

//        #endregion

//        #region payment gateway

//        [HttpPost("create-payment-intent")]
//        public ActionResult CreatePaymentIntent([FromBody] PaymentRequest paymentRequest)
//        {
//            try
//            {
//                // Get the currency code based on the country name
//                string currency = GetCurrencyFromCountry(paymentRequest.CountryName);

//                // If no valid currency is found, default to USD
//                if (currency == null)
//                {
//                    currency = "usd";
//                }

//                // Create the PaymentIntent options
//                var options = new PaymentIntentCreateOptions
//                {
//                    Amount = paymentRequest.Amount,  // Amount in cents (e.g., $10 = 1000)
//                    Currency = currency,             // Currency based on country name
//                    PaymentMethodTypes = new List<string> { "card" },
//                };

//                // Create the payment intent using Stripe's service
//                var service = new PaymentIntentService();
//                PaymentIntent intent = service.Create(options);

//                // Return the client secret to the frontend for confirming payment
//                return Ok(new { clientSecret = intent.ClientSecret });
//            }
//            catch (StripeException e)
//            {
//                // Return error if Stripe API call fails
//                return BadRequest(new { error = e.Message });
//            }
//        }

//        private string GetCurrencyFromCountry(string countryName)
//        {
//            // Example: Map country name to currency code
//            var countryCurrencyMapping = new Dictionary<string, string>
//    {
//        { "United States", "usd" },  // Country -> Currency code
//        { "India", "inr" },
//        { "United Kingdom", "gbp" },
//        { "European Union", "eur" },
//        { "Japan", "jpy" },
//        // Add other countries and currencies as needed
//    };

//            // Return the currency code if found, otherwise return null
//            if (countryCurrencyMapping.ContainsKey(countryName))
//            {
//                return countryCurrencyMapping[countryName];
//            }

//            return null;  // Return null if no valid currency is found
//        }
//        #endregion

//        #region Shopping Cart

//        [HttpPost("add-to-cart")]
//        public async Task<IActionResult> AddToCart([FromBody] addtocart cartItem)
//        {
//            if (cartItem == null)
//                return BadRequest("Invalid request data");

//            try
//            {
//                var updatedCartItem = await _userRepository.AddToCart(cartItem);
//                return Ok(updatedCartItem);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal Server Error: {ex.Message}");
//            }
//        }


//        [HttpGet("GetCartItems/{shopperRegId}")]
//        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(int shopperRegId)
//        {
//            var cartItems = await _userRepository.GetCartItems(shopperRegId);

//            if (cartItems == null || !cartItems.Any())
//            {
//                return NotFound("No items found in the cart.");
//            }

//            return Ok(cartItems);
//        }

//        [HttpPut("IncreaseCartQty/{cartId}")]
//        public async Task<IActionResult> IncreaseCartQty(int cartId)
//        {
//            var success = await _userRepository.IncreaseCartItemQty(cartId);

//            if (!success)
//            {
//                return NotFound("Cart item not found.");
//            }

//            return Ok("Cart item quantity increased.");
//        }
//        [HttpPut("DecreaseCartQty/{cartId}")]
//        public async Task<IActionResult> DecreaseCartQty(int cartId)
//        {
//            var success = await _userRepository.DecreaseCartItemQty(cartId);

//            if (!success)
//            {
//                return NotFound("Cart item not found.");
//            }

//            return Ok("Cart item quantity decreased.");
//        }
//        [HttpDelete("RemoveFromCart/{cartId}")]
//        public async Task<IActionResult> RemoveFromCart(int cartId)
//        {
//            var success = await _userRepository.RemoveFromCart(cartId);

//            if (!success)
//            {
//                return NotFound("Cart item not found.");
//            }

//            return Ok("Cart item removed successfully.");
//        }

//        [HttpPut("MoveToWishlist/{cartId}")]
//        public async Task<IActionResult> MoveToWishlist(int cartId)
//        {
//            var result = await _userRepository.MoveToWishlist(cartId);
//            if (result) return Ok(new { message = "Item moved to wishlist!" });
//            return NotFound(new { message = "Item not found!" });
//        }

//        [HttpPut("MoveBackToCart/{cartId}")]
//        public async Task<IActionResult> MoveBackToCart(int cartId)
//        {
//            var result = await _userRepository.MoveBackToCart(cartId);
//            if (result) return Ok(new { message = "Item moved back to cart!" });
//            return NotFound(new { message = "Item not found!" });
//        }
//        [HttpPost("CreateOrder")]
//        public async Task<IActionResult> CreateOrder([FromQuery] int shopperRegId, [FromQuery] string shippingType)
//        {
//            var orderId = await _userRepository.CreateOrderAsync(shopperRegId, shippingType);

//            if (orderId == 0)
//            {
//                return BadRequest("No items in cart to place an order.");
//            }

//            return Ok(new { Message = "Order placed successfully", OrderId = orderId });
//        }

//        [HttpPost("AddPayment")]
//        public IActionResult AddPayment([FromBody] PaymentRequestModel model)
//        {
//            if (model == null || model.OrderId <= 0 || model.AmountPaid <= 0 || string.IsNullOrEmpty(model.PaymentMethod))
//            {
//                return BadRequest("Invalid payment details.");
//            }

//            var payment = _userRepository.AddPayment(model.OrderId, model.AmountPaid, model.PaymentMethod);

//            return Ok(new { message = "Payment successful!", paymentId = payment.PaymentId });
//        }

//        [HttpPost("update-cart-status/{orderId}")]
//        public async Task<IActionResult> UpdateCartStatus(int orderId)
//        {
//            var result = await _userRepository.UpdateCartStatusAsync(orderId);
//            if (!result)
//            {
//                return NotFound(new { message = "Order or Cart items not found." });
//            }

//            return Ok(new { message = "Cart status updated successfully." });
//        }
//        #endregion 
//    }




//}











