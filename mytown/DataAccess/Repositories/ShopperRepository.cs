using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class ShopperRepository : IShopperRepository
    {
        private readonly AppDbContext _context;

        public ShopperRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<ShopperRegister> RegisterShopper(ShopperRegister shopper)
        //{
        //    if (await IsEmailTaken(shopper.Email))
        //        return null;
        //    //throw new Exception("Email is already in use.");
        //    shopper.IsEmailVerified = false;

        //    _context.ShopperRegisters.Add(shopper);
        //    await _context.SaveChangesAsync();

        //    return shopper;
        //}

        //public async Task<bool> IsEmailTaken(string email)
        //{
        //    return await _context.ShopperRegisters.AnyAsync(s => s.Email == email);
        //}

        ////public async Task<ShopperVerification> GenerateEmailVerification(string email)
        ////{
        ////    var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == email);
        ////    if (shopper == null) throw new Exception("User not found.");

        ////    var token = Guid.NewGuid().ToString();
        ////    var expiryDate = DateTime.UtcNow.AddHours(24);

        ////    var verification = new ShopperVerification
        ////    {
        ////        Email = email,
        ////        VerificationToken = token,
        ////        ExpiryDate = expiryDate,
        ////        IsVerified = false
        ////    };

        ////    _context.ShopperVerification.Add(verification);
        ////    await _context.SaveChangesAsync();

        ////    return verification;
        ////}

        //public async Task SaveVerificationToken(int shopperId, string token, DateTime expiryDate)
        //{
        //    var verificationToken = new ShopperVerification
        //    {
        //        ShopperId = shopperId,
        //        VerificationToken = token,
        //        ExpiryDate = expiryDate,
        //        IsUsed = false,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.ShopperVerification.Add(verificationToken);
        //    await _context.SaveChangesAsync();
        //}


        //public async Task<bool> VerifyEmail(string token)
        //{
        //    // Look up the verification token
        //    var verification = await _context.ShopperVerification
        //        .FirstOrDefaultAsync(v => v.VerificationToken == token && !v.IsUsed);

        //    if (verification == null || verification.ExpiryDate < DateTime.UtcNow)
        //        return false;

        //    // Find the associated shopper
        //    var shopper = await _context.ShopperRegisters
        //        .FirstOrDefaultAsync(s => s.ShopperRegId == verification.ShopperId);

        //    if (shopper == null)
        //        return false;

        //    // Mark email as verified
        //    shopper.IsEmailVerified = true;
        //    _context.ShopperRegisters.Update(shopper);

        //    // Mark the token as used
        //    verification.IsUsed = true;
        //    _context.ShopperVerification.Update(verification);

        //    await _context.SaveChangesAsync();
        //    return true;
        //}


        //public async Task<ShopperVerification> FindVerificationByToken(string token)
        //{
        //    return await _context.ShopperVerification.FirstOrDefaultAsync(v => v.VerificationToken == token);
        //}

        //public async Task RemoveVerification(ShopperVerification verification)
        //{
        //    _context.ShopperVerification.Remove(verification);
        //    await _context.SaveChangesAsync();
        //}


        public async Task SavePendingVerification(PendingVerification pending)
        {
            _context.PendingVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingVerification> FindPendingVerificationByToken(string token)
        {
            return await _context.PendingVerifications
                .FirstOrDefaultAsync(p => p.Token == token);
        }

        public async Task DeletePendingVerification(string token)
        {
            var pending = await _context.PendingVerifications
                .FirstOrDefaultAsync(p => p.Token == token);

            if (pending != null)
            {
                _context.PendingVerifications.Remove(pending);
                await _context.SaveChangesAsync();
            }
        }

        // Other existing shopper methods like:
        public async Task<(bool isTaken, string message)> IsEmailTaken(string email)
        {
            var shopper = await _context.ShopperRegisters
        .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());

            if (shopper == null || shopper.status == "Deactivated")
                return (false, null); // Treat as new

            if (shopper.status == "Blocked")
                return (true, "This email is blocked. Please contact support.");

            return (true, null); 
        }

        public async Task<ShopperRegister> RegisterShopper(ShopperRegister shopper)
        {
            try
            {
                _context.ShopperRegisters.Add(shopper);
                await _context.SaveChangesAsync();
                return shopper;
            }
            catch (DbUpdateException ex)
            {
                // Database update-related issues (e.g., constraint violations)
                Console.WriteLine("Database Update Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);

                throw new Exception("There was an error saving the shopper registration to the database.");
            }
            catch (Exception ex)
            {
                // General fallback
                Console.WriteLine("General Exception: " + ex.Message);
                throw new Exception("An unexpected error occurred during shopper registration.");
            }
        }


        // resend email verfication
        public async Task<ShopperVerification> FindPendingVerificationByEmail(string email)
        {
            return await _context.ShopperVerification
                .Include(sv => sv.Shopper)
                .Where(sv => sv.Shopper.Email == email && !sv.IsUsed && sv.ExpiryDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }


        public async Task RemoveVerification(ShopperVerification verification)
        {
            _context.ShopperVerification.Remove(verification);
            await _context.SaveChangesAsync();
        }

        public async Task<ShopperRegister> GetShopperByIdAsync(int shopperRegId)
        {
            return await _context.ShopperRegisters
                                .FirstOrDefaultAsync(b => b.ShopperRegId == shopperRegId);
        }

        public async Task<IEnumerable<object>> GetTownsWithStoreCountByCountryAsync(string country)
        {
            return await _context.BusinessRegisters
                .Where(br => br.BusinessProfile != null && br.businessCountry == country) 
                .GroupBy(br => br.Town)                                                  
                .Select(g => new
                {
                    Town = g.Key,
                    StoreCount = g.Count()
                })
                .ToListAsync();
        }
        // get recently viewed products for that shopper
        public async Task<IEnumerable<ProductDto>> GetRecentlyViewedProductsAsync(
    int shopperId, int days = 7, int limit = 10)
        {
            var sinceDate = DateTime.UtcNow.AddDays(-days);

            var productDtos = await _context.ShopperProductRecentViews
                .Where(v => v.ShopperId == shopperId && v.LastViewedAt >= sinceDate)
                .OrderByDescending(v => v.LastViewedAt)
                .Include(v => v.Product)                     // include product
                    .ThenInclude(p => p.BusinessRegister)    // include store
                .Include(v => v.Product)
                    .ThenInclude(p => p.Images)              // include images
                .Select(v => new ProductDto
                {
                    ProductId = v.Product.product_id,
                    BusRegId = v.Product.BusRegId,
                    BuscatId = v.Product.BuscatId,
                    ProdSubcatId = v.Product.prod_subcat_id,
                    ProductName = v.Product.product_name,
                    ProductSubject = v.Product.product_subject,
                    ProductDescription = v.Product.product_description,
                    ProductAmount = v.Product.product_cost ?? 0,
                    ProductLength = v.Product.product_length ?? 0,
                    ProductWidth = v.Product.product_width ?? 0,
                    ProductWeight = v.Product.product_weight ?? 0,
                    Quantity = v.Product.product_quantity ?? 0,
                    ProductHeight = v.Product.product_height ?? 0,
                    Size = v.Product.size,
                    Color = v.Product.color,
                    Discount = v.Product.discount,
                    DiscountPrice = v.Product.discount_price,
                    BusinessName = v.Product.BusinessRegister.Businessname,

                    // 🔹 Map product images
                    Images = v.Product.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                })
                .Take(limit)
                .ToListAsync();

            return productDtos;
        }



    }





}
