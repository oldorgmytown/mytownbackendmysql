using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.DataAccess.Interfaces;
using mytown.Models.mytown.DataAccess;
using mytown.Models.DTO_s;
using Azure.Storage.Blobs;

namespace mytown.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ProductRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ---------------- Add Product (old-shaped in/out, new-table storage) ----------------

        public async Task<Products> AddProductAsync(Products product)
        {
            var entity = new ProductsNew
            {
                BusRegId = product.BusRegId,
                BusCatId = product.BuscatId,          // int -> long? implicit, fine
                ProdSubcatId = product.ProdSubcatId,  // int -> long? implicit, fine
                ProductName = product.ProductName ?? string.Empty,
                ProductDescription = product.ProductDescription,
                ProductStatus = product.ProductStatus ?? "Pending",
                IsActive = product.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ProductsNew.Add(entity);
            await _context.SaveChangesAsync();

            product.ProductId = (int)entity.ProductId;
            return product;
        }

        public async Task<Sku_ProductVariant> AddProductVariantAsync(Sku_CreateVariantDto dto)
        {
            var parentProduct = await _context.ProductsNew
                .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

            if (parentProduct == null)
                throw new Exception($"Product {dto.ProductId} not found.");

            var variant = new ProductVariantNew
            {
                ProductId = parentProduct.ProductId,
                Price = dto.Sku_Cost ?? 0,
                DiscountPrice = dto.DiscountPrice,
                Discount = dto.Discount ?? 0,
                StockQuantity = dto.Quantity ?? 0,
                Weight = dto.Weight,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ProductVariantsNew.Add(variant);
            await _context.SaveChangesAsync(); // generates VariantId

            await UpsertColorSizeAttributesAsync(
                variant, dto.Color, dto.SizeId,
                parentProduct.ProdSubcatId, parentProduct.BusCatId);

            if (dto.Images != null && dto.Images.Any())
            {
                int order = 1;
                foreach (var file in dto.Images)
                {
                    var fileName = await UploadToBlobAsync(file, "product");

                    _context.ProductVariantImagesNew.Add(new ProductVariantImageNew
                    {
                        SkuId = variant.SkuId,
                        FileName = fileName,
                        SortOrder = order++,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await _context.SaveChangesAsync();
            }

            return MapVariantToOld(variant, dto.Color, dto.SizeId);
        }

        public async Task<ProductSizeMeasurementDto?> GetMeasurementBySizeIdAsync(int sizeId)
        {
            return await _context.ProductSize_Measurements
                .Where(m => m.SizeId == sizeId)
                .Select(m => new ProductSizeMeasurementDto
                {
                    MeasurementId = m.MeasurementId,
                    SizeId = m.SizeId,
                    Length = m.Length,
                    Height = m.Height,
                    Width = m.Width,
                    Weight = m.Weight,
                    Unit = m.Unit
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProdVariantdetailsDto?> GetProductandVariantAsync(int productId)
        {
            var product = await _context.ProductsNew
                .Include(p => p.BusinessRegister)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) return null;

            return MapProductToDto(product);
        }

        public async Task<Products> UpdateProductAsync(int productId, ProductCreateDto dto)
        {
            var existing = await _context.ProductsNew.FindAsync((long)productId);
            if (existing == null) return null;

            if (dto.BuscatId != 0) existing.BusCatId = dto.BuscatId;
            if (dto.ProdSubcatId != 0) existing.ProdSubcatId = dto.ProdSubcatId;
            if (!string.IsNullOrWhiteSpace(dto.ProductName)) existing.ProductName = dto.ProductName;
            if (!string.IsNullOrWhiteSpace(dto.ProductDescription)) existing.ProductDescription = dto.ProductDescription;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Products
            {
                ProductId = (int)existing.ProductId,
                BusRegId = existing.BusRegId,
                BuscatId = (int)(existing.BusCatId ?? 0),
                ProdSubcatId = (int)(existing.ProdSubcatId ?? 0),
                ProductName = existing.ProductName,
                ProductDescription = existing.ProductDescription,
                ProductStatus = existing.ProductStatus,
                IsActive = existing.IsActive
            };
        }

        public async Task<Sku_ProductVariant?> UpdateVariantAsync(Sku_ProductVariantDto dto)
        {
            var variant = await _context.ProductVariantsNew
                .Include(v => v.Attributes)
                .Include(v => v.Images)
                .FirstOrDefaultAsync(v => v.SkuId== dto.SkuId_Productvariant);

            if (variant == null) return null;

            if (dto.Sku_Cost.HasValue) variant.Price = dto.Sku_Cost.Value;
            if (dto.Quantity.HasValue) variant.StockQuantity = dto.Quantity.Value;
            if (dto.DiscountPrice.HasValue) variant.DiscountPrice = dto.DiscountPrice.Value;
            if (dto.Weight.HasValue) variant.Weight = dto.Weight.Value;
            if (dto.Discount.HasValue) variant.Discount = dto.Discount.Value;
            variant.UpdatedAt = DateTime.UtcNow;

            // Replace attributes entirely with what was submitted
            if (dto.Attributes != null)
            {
                _context.ProductVariantAttributesNew.RemoveRange(variant.Attributes);

                foreach (var attr in dto.Attributes)
                {
                    _context.ProductVariantAttributesNew.Add(new ProductVariantAttributeNew
                    {
                        SkuId = variant.SkuId,
                        AttributeId = attr.AttributeId,
                        AttributeValueId = attr.AttributeValueId,
                        AttributeValue = attr.AttributeValue,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Replace images entirely with the submitted filename list
            if (dto.UpdatedImageFileNames != null)
            {
                var oldFileNames = variant.Images.Select(i => i.FileName).ToList();
                var removedFileNames = oldFileNames.Except(dto.UpdatedImageFileNames).ToList();

                foreach (var fileName in removedFileNames)
                    await DeleteFromBlobAsync(fileName);

                _context.ProductVariantImagesNew.RemoveRange(variant.Images);

                int order = 1;
                foreach (var fileName in dto.UpdatedImageFileNames)
                {
                    _context.ProductVariantImagesNew.Add(new ProductVariantImageNew
                    {
                        SkuId = variant.SkuId,
                        FileName = fileName,
                        SortOrder = order++,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return MapVariantToOld(variant, null, null);
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
                await blobClient.UploadAsync(stream, overwrite: true);

            return newFileName;
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

        public async Task DeleteProductAsync(int productId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.ProductsNew
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Attributes)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null) return;

                var variantIds = product.Variants.Select(v => v.SkuId).ToList();

                var images = await _context.ProductVariantImagesNew
                    .Where(i => variantIds.Contains(i.SkuId))
                    .ToListAsync();

                foreach (var img in images)
                    await DeleteFromBlobAsync(img.FileName);
                _context.ProductVariantImagesNew.RemoveRange(images);

                var attrs = product.Variants.SelectMany(v => v.Attributes).ToList();
                _context.ProductVariantAttributesNew.RemoveRange(attrs);

                _context.ProductVariantsNew.RemoveRange(product.Variants);
                _context.ProductsNew.Remove(product);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteProductVariantAsync(int productId, int skuId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var variant = await _context.ProductVariantsNew
                    .Include(v => v.Attributes)
                    .Include(v => v.Images)
                    .FirstOrDefaultAsync(v => v.ProductId == productId && v.SkuId == skuId);

                if (variant == null) return;

                foreach (var img in variant.Images)
                    await DeleteFromBlobAsync(img.FileName);
                _context.ProductVariantImagesNew.RemoveRange(variant.Images);

                _context.ProductVariantAttributesNew.RemoveRange(variant.Attributes);
                _context.ProductVariantsNew.Remove(variant);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ProdVariantdetailsDto>> GetAllProductsAsync(int busRegId)
        {
            var products = await _context.ProductsNew
                .Where(p => p.BusRegId == busRegId)
                .Include(p => p.BusinessRegister)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .ToListAsync();

            return products.Select(MapProductToDto).ToList();
        }

        // ---------------- Shopper-facing queries: still pending attribute-name confirmation ----------------

        public Task<IEnumerable<ProdcVariantforShopperDto>> GetDiscountedProductsAsync()
            => throw new NotImplementedException("Pending attribute-name confirmation");

        public Task<IEnumerable<ProdcVariantforShopperDto>> GetProductsBySubCategoryAsync(int subCategoryId)
            => throw new NotImplementedException("Pending attribute-name confirmation");

        public Task<IEnumerable<ProdcVariantforShopperDto>> GetTopPurchasedProductsByLocation(string location, int minOrders = 5)
            => throw new NotImplementedException("Pending attribute-name confirmation");

        public async Task SaveProductViewAsync(int shopperId, int productId)
        {
            _context.ShopperProductRecentViews.Add(new ShopperProductRecentView
            {
                ShopperId = shopperId,
                ProductId = productId,
                LastViewedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        // ---------------- Helpers ----------------

        private async Task UpsertColorSizeAttributesAsync(
            ProductVariantNew variant, string? color, int? sizeId, long? prodSubcatId, long? busCatId)
        {
            int? prodSubcatIdInt = prodSubcatId.HasValue ? (int)prodSubcatId.Value : (int?)null;
            int? busCatIdInt = busCatId.HasValue ? (int)busCatId.Value : (int?)null;

            if (!string.IsNullOrWhiteSpace(color))
            {
                var colorAttr = await _context.ProductAttributes.FirstOrDefaultAsync(a =>
                    a.AttributeName.ToLower() == "color" &&
                    a.ProdSubcatId == prodSubcatIdInt && a.BusCatId == busCatIdInt);

                if (colorAttr != null)
                {
                    var row = await _context.ProductVariantAttributesNew.FirstOrDefaultAsync(a =>
                        a.SkuId == variant.SkuId && a.AttributeId == colorAttr.AttributeId);

                    if (row == null)
                        _context.ProductVariantAttributesNew.Add(new ProductVariantAttributeNew
                        {
                            SkuId = variant.SkuId,
                            AttributeId = colorAttr.AttributeId,
                            AttributeValue = color,
                            CreatedAt = DateTime.UtcNow
                        });
                    else
                        row.AttributeValue = color;
                }
            }

            if (sizeId.HasValue)
            {
                var sizeAttr = await _context.ProductAttributes.FirstOrDefaultAsync(a =>
                    a.AttributeName.ToLower() == "size" &&
                    a.ProdSubcatId == prodSubcatIdInt && a.BusCatId == busCatIdInt);

                if (sizeAttr != null)
                {
                    var row = await _context.ProductVariantAttributesNew.FirstOrDefaultAsync(a =>
                        a.SkuId == variant.SkuId && a.AttributeId == sizeAttr.AttributeId);

                    if (row == null)
                        _context.ProductVariantAttributesNew.Add(new ProductVariantAttributeNew
                        {
                            SkuId = variant.SkuId,
                            AttributeId = sizeAttr.AttributeId,
                            AttributeValueId = sizeId.Value,
                            CreatedAt = DateTime.UtcNow
                        });
                    else
                        row.AttributeValueId = sizeId.Value;
                }
            }

            await _context.SaveChangesAsync();
        }

        private Sku_ProductVariant MapVariantToOld(ProductVariantNew v, string? color, int? sizeId)
        {
            return new Sku_ProductVariant
            {
                SkuId = (int)v.SkuId,
                ProductId = (int)v.ProductId,
                Color = color,
                SizeId = sizeId,
                Sku_Cost = v.Price,
                DiscountPrice = v.DiscountPrice,
                Quantity = v.StockQuantity,
                Weight = v.Weight,
                Discount = v.Discount
            };
        }

        private ProdVariantdetailsDto MapProductToDto(ProductsNew p)
        {
            var biz = p.BusinessRegister;
            var location = biz != null
                ? string.Join(", ", new[] { biz.Town, biz.BusinessCity, biz.BusinessState }
                    .Where(s => !string.IsNullOrWhiteSpace(s)))
                : "";

            return new ProdVariantdetailsDto
            {
                ProductId = (int)p.ProductId,
                BusRegId = p.BusRegId,
                BuscatId = (int)(p.BusCatId ?? 0),
                ProdSubcatId = (int)(p.ProdSubcatId ?? 0),
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                SupplierName = biz?.BusinessName,
                IsProductAvailable = p.ProductStatus == "Approved" && p.IsActive,
                Location = location,
                Country = biz?.BusinessCountry ?? "",

                Variants = p.Variants.Select(v => new Sku_ProductVariantDto
                {
                    SkuId_Productvariant = (int)v.SkuId,
                    ProductId = (int)v.ProductId,
                    Sku_Cost = v.Price,
                    DiscountPrice = v.DiscountPrice,
                    Quantity = v.StockQuantity,
                    Weight = v.Weight,
                    Discount = v.Discount,
                    Images = v.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto { FileName = i.FileName, SortOrder = i.SortOrder })
                        .ToList(),
                    Attributes = v.Attributes
                        .Select(a => new VariantAttributeDto
                        {
                            AttributeId = a.AttributeId,
                            AttributeValueId = a.AttributeValueId,
                            AttributeValue = a.AttributeValue
                        })
                        .ToList()
                }).ToList()
            };
        }
    }
}