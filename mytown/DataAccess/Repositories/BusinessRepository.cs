using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;
using Stripe;
using System.Text;
using System.Text.Json;
using static mytown.Models.busprofilepreview;


namespace mytown.DataAccess.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        private readonly HttpClient _httpClient;

        public BusinessRepository(AppDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
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

        // resend email verification
        public async Task<PendingBusinessVerification> FindPendingVerificationByEmail(string email)
        {
            return await _context.PendingBusinessVerifications
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower()
                                        && p.ExpiryDate > DateTime.UtcNow);
        }

        //public async Task DeletePendingVerification(string token)
        //{
        //    var pending = await _context.PendingBusinessVerifications
        //        .FirstOrDefaultAsync(p => p.Token == token);

        //    if (pending != null)
        //    {
        //        _context.PendingBusinessVerifications.Remove(pending);
        //        await _context.SaveChangesAsync();
        //    }
        //}



        //get business owner home page with busregid
        public async Task<BusinessRegister> GetBusinessByIdAsync(int busRegId)
        {
            //return await _context.BusinessRegisters
            //                    .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            return await _context.BusinessRegisters
                        .Include(b => b.BusinessProfile) // load related profile
                        .FirstOrDefaultAsync(b => b.BusRegId == busRegId);
        }

        //create profile during business registration

        public async Task CreateProfile(BusinessProfile profile)
        {
            _context.BusinessProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        // add bank account details
        public async Task SaveBusinessAccountDetails(BusinessAccountDetail businessAccountDetail)
        {
            _context.BusinessAccountDetails.Add(businessAccountDetail);
            await _context.SaveChangesAsync();
        }
        //get business store types
        public async Task<ActionResult<IEnumerable<BusinessCategory>>> GetBusinessCategories()
        {
            return await _context.BusinessCategories.ToListAsync();
        }

        //get business services types
        public async Task<ActionResult<IEnumerable<BusinessService>>> GetBusinessServices()
        {
            return await _context.BusinessServices.ToListAsync();
        }

        // Fetch subcategories by BuscatId
        public async Task<IEnumerable<ProductSubCategory>> BusinessSubCategoriesforStores(int buscatId)
        {
            return await _context.product_sub_categories
                                 .Where(p => p.BuscatId == buscatId)
                                 .ToListAsync();
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

        //get categories ofproducts for a businessid
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






     
        //get products for selected category
        public IEnumerable<Products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
        {
            return _context.products
                           .Where(p => p.BusRegId == busRegId && p.ProdSubcatId == prodSubcatId)
                           .ToList();
        }

        public async Task<IEnumerable<ProductGroupResponseDto>> GetProductGroupsBySubCategoryId(int prodSubcatId)
        {
            return await _context.Product_Groups
                .Where(x => x.ProdSubcatId == prodSubcatId)
                .Select(x => new ProductGroupResponseDto
                {
                    ProdGroupId = x.ProdGroupId,
                    ProdGroupName = x.ProdGroupName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesByGroupAndSubCategory(
    int prodSubcatId,
    int prodGroupId)
        {
            return await _context.Product_Types
                .Where(x =>
                    x.ProdSubcatId == prodSubcatId &&
                    x.ProdGroupId == prodGroupId)
                .Select(x => new ProductType
                {
                    ProdTypeId = x.ProdTypeId,
                    ProdSubcatId = x.ProdSubcatId,
                    ProdGroupId = x.ProdGroupId,
                    ProdTypeName = x.ProdTypeName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductAttributeDto>> GetAttributesBySubCategoryId(
      int prodSubcatId,
      int busCatId,
      int productGroupId)
        {
            return await _context.ProductAttributes
                .Where(x =>
                    x.ProdSubcatId == prodSubcatId &&
                    x.BusCatId == busCatId &&
                    (
                        x.ProductGroupId == null ||
                        x.ProductGroupId == productGroupId
                    ))
                .Select(x => new ProductAttributeDto
                {
                    AttributeId = x.AttributeId,
                    AttributeName = x.AttributeName,
                    ProdSubcatId = x.ProdSubcatId,
                    BusCatId = x.BusCatId,
                    ProductGroupId = x.ProductGroupId,

                    Values = _context.ProductAttributeValues
                        .Where(v => v.AttributeId == x.AttributeId)
                        .Select(v => new ProductAttributeValueDto
                        {
                            AttributeValueId = v.AttributeValueId,
                            AttributeValue = v.AttributeValue
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<BankVerificationResponseDto> VerifyBankAccountAsync(
       BankVerificationRequestDto request)
        {
            var clientId = _configuration["CashfreeVerification:ClientId"];
            var clientSecret = _configuration["CashfreeVerification:ClientSecret"];

            var url =
                "https://sandbox.cashfree.com/verification/bank-account/sync";

            var payload = new
            {
                bank_account = request.AccountNumber,
                ifsc = request.Ifsc
            };

            var json = JsonSerializer.Serialize(payload);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                url);

            httpRequest.Headers.Add("x-client-id", clientId);
            httpRequest.Headers.Add("x-client-secret", clientSecret);

            httpRequest.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new BankVerificationResponseDto
                {
                    Success = false,
                    Message = responseContent
                };
            }

            using var document =
                JsonDocument.Parse(responseContent);

            var root = document.RootElement;

            string? accountHolderName = null;
            string? bankName = null;

            if (root.TryGetProperty(
                    "name_at_bank",
                    out var nameProperty))
            {
                accountHolderName =
                    nameProperty.GetString();
            }

            if (root.TryGetProperty(
                    "bank_name",
                    out var bankProperty))
            {
                bankName =
                    bankProperty.GetString();
            }

            return new BankVerificationResponseDto
            {
                Success = true,
                AccountHolderName = accountHolderName,
                BankName = bankName,
                Message = "Bank account verified successfully."
            };
        }
    }

}
