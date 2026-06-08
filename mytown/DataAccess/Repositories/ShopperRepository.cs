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

        public async Task<(bool isTaken, string message)> IsEmailTaken(string email)
        {
            var shopper = await _context.ShopperRegisters
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());

            if (shopper == null || shopper.Status == "Deactivated")
                return (false, null);

            if (shopper.Status == "Blocked")
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
                Console.WriteLine("Database Update Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);

                throw new Exception("There was an error saving the shopper registration to the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: " + ex.Message);
                throw new Exception("An unexpected error occurred during shopper registration.");
            }
        }

        public async Task<PendingVerification> FindPendingVerificationByEmail(string email)
        {
            return await _context.PendingVerifications
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower()
                                        && p.ExpiryDate > DateTime.UtcNow);
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
                .Where(br => br.BusinessProfile != null && br.BusinessCountry == country)
                .GroupBy(br => br.Town)
                .Select(g => new
                {
                    Town = g.Key,
                    StoreCount = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProdcVariantforShopperDto>> GetRecentlyViewedProductsAsync(
            int shopperId, int days = 7, int limit = 10)
        {
            var sinceDate = DateTime.UtcNow.AddDays(-days);

            var productDtos = await _context.ShopperProductRecentViews
                .Where(v => v.ShopperId == shopperId && v.LastViewedAt >= sinceDate && v.Product.IsActive
                    && v.Product.ProductStatus == "Approved")
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
                    ProductId = v.Product.ProductId,
                    BusRegId = v.Product.BusRegId,
                    BusinessName = v.Product.BusinessRegister.BusinessName,
                    BuscatId = v.Product.BuscatId,
                    ProdcatId = v.Product.ProdSubcatId,
                    ProductTypeId = v.Product.ProductTypeId,
                    ProductTypeName = v.Product.ProductType != null ? v.Product.ProductType.ProdTypeName : null,
                    FabricId = v.Product.FabricId,
                    FabricName = v.Product.Fabric != null ? v.Product.Fabric.FabricName : null,
                    DesignId = v.Product.DesignId,
                    DesignName = v.Product.Design != null ? v.Product.Design.DesignName : null,
                    ProductName = v.Product.ProductName,
                    ProductDescription = v.Product.ProductDescription,
                    SupplierName = v.Product.SupplierName,
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
            ShopperAlternateAddress entity;

            if (address.AltAddressId > 0)
            {
                entity = await _context.ShopperAlternateAddresses
                    .FirstOrDefaultAsync(a =>
                        a.AltAddressId == address.AltAddressId &&
                        a.ShopperRegId == address.ShopperRegId)
                    ?? throw new Exception("Address not found or unauthorized");

                entity.AltName = address.AltName;
                entity.AltPhoneNumber = address.AltPhoneNumber;
                entity.AltAddress = address.AltAddress;
                entity.AltTown = address.AltTown;
                entity.AltCity = address.AltCity;
                entity.AltState = address.AltState;
                entity.AltCountry = address.AltCountry;
                entity.AltPostalCode = address.AltPostalCode;
                entity.DeliveryNotes = address.DeliveryNotes;
            }
            else
            {
                entity = address;
                _context.ShopperAlternateAddresses.Add(entity);
            }

            await _context.SaveChangesAsync();

            return new ShopperAlternateAddressDto
            {
                AltAddressId = entity.AltAddressId,
                ShopperRegId = entity.ShopperRegId,
                AltName = entity.AltName,
                AltPhoneNumber = entity.AltPhoneNumber,
                AltAddress = entity.AltAddress,
                AltTown = entity.AltTown,
                AltCity = entity.AltCity,
                AltState = entity.AltState,
                AltCountry = entity.AltCountry,
                AltPostalCode = entity.AltPostalCode,
                DeliveryNotes = entity.DeliveryNotes
            };
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _context.ShopperAlternateAddresses
                .FirstOrDefaultAsync(a => a.AltAddressId == id && !a.IsDeleted);
            if (address == null) return false;

            address.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        //  New method - Check if email exists
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.ShopperRegisters
                .AnyAsync(s => s.Email.ToLower() == email.ToLower());
        }
    }
}