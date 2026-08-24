using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

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

            var productDtos = await (
                from rv in _context.ShopperProductRecentViews

                join p in _context.ProductsNew
                    on rv.ProductId equals p.ProductId

                join bp in _context.BusinessRegisters
                    on p.BusRegId equals bp.BusRegId

                join pt in _context.Product_Types
                    on p.ProdTypeId equals (long?)pt.ProdTypeId into ptJoin
                from pt in ptJoin.DefaultIfEmpty()

                where rv.ShopperId == shopperId
                      && rv.LastViewedAt >= sinceDate
                      && p.IsActive
                      && p.ProductStatus == "ACTIVE"

                orderby rv.LastViewedAt descending

                select new ProdcVariantforShopperDto
                {
                    ProductId = (int)p.ProductId,

                    BusRegId = p.BusRegId,
                    BusinessName = bp.BusinessName,

                    BuscatId = (int)(p.BusCatId ?? 0),

                    Location = $"{bp.BusinessCity}, {bp.BusinessState}",
                    Country = bp.BusinessCountry,

                    ProdcatId = (int)(p.ProdSubcatId ?? 0),

                    ProductTypeId = (int?)p.ProdTypeId,
                    ProductTypeName = pt != null
                        ? pt.ProdTypeName
                        : null,

                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,

                    SupplierName = bp.BusinessName,

                    Variants = _context.ProductVariantsNew
                        .Where(v => v.ProductId == p.ProductId)
                        .Select(v => new Sku_ProductVariantDto
                        {
                            SkuId_Productvariant = (int)v.SkuId,
                            ProductId = (int)v.ProductId,

                            Sku_Cost = v.Price,
                            DiscountPrice = v.DiscountPrice,
                            Quantity = v.StockQuantity,
                            Weight = v.Weight,
                            Discount = v.Discount,

                            Images = _context.ProductVariantImagesNew
                                .Where(i => i.SkuId == v.SkuId)
                                .OrderBy(i => i.SortOrder)
                                .Select(i => new ProductImageDto
                                {
                                    FileName = i.FileName,
                                    SortOrder = i.SortOrder
                                })
                                .ToList(),

                            Attributes = v.Attributes
                                .Select(a => new VariantAttributeDto
                                {
                                    AttributeId = (int)a.AttributeId,

                                    AttributeValueId = a.AttributeValueId.HasValue
                                        ? (int?)a.AttributeValueId.Value
                                        : null,

                                    AttributeValue = a.AttributeValue
                                        ?? _context.ProductAttributeValues
                                            .Where(av =>
                                                a.AttributeValueId.HasValue &&
                                                av.AttributeValueId ==
                                                (int)a.AttributeValueId.Value)
                                            .Select(av => av.AttributeValue)
                                            .FirstOrDefault()
                                })
                                .ToList()
                        })
                        .ToList()
                }
            )
            .Take(limit)
            .ToListAsync();

            return productDtos;
        }
        public async Task<IEnumerable<ShopperAlternateAddressDto>> GetAddressesByShopperIdAsync(int shopperRegId)
        {
            return await _context.ShopperAlternateAddresses
                .Where(a => a.ShopperRegId == shopperRegId && !a.IsDeleted)
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
                .Where(a => a.AltAddressId == id && !a.IsDeleted)
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

        // Shopper Experiences

        public async Task<ShopperExperience> CreateExperienceAsync(ShopperExperience experience)
        {
            _context.ShopperExperiences.Add(experience);
            await _context.SaveChangesAsync();
            return experience;
        }

        public async Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId)
        {
            return await (from e in _context.ShopperExperiences
                          join s in _context.ShopperRegisters on e.ShopperRegId equals s.ShopperRegId
                          join b in _context.BusinessRegisters on e.BusRegId equals b.BusRegId
                          where e.BusRegId == busRegId && e.Status == "Approved"
                          orderby e.CreatedDate descending
                          select new ShopperExperienceDto
                          {
                              ShopperExperienceId = e.ShopperExperienceId,
                              ShopperRegId = e.ShopperRegId,
                              ShopperName = e.IsAnonymous ? "Anonymous" : s.Username,
                              BusRegId = e.BusRegId,
                              BusinessName = b.BusinessName,
                              PostType = e.PostType,
                              Rating = e.Rating,
                              Title = e.Title,
                              Experience = e.Experience,
                              IsAnonymous = e.IsAnonymous,
                              Status = e.Status,
                              CreatedDate = e.CreatedDate
                          }).ToListAsync();
        }
    }
}