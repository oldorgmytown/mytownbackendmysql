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

        //latest code to add produicts main data and varinats with images
        public async Task<products> AddProductAsync(products product)
        {
            _context.products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<Sku_ProductVariant> AddProductVariantAsync(Sku_CreateVariantDto dto)
        {
            var variant = new Sku_ProductVariant
            {
                ProductId = dto.ProductId,
                Color = dto.Color,
                SizeId = dto.SizeId,
                Sku_Cost = dto.Sku_Cost ?? 0,
                DiscountPrice = dto.DiscountPrice,
                Quantity = dto.Quantity ?? 0,
                Length = dto.Length,
                Width = dto.Width,
                Height = dto.Height,
                Weight = dto.Weight,
                Discount = dto.Discount
            };

            await _context.Sku_ProductVariants.AddAsync(variant);
            await _context.SaveChangesAsync(); // ensures SkuId is generated

            if (dto.Images != null && dto.Images.Any())
            {
                int order = 1;

                foreach (var file in dto.Images)
                {
                    // Upload to blob storage
                    var fileName = await UploadToBlobAsync(file, "product");

                    var image = new ProductImage
                    {
                        ProductId = variant.ProductId,
                        SkuId = variant.SkuId,
                        FileName = fileName,
                        SortOrder = order++
                    };

                    await _context.ProductImages.AddAsync(image);
                }

                await _context.SaveChangesAsync();
            }

            return variant;
        }


        // get Size measurements on add product form

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
            var product = await _context.products
                .Include(p => p.Sku_ProductVariants)
                    .ThenInclude(v => v.Images)
                .Include(p => p.Sku_ProductVariants)
            .ThenInclude(v => v.Size) //get sizename
                .FirstOrDefaultAsync(p => p.product_id == productId);

            if (product == null) return null;

            return new ProdVariantdetailsDto
            {
                ProductId = product.product_id,
                BusRegId = product.BusRegId,
                BuscatId = product.BuscatId,
                ProdSubcatId = product.prod_subcat_id,
                ProductName = product.product_name,                
                ProductDescription = product.product_description,              
                SupplierName = product.supplier_name,
                ProductTypeId = product.ProductTypeId,
                FabricId = product.FabricId,
                DesignId = product.DesignId,



                Variants = product.Sku_ProductVariants.Select(v => new Sku_ProductVariantDto
                {
                    SkuId_Productvariant = v.SkuId,
                    ProductId = v.ProductId,
                    Color = v.Color,
                    SizeId = v.SizeId,
                    SizeName = v.Size != null ? v.Size.SizeName : null,
                    Sku_Cost = v.Sku_Cost,
                    DiscountPrice = v.DiscountPrice,
                    Quantity = v.Quantity,
                    Length = v.Length,
                    Width = v.Width,
                    Height = v.Height,
                    Weight = v.Weight,
                    Discount = v.Discount,
                    Images = v.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                }).ToList()
            };
        }




        //------------------------------------------------------------------------------------------------------------------------------------------------//
        //public async Task<products> CreateProductAsync(products product, List<IFormFile> imageFiles)
        //{
        //    await _context.products.AddAsync(product);
        //    await _context.SaveChangesAsync();

        //    if (imageFiles != null && imageFiles.Any())
        //    {
        //        int order = 1;
        //        foreach (var file in imageFiles)
        //        {
        //            var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        //            // Upload to Blob
        //            await UploadToBlobAsync(file, newFileName);

        //            // Save in DB
        //            var productImage = new ProductImage
        //            {
        //                ProductId = product.product_id,
        //                FileName = newFileName,
        //                SortOrder = order++
        //            };
        //            await _context.ProductImages.AddAsync(productImage);
        //        }
        //        await _context.SaveChangesAsync();
        //    }

        //    return product;
        //}


        //public async Task<products> UpdateProductAsync(products updatedProduct, List<IFormFile> imageFiles)
        //{
        //    var existingProduct = await _context.products
        //        .Include(p => p.Images)
        //        .FirstOrDefaultAsync(p => p.product_id == updatedProduct.product_id);

        //    if (existingProduct == null)
        //        return null;

        //    //  Update product fields
        //    existingProduct.product_name = updatedProduct.product_name;
        //    existingProduct.product_subject = updatedProduct.product_subject;
        //    existingProduct.product_description = updatedProduct.product_description;
        //    existingProduct.product_cost = updatedProduct.product_cost;
        //    existingProduct.product_length = updatedProduct.product_length;
        //    existingProduct.product_width = updatedProduct.product_width;
        //    existingProduct.product_weight = updatedProduct.product_weight;
        //    existingProduct.product_quantity = updatedProduct.product_quantity;
        //    existingProduct.product_height = updatedProduct.product_height;
        //    existingProduct.discount = updatedProduct.discount;
        //    existingProduct.discount_price = updatedProduct.discount_price;
        //    existingProduct.color = updatedProduct.color;
        //    existingProduct.size = updatedProduct.size;

        //    //  Remove old images (from Blob + DB)
        //    foreach (var oldImage in existingProduct.Images)
        //    {
        //        await DeleteFromBlobAsync(oldImage.FileName);
        //    }
        //    _context.ProductImages.RemoveRange(existingProduct.Images);

        //    //  Add new images
        //    if (imageFiles != null && imageFiles.Any())
        //    {
        //        int order = 1;
        //        foreach (var file in imageFiles)
        //        {
        //            var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        //            // Upload to Blob
        //            await UploadToBlobAsync(file, newFileName);

        //            // Save in DB
        //            var productImage = new ProductImage
        //            {
        //                ProductId = existingProduct.product_id,
        //                FileName = newFileName,
        //                SortOrder = order++
        //            };
        //            await _context.ProductImages.AddAsync(productImage);
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //    return existingProduct;
        //}


        //Update only product main details
        public async Task<products> UpdateProductAsync(int productId, ProductCreateDto dto)
{
    var existingProduct = await _context.products.FindAsync(productId);
    if (existingProduct == null) return null;

    // Only update if new value is not null
    if (dto.BusRegId != 0) existingProduct.BusRegId = dto.BusRegId;
    if (dto.BuscatId != 0) existingProduct.BuscatId = dto.BuscatId;
    if (dto.ProdSubcatId != 0) existingProduct.prod_subcat_id = dto.ProdSubcatId;

    if (!string.IsNullOrWhiteSpace(dto.ProductName))
        existingProduct.product_name = dto.ProductName;

    if (!string.IsNullOrWhiteSpace(dto.ProductDescription))
        existingProduct.product_description = dto.ProductDescription;

    if (!string.IsNullOrWhiteSpace(dto.SupplierName))
        existingProduct.supplier_name = dto.SupplierName;

    if (dto.ProductTypeId.HasValue)
        existingProduct.ProductTypeId = dto.ProductTypeId;

    if (dto.FabricId.HasValue)
        existingProduct.FabricId = dto.FabricId;

    if (dto.DesignId.HasValue)
        existingProduct.DesignId = dto.DesignId;

    _context.products.Update(existingProduct);
    await _context.SaveChangesAsync();

    return existingProduct;
}


        // Update Product Variant details

        public async Task<Sku_ProductVariant?> UpdateVariantAsync(Sku_ProductVariantDto dto, List<IFormFile> files)
        {
            var variant = await _context.Sku_ProductVariants
                .Include(v => v.Images)
                .FirstOrDefaultAsync(v => v.SkuId == dto.SkuId_Productvariant);

            if (variant == null)
                return null;

            // --- Update only if values were supplied ---
            if (!string.IsNullOrWhiteSpace(dto.Color))
                variant.Color = dto.Color;

            if (dto.SizeId.HasValue)
                variant.SizeId = dto.SizeId;

            if (dto.Sku_Cost.HasValue)
                variant.Sku_Cost = dto.Sku_Cost.Value;

            if (dto.Quantity.HasValue)
                variant.Quantity = dto.Quantity.Value;

            if (dto.DiscountPrice.HasValue)
                variant.DiscountPrice = dto.DiscountPrice.Value;

            if (dto.Length.HasValue) variant.Length = dto.Length.Value;
            if (dto.Width.HasValue) variant.Width = dto.Width.Value;
            if (dto.Height.HasValue) variant.Height = dto.Height.Value;
            if (dto.Weight.HasValue) variant.Weight = dto.Weight.Value;
            if (dto.Discount.HasValue) variant.Discount = dto.Discount.Value;

            // ----- Handle images -----
            if (files != null && files.Any())
            {
                // delete old images (db + blob)
                foreach (var img in variant.Images)
                    await DeleteFromBlobAsync(img.FileName);

                _context.ProductImages.RemoveRange(variant.Images);

                int order = 1;
                foreach (var file in files)
                {
                    var storedFileName = await UploadToBlobAsync(file, "product");

                    await _context.ProductImages.AddAsync(new ProductImage
                    {
                        ProductId = variant.ProductId,
                        SkuId = variant.SkuId,
                        FileName = storedFileName,
                        SortOrder = order++
                    });
                }
            }

            await _context.SaveChangesAsync();
            return variant;
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
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return newFileName; // store only file name in DB
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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //  Get product with variants and images
                var product = await _context.products
                    .Include(p => p.Sku_ProductVariants)
                        .ThenInclude(v => v.Images)
                    .FirstOrDefaultAsync(p => p.product_id == productId);

                if (product == null)
                    return;

                //  Delete product images from Blob
                foreach (var variant in product.Sku_ProductVariants)
                {
                    foreach (var img in variant.Images)
                    {
                        try
                        {
                            await DeleteFromBlobAsync(img.FileName); // ✅ reuse your method
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to delete blob {img.FileName}", ex);
                        }
                    }

                    _context.ProductImages.RemoveRange(variant.Images);
                }

                // Delete variants
                _context.Sku_ProductVariants.RemoveRange(product.Sku_ProductVariants);

                // 4️ Delete product itself
                _context.products.Remove(product);

                // 5️ Save and commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteProductVariantAsync(int productId, int skuId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Load variant with its images
                var variant = await _context.Sku_ProductVariants
                    .Include(v => v.Images)
                    .FirstOrDefaultAsync(v => v.ProductId == productId && v.SkuId == skuId);

                if (variant == null)
                    return;

                // 2️⃣ Delete images from blob + DB
                foreach (var img in variant.Images)
                {
                    try
                    {
                        await DeleteFromBlobAsync(img.FileName);                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to delete blob {img.FileName}", ex);
                    }
                }

                _context.ProductImages.RemoveRange(variant.Images);

                // 3️ Delete the variant itself
                _context.Sku_ProductVariants.Remove(variant);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        //public async Task<products> UpdateProductAsync(products product)
        //{
        //    _context.products.Update(product);
        //    await _context.SaveChangesAsync();
        //    return product;
        //}
        //edit or update product details 
        //public bool UpdateProduct(products updatedProduct)
        //{
        //    var existingProduct = _context.products.FirstOrDefault(p => p.product_id == updatedProduct.product_id);

        //    if (existingProduct == null)
        //        return false;

        //    // Update fields
        //    existingProduct.product_name = updatedProduct.product_name;
        //    existingProduct.product_subject = updatedProduct.product_subject;
        //    existingProduct.product_description = updatedProduct.product_description;
        //    existingProduct.product_image = updatedProduct.product_image;
        //    existingProduct.product_cost = updatedProduct.product_cost;
        //    existingProduct.product_length = updatedProduct.product_length;
        //    existingProduct.product_width = updatedProduct.product_width;
        //    existingProduct.product_weight = updatedProduct.product_weight;
        //    existingProduct.product_quantity = updatedProduct.product_quantity;

        //    _context.SaveChanges(); // Commit changes to the database
        //    return true;
        //}



        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _context.products
                .Include(p => p.Images)
                .Include(p => p.BusinessRegister)
                .FirstOrDefaultAsync(p => p.product_id == productId);

            if (product == null) return null;

            return new ProductDto
            {
                ProductId = product.product_id,
                BusRegId = product.BusRegId,
                BuscatId = product.BuscatId,
                ProductType = 0,
                ProdSubcatId = product.prod_subcat_id,
                ProductName = product.product_name,
                ProductSubject = product.product_subject,
                ProductDescription = product.product_description,
                ProductAmount = product.product_cost??0,
                ProductLength = product.product_length??0,
                ProductWidth = product.product_width ?? 0,
                ProductWeight = product.product_weight ?? 0,
                Quantity = product.product_quantity ?? 0,
                ProductHeight = product.product_height ?? 0,
                PurchasedCount = 0,
                Discount = product.discount,
                DiscountPrice = product.discount_price,
                Color = product.color,
                Size = product.size,
                BusinessName = product.BusinessRegister?.Businessname,
                Images = product.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new ProductImageDto
                    {
                        FileName = i.FileName,
                        SortOrder = i.SortOrder
                    })
                    .ToList()
            };
        }




        public async Task<IEnumerable<ProdVariantdetailsDto>> GetAllProductsAsync(int busRegId)
        {
            return await _context.products
        .Where(p => p.BusRegId == busRegId)
        .Include(p => p.Images) // product images
        .Include(p => p.Sku_ProductVariants)
            .ThenInclude(v => v.Images) // variant images
         .Include(p => p.Sku_ProductVariants)
            .ThenInclude(v => v.Size) //get sizename
        .Include(p => p.BusinessRegister) // business info
        .Select(p => new ProdVariantdetailsDto
        {
            ProductId = p.product_id,
            BusRegId = p.BusRegId,
            BuscatId = p.BuscatId,
            ProdSubcatId = p.prod_subcat_id,
            ProductName = p.product_name,
            ProductDescription = p.product_description,
            SupplierName = p.supplier_name,
            ProductTypeId = p.ProductTypeId,
            FabricId = p.FabricId,
            DesignId = p.DesignId,
            
            // Variants
            Variants = p.Sku_ProductVariants
                .Select(v => new Sku_ProductVariantDto
                {
                    SkuId_Productvariant = v.SkuId,
                    ProductId = v.ProductId,
                    Color = v.Color,
                    SizeId = v.SizeId,
                    SizeName = v.Size != null ? v.Size.SizeName : null,
                    Sku_Cost = v.Sku_Cost,
                    DiscountPrice = v.DiscountPrice,
                    Quantity = v.Quantity,
                    Length = v.Length,
                    Width = v.Width,
                    Height = v.Height,
                    Weight = v.Weight,
                    Discount = v.Discount,
                    Images = v.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                })
                .ToList()
        })
        .ToListAsync();
        }


        public async Task<IEnumerable<ProductDto>> GetDiscountedProductsAsync()
        {
            return await _context.products
                .Include(p => p.BusinessRegister)
                .Include(p => p.Images) // include related product images
                .Where(p => p.discount != null) // filter only products with discount
                .Select(p => new ProductDto
                {
                    ProductId = p.product_id,
                    BusRegId = p.BusRegId,
                    BuscatId = p.BuscatId,
                    ProdSubcatId = p.prod_subcat_id,
                    ProductName = p.product_name ?? string.Empty,
                    ProductSubject = p.product_subject ?? string.Empty,
                    ProductDescription = p.product_description ?? string.Empty,
                    ProductImage = p.product_image ?? string.Empty,
                    ProductAmount = p.product_cost ?? 0,
                    ProductLength = p.product_length ?? 0,
                    ProductWidth = p.product_width ?? 0,
                    ProductWeight = p.product_weight ?? 0,
                    Quantity = p.product_quantity ?? 0,
                    ProductHeight = p.product_height ?? 0,
                    Color = p.color ?? string.Empty,
                    Size = p.size ?? string.Empty,

                    Discount = p.discount,
                    DiscountPrice = p.discount_price,

                    BusinessName = p.BusinessRegister.Businessname ?? string.Empty,

                    Images = p.Images
                        .OrderBy(i => i.SortOrder) // keep consistent order
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName ?? string.Empty,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                })
                .ToListAsync();
        }



        public async Task<IEnumerable<ProductDto>> GetProductsBySubCategoryAsync(int subCategoryId)
        {
            var products = await _context.products
                .Where(p => p.prod_subcat_id == subCategoryId)
                .Include(p => p.BusinessRegister)
                .Include(p => p.Images)
                .Select(p => new ProductDto
                {
                    ProductId = p.product_id,
                    BusRegId = p.BusRegId,
                    BuscatId = p.BuscatId,
                    ProdSubcatId = p.prod_subcat_id,
                    ProductName = p.product_name ?? string.Empty,
                    ProductSubject = p.product_subject ?? string.Empty,
                    ProductDescription = p.product_description ?? string.Empty,
                    ProductAmount = p.product_cost ?? 0,
                    ProductLength = p.product_length ?? 0,
                    ProductWidth = p.product_width ?? 0,
                    ProductWeight = p.product_weight ?? 0,
                    Quantity = p.product_quantity ?? 0,
                    ProductHeight = p.product_height ?? 0,
                    Size = p.size ?? string.Empty,
                    Color = p.color ?? string.Empty,
                    Discount = p.discount,
                    DiscountPrice = p.discount_price,
                    BusinessName = p.BusinessRegister.Businessname ?? string.Empty,

                    // optional fields
                    PurchasedCount = 0,

                    // Map product images
                    Images = p.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            FileName = i.FileName ?? string.Empty,
                            SortOrder = i.SortOrder
                        })
                        .ToList()
                })
                .ToListAsync();

            return products;
        }


        // save shopper recently viewd products
        public async Task SaveProductViewAsync(int shopperId, int productId)
        {
            var view = new ShopperProductRecentView
            {
                ShopperId = shopperId,
                ProductId = productId,
                LastViewedAt = DateTime.UtcNow
            };

            _context.ShopperProductRecentViews.Add(view);
            await _context.SaveChangesAsync();
        }

        //public async Task<IEnumerable<ProductDto>> GetTopPurchasedProductsByTownAsync(string town, int limit = 10)
        //{
        //    var products = await _context.products
        //        .Where(p => p.BusinessRegister.Town == town) // join products → BusinessRegister (Town filter)
        //        .OrderByDescending(p => p.PurchasedCount)    // most purchased first
        //        .Take(limit)
        //        .Include(p => p.Images)                      // include product images
        //        .Select(p => new ProductDto
        //        {
        //            ProductId = p.product_id,
        //            BusRegId = p.BusRegId,
        //            BuscatId = p.BuscatId,
        //            ProdSubcatId = p.prod_subcat_id,
        //            ProductName = p.product_name,
        //            ProductSubject = p.product_subject,
        //            ProductDescription = p.product_description,
        //            ProductAmount = p.product_cost,
        //            Discount = p.discount,
        //            DiscountPrice = p.discount_price,
        //            Color = p.color,
        //            Size = p.size,
        //            PurchasedCount = p.PurchasedCount,
        //            BusinessName = p.BusinessRegister.Businessname,
        //            Images = p.Images.Select(i => new ProductImageDto
        //            {
        //                FileName = i.FileName,
        //                SortOrder = i.SortOrder
        //            }).ToList()
        //        })
        //        .ToListAsync();

        //    return products;
        //}
        public List<ProductDto> GetTopPurchasedProductsByLocation(string location, int minOrders = 5)
        {
            if (string.IsNullOrEmpty(location))
                return new List<ProductDto>();

            var query =
                from bp in _context.BusinessProfiles
                where bp.business_location != null && bp.business_location.Contains(location)
                join p in _context.products on bp.BusRegId equals p.BusRegId
                join o in _context.OrderDetails on p.product_id equals o.ProductId into productOrders
                select new
                {
                    Product = p,
                    StoreId = bp.BusRegId,
                    StoreName = bp.BusinessUsername ?? string.Empty,
                    TotalOrders = productOrders.Sum(po => (int?)po.Quantity) ?? 0
                };

            var result = query
                .Where(x => x.TotalOrders > minOrders)
                .OrderByDescending(x => x.TotalOrders)
                .Select(x => new ProductDto
                {
                    ProductId = x.Product.product_id,
                    BusRegId = x.StoreId,
                    StoreName = x.StoreName,
                    BuscatId = x.Product.BuscatId,
                    ProdSubcatId = x.Product.prod_subcat_id,
                    ProductName = x.Product.product_name,
                    ProductSubject = x.Product.product_subject,
                    ProductDescription = x.Product.product_description,
                    ProductAmount = x.Product.product_cost??0,
                    Discount = x.Product.discount,
                    DiscountPrice = x.Product.discount_price,
                    Color = x.Product.color,
                    PurchasedCount = x.TotalOrders,
                    Images = x.Product.Images != null
                        ? x.Product.Images
                            .OrderBy(i => i.SortOrder)
                            .Select(i => new ProductImageDto
                            {
                                FileName = i.FileName,
                                SortOrder = i.SortOrder
                            })
                            .ToList()
                        : new List<ProductImageDto>()
                })
                .ToList();

            return result ?? new List<ProductDto>();
        }


    }
}

