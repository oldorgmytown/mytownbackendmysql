using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Repositories.Interfaces;


namespace mytown.Repositories
{
    public class ProductsNewRepository : IProductsNewRepository
    {
        private readonly AppDbContext _context;

        private IDbContextTransaction? _transaction;

        public ProductsNewRepository(AppDbContext context)
        {
            _context = context;
        }


        // -----------------------------------------
        // BEGIN TRANSACTION
        // -----------------------------------------

        public async Task BeginTransactionAsync()
        {
            _transaction =
                await _context.Database.BeginTransactionAsync();
        }


        // -----------------------------------------
        // ADD PRODUCT
        // -----------------------------------------

        public void AddProduct(ProductsNew product)
        {
            _context.ProductsNew.Add(product);
        }


        // -----------------------------------------
        // ADD VARIANT
        // -----------------------------------------

        public void AddVariant(ProductVariantNew variant)
        {
            _context.ProductVariantsNew.Add(variant);
        }


        // -----------------------------------------
        // ADD VARIANT ATTRIBUTE
        // -----------------------------------------

        public void AddVariantAttribute(
            ProductVariantAttributeNew variantAttribute)
        {
            _context.ProductVariantAttributesNew.Add(
                variantAttribute);
        }


        public void AddVariantImage(ProductVariantImageNew image)
        {
            _context.ProductVariantImagesNew.Add(image);
        }
        // -----------------------------------------
        // SAVE EVERYTHING
        // -----------------------------------------

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        // -----------------------------------------
        // COMMIT
        // -----------------------------------------

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }


        // -----------------------------------------
        // ROLLBACK
        // -----------------------------------------

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }

        //Filter dropdown for product categories, subcategories and product types

      public async Task<ProductMasterNamesDto> GetProductMasterNamesByBusinessAsync(
    int busRegId)
{
    // ---------------------------------------------------------
    // Get products belonging to this business
    // ---------------------------------------------------------

    var products = await _context.ProductsNew
        .AsNoTracking()
        .Where(p =>
            p.BusRegId == busRegId &&
            p.IsActive)
        .Select(p => new
        {
            p.ProductId,
            p.ProdSubcatId,
            p.ProductGroupId,
            p.ProdTypeId
        })
        .ToListAsync();


    // ---------------------------------------------------------
    // Extract IDs
    // ---------------------------------------------------------

    var subCategoryIds = products
        .Where(x => x.ProdSubcatId.HasValue)
        .Select(x => x.ProdSubcatId!.Value)
        .Distinct()
        .ToList();

    var groupIds = products
        .Where(x => x.ProductGroupId.HasValue)
        .Select(x => x.ProductGroupId!.Value)
        .Distinct()
        .ToList();

    var typeIds = products
        .Where(x => x.ProdTypeId.HasValue)
        .Select(x => x.ProdTypeId!.Value)
        .Distinct()
        .ToList();

    var productIds = products
        .Select(x => x.ProductId)
        .ToList();


    // ---------------------------------------------------------
    // Product SubCategories
    // ---------------------------------------------------------

    var subCategories = await _context.product_sub_categories
        .AsNoTracking()
        .Where(sc =>
            subCategoryIds.Contains(
                (long)sc.ProdSubcatId))
        .Select(sc => new ProductSubCategoryDto
        {
            ProdSubcatId = sc.ProdSubcatId,
            ProdSubCatName = sc.ProdSubcatName
        })
        .OrderBy(sc => sc.ProdSubCatName)
        .ToListAsync();


    // ---------------------------------------------------------
    // Product Groups
    // ---------------------------------------------------------

    var groups = await _context.Product_Groups
        .AsNoTracking()
        .Where(g =>
            groupIds.Contains(
                (long)g.ProdGroupId))
        .Select(g => new ProductGroupDto
        {
            ProductGroupId = g.ProdGroupId,
            ProductGroupName = g.ProdGroupName
        })
        .OrderBy(g => g.ProductGroupName)
        .ToListAsync();


    // ---------------------------------------------------------
    // Product Types
    // ---------------------------------------------------------

    var types = await _context.Product_Types
        .AsNoTracking()
        .Where(t =>
            typeIds.Contains(
                (long)t.ProdTypeId))
        .Select(t => new ProductTypeDto
        {
            ProdTypeId = t.ProdTypeId,
            ProductTypeName = t.ProdTypeName
        })
        .OrderBy(t => t.ProductTypeName)
        .ToListAsync();


    // ---------------------------------------------------------
    // Get SKUs belonging to these products
    // ---------------------------------------------------------

    var skuIds = await _context.ProductVariantsNew
        .AsNoTracking()
        .Where(v =>
            productIds.Contains(v.ProductId) &&
            v.IsActive)
        .Select(v => v.SkuId)
        .ToListAsync();


    // ---------------------------------------------------------
    // Get attributes used by these SKUs
    // ---------------------------------------------------------

    var variantAttributes = await _context.ProductVariantAttributesNew
        .AsNoTracking()
        .Where(va =>
            skuIds.Contains(va.SkuId))
        .Select(va => new
        {
            va.AttributeId,
            va.AttributeValueId,
            va.AttributeValue
        })
        .ToListAsync();


    // ---------------------------------------------------------
    // Attribute IDs
    // ---------------------------------------------------------

    var attributeIds = variantAttributes
        .Select(x => x.AttributeId)
        .Distinct()
        .ToList();


    // ---------------------------------------------------------
    // Attribute names
    // ---------------------------------------------------------

    var attributes = await _context.ProductAttributes
        .AsNoTracking()
        .Where(a =>
            attributeIds.Contains(a.AttributeId))
        .Select(a => new
        {
            a.AttributeId,
            a.AttributeName
        })
        .ToListAsync();


    // ---------------------------------------------------------
    // Attribute Value IDs
    // ---------------------------------------------------------

    var attributeValueIds = variantAttributes
        .Where(x => x.AttributeValueId.HasValue)
        .Select(x => x.AttributeValueId!.Value)
        .Distinct()
        .ToList();


    // ---------------------------------------------------------
    // Attribute master values
    // ---------------------------------------------------------

    var attributeValues = await _context.ProductAttributeValues
        .AsNoTracking()
        .Where(av =>
            attributeValueIds.Contains(av.AttributeValueId))
        .Select(av => new
        {
            av.AttributeValueId,
            av.AttributeId,
            av.AttributeValue
        })
        .ToListAsync();


    // ---------------------------------------------------------
    // Build attribute response
    // ---------------------------------------------------------

    var attributeDtos = attributes
        .Select(attribute => new ProductAttributeMasterDto
        {
            AttributeId = attribute.AttributeId,

            AttributeName = attribute.AttributeName,

            Values = variantAttributes
                .Where(va =>
                    va.AttributeId == attribute.AttributeId)
                .Select(va =>
                {
                    // If the variant already has a stored value,
                    // use it. Otherwise get it from master table.
                    var value = va.AttributeValue;

                    if (string.IsNullOrWhiteSpace(value) &&
                        va.AttributeValueId.HasValue)
                    {
                        value = attributeValues
                            .Where(av =>
                                av.AttributeValueId ==
                                va.AttributeValueId.Value)
                            .Select(av => av.AttributeValue)
                            .FirstOrDefault();
                    }

                    return new ProductAttributeValueDto
                    {
                        AttributeValueId =
                            va.AttributeValueId ?? 0,

                        AttributeValue =
                            value ?? string.Empty
                    };
                })
                .Where(v =>
                    !string.IsNullOrWhiteSpace(
                        v.AttributeValue))
                .GroupBy(v => new
                {
                    v.AttributeValueId,
                    v.AttributeValue
                })
                .Select(g => g.First())
                .OrderBy(v => v.AttributeValue)
                .ToList()
        })
        .Where(a => a.Values.Any())
        .OrderBy(a => a.AttributeName)
        .ToList();


    // ---------------------------------------------------------
    // Final response
    // ---------------------------------------------------------

    return new ProductMasterNamesDto
    {
        ProductSubCategories = subCategories,

        ProductGroups = groups,

        ProductTypes = types,

        Attributes = attributeDtos
    };
}

        public async Task<List<ProductSearchResultDto>> SearchProductsAsync(
       ProductSearchRequestDto request)
        {
            var search = request.Search?.Trim();

            if (string.IsNullOrWhiteSpace(search))
                return new List<ProductSearchResultDto>();

            var pattern = $"%{search}%";

            // ---------------------------------------------------------
            // STEP 1: Find matching products
            // ---------------------------------------------------------

            var productIdsQuery = _context.ProductsNew
                .AsNoTracking()
                .Where(p =>
                    p.BusRegId == request.BusRegId &&
                    p.IsActive &&
                    p.ProductStatus == "ACTIVE"
                )
                .Where(p =>
                    // Product name
                    EF.Functions.Like(
                        p.ProductName,
                        pattern
                    )

                    ||

                    // Product description
                    EF.Functions.Like(
                        p.ProductDescription ?? "",
                        pattern
                    )

                    ||

                    // Brand
                    _context.ProductVariantsNew.Any(v =>
                        v.ProductId == p.ProductId &&
                        v.IsActive &&
                        v.Brand != null &&
                        EF.Functions.Like(
                            v.Brand,
                            pattern
                        )
                    )

                    ||

                    // Measurement unit
                    _context.ProductVariantsNew.Any(v =>
                        v.ProductId == p.ProductId &&
                        v.IsActive &&
                        v.MeasurementUnit != null &&
                        EF.Functions.Like(
                            v.MeasurementUnit,
                            pattern
                        )
                    )

                    ||

                    // Variant attribute stored value
                    _context.ProductVariantAttributesNew.Any(va =>
                        _context.ProductVariantsNew.Any(v =>
                            v.ProductId == p.ProductId &&
                            v.SkuId == va.SkuId &&
                            v.IsActive
                        )
                        &&
                        EF.Functions.Like(
                            va.AttributeValue ?? "",
                            pattern
                        )
                    )

                    ||

                    // Attribute name
                    _context.ProductVariantAttributesNew.Any(va =>
                        _context.ProductVariantsNew.Any(v =>
                            v.ProductId == p.ProductId &&
                            v.SkuId == va.SkuId &&
                            v.IsActive
                        )
                        &&
                        _context.ProductAttributes.Any(a =>
                            a.AttributeId == va.AttributeId &&
                            EF.Functions.Like(
                                a.AttributeName,
                                pattern
                            )
                        )
                    )

                    ||

                    // Attribute master value
                    _context.ProductVariantAttributesNew.Any(va =>
                        _context.ProductVariantsNew.Any(v =>
                            v.ProductId == p.ProductId &&
                            v.SkuId == va.SkuId &&
                            v.IsActive
                        )
                        &&
                        va.AttributeValueId.HasValue &&
                        _context.ProductAttributeValues.Any(av =>
                            av.AttributeValueId == va.AttributeValueId.Value &&
                            EF.Functions.Like(
                                av.AttributeValue,
                                pattern
                            )
                        )
                    )

                    ||

                    // Subcategory
                    _context.product_sub_categories.Any(sc =>
                        p.ProdSubcatId.HasValue &&
                        sc.ProdSubcatId == p.ProdSubcatId.Value &&
                        EF.Functions.Like(
                            sc.ProdSubcatName,
                            pattern
                        )
                    )

                    ||

                    // Product group
                    _context.Product_Groups.Any(pg =>
                        p.ProductGroupId.HasValue &&
                        pg.ProdGroupId == p.ProductGroupId.Value &&
                        EF.Functions.Like(
                            pg.ProdGroupName,
                            pattern
                        )
                    )

                    ||

                    // Product type
                    _context.Product_Types.Any(pt =>
                        p.ProdTypeId.HasValue &&
                        pt.ProdTypeId == p.ProdTypeId.Value &&
                        EF.Functions.Like(
                            pt.ProdTypeName,
                            pattern
                        )
                    )
                )
                .Select(p => p.ProductId);

            // ---------------------------------------------------------
            // STEP 2: Pagination
            // ---------------------------------------------------------

            var productIds = await productIdsQuery
                .OrderBy(id => id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            if (!productIds.Any())
                return new List<ProductSearchResultDto>();

            // ---------------------------------------------------------
            // STEP 3: Product + cheapest active variant
            // ---------------------------------------------------------

            var products = await _context.ProductsNew
                .AsNoTracking()
                .Where(p =>
                    productIds.Contains(p.ProductId) &&
                    p.BusRegId == request.BusRegId &&
                    p.IsActive &&
                    p.ProductStatus == "ACTIVE"
                )
                .Select(p => new
                {
                    Product = p,

                    Variant = _context.ProductVariantsNew
                        .Where(v =>
                            v.ProductId == p.ProductId &&
                            v.IsActive
                        )
                        .OrderBy(v => v.Price)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // ---------------------------------------------------------
            // STEP 4: Images
            // ---------------------------------------------------------

            var skuIds = products
                .Where(x => x.Variant != null)
                .Select(x => x.Variant!.SkuId)
                .ToList();

            var images = await _context.ProductVariantImagesNew
                .AsNoTracking()
                .Where(i => skuIds.Contains(i.SkuId))
                .GroupBy(i => i.SkuId)
                .Select(g => g
                    .OrderBy(i => i.ImageId)
                    .FirstOrDefault())
                .ToListAsync();

            var imageDictionary = images
                .Where(x => x != null)
                .ToDictionary(
                    x => x!.SkuId,
                    x => x!.FileName
                );

            // ---------------------------------------------------------
            // STEP 5: Response
            // ---------------------------------------------------------

            return products
                .Select(x =>
                {
                    var variant = x.Variant;

                    string? image = null;

                    if (variant != null &&
                        imageDictionary.TryGetValue(
                            variant.SkuId,
                            out var imageName))
                    {
                        image = imageName;
                    }

                    return new ProductSearchResultDto
                    {
                        ProductId = (int)x.Product.ProductId,

                        ProductName = x.Product.ProductName,

                        Price = variant?.Price,

                        Discount = variant?.Discount,

                        DiscountPrice = variant?.DiscountPrice,

                        Image = image
                    };
                })
                .ToList();
        }
    }
}