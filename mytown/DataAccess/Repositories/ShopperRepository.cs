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
        public async Task<IEnumerable<ProdcVariantforShopperDto>> GetRecentlyViewedProductsAsync(
      int shopperId, int days = 7, int limit = 10)
        {
            var sinceDate = DateTime.UtcNow.AddDays(-days);

            var productDtos = await _context.ShopperProductRecentViews
                .Where(v => v.ShopperId == shopperId && v.LastViewedAt >= sinceDate)
                .OrderByDescending(v => v.LastViewedAt)
                .Include(v => v.Product)
                    .ThenInclude(p => p.BusinessRegister)
                .Include(v => v.Product)
                    .ThenInclude(p => p.Sku_ProductVariants)
                        .ThenInclude(s => s.Images)
                .Include(v => v.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(v => v.Product)
                    .ThenInclude(p => p.Fabric)
                .Include(v => v.Product)
                    .ThenInclude(p => p.Design)
                .Select(v => new ProdcVariantforShopperDto
                {
                    ProductId = v.Product.product_id,
                    BusRegId = v.Product.BusRegId,
                    BusinessName = v.Product.BusinessRegister.Businessname,
                    BuscatId = v.Product.BuscatId,
                  //  BuscatName = v.Product.BusinessRegister.BusinessCategoryName, // if you have it
                    ProdcatId = v.Product.prod_subcat_id,
                 //   ProdcatName = v.Product.ProductSubCategoryName, // if you have it
                    ProductTypeId = v.Product.ProductTypeId,
                    ProductTypeName = v.Product.ProductType != null ? v.Product.ProductType.prod_type_name : null,
                    FabricId = v.Product.FabricId,
                    FabricName = v.Product.Fabric != null ? v.Product.Fabric.fabric_name : null,
                    DesignId = v.Product.DesignId,
                    DesignName = v.Product.Design != null ? v.Product.Design.design_name : null,
                    ProductName = v.Product.product_name,
                    ProductDescription = v.Product.product_description,
                    SupplierName = v.Product.supplier_name,

                    Variants = v.Product.Sku_ProductVariants.Select(s => new Sku_ProductVariantDto
                    {
                        SkuId_Productvariant = s.SkuId,
                        ProductId = s.ProductId,
                        Color = s.Color,
                        SizeId = s.SizeId,
                        SizeName = s.Size != null ? s.Size.SizeName : null,
                        Sku_Cost = s.Sku_Cost,
                        DiscountPrice = s.DiscountPrice,
                        Quantity = s.Quantity,
                        Length = s.Length,
                        Width = s.Width,
                        Height = s.Height,
                        Weight = s.Weight,
                        Discount = s.Discount,
                        Images = s.Images
                            .OrderBy(i => i.SortOrder)
                            .Select(img => new ProductImageDto
                            {
                                FileName = img.FileName,
                                SortOrder = img.SortOrder
                            })
                            .ToList()
                    }).ToList()
                })
                .Take(limit)
                .ToListAsync();

            return productDtos;
        }


        // Shopper Alternate Address

        public async Task<IEnumerable<ShopperAlternateAddressDto>> GetAddressesByShopperIdAsync(int shopperRegId)
        {
            return await _context.ShopperAlternateAddresses
                .Where(a => a.ShopperRegId == shopperRegId)
                .Select(a => new ShopperAlternateAddressDto
                {
                    AltAddressId = a.AltAddressId,
                    ShopperRegId = a.ShopperRegId,
                    AltName = a.AltName,
                    AltPhoneNumber = a.AltPhoneNumber,
                    AltAddress = a.AltAddress,
                    AltTown = a.AltTown,
                    AltCity = a.AltCity,
                    AltState = a.AltState,
                    AltCountry = a.AltCountry,
                    AltPostalCode = a.AltPostalCode,
                    DeliveryNotes = a.DeliveryNotes
                })
                .ToListAsync();
        }

        public async Task<ShopperAlternateAddressDto?> GetAddressByIdAsync(int id)
        {
            return await _context.ShopperAlternateAddresses
                .Where(a => a.AltAddressId == id)
                .Select(a => new ShopperAlternateAddressDto
                {
                    AltAddressId = a.AltAddressId,
                    AltName = a.AltName,
                    AltPhoneNumber = a.AltPhoneNumber,
                    AltAddress = a.AltAddress,
                    AltTown = a.AltTown,
                    AltCity = a.AltCity,
                    AltState = a.AltState,
                    AltCountry = a.AltCountry,
                    AltPostalCode = a.AltPostalCode,
                    DeliveryNotes = a.DeliveryNotes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ShopperAlternateAddressDto> AddAddressAsync(ShopperAlternateAddress address)
        {
            _context.ShopperAlternateAddresses.Add(address);
            await _context.SaveChangesAsync();

            return new ShopperAlternateAddressDto
            {
                AltAddressId = address.AltAddressId,
                AltName = address.AltName,
                AltPhoneNumber = address.AltPhoneNumber,
                AltAddress = address.AltAddress,
                AltTown = address.AltTown,
                AltCity = address.AltCity,
                AltState = address.AltState,
                AltCountry = address.AltCountry,
                AltPostalCode = address.AltPostalCode,
                DeliveryNotes = address.DeliveryNotes
            };
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _context.ShopperAlternateAddresses.FindAsync(id);
            if (address == null) return false;

            _context.ShopperAlternateAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }


    }



}






