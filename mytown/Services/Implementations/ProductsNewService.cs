using mytown.DTOs.ProductsNew;
using mytown.Models;
using mytown.Repositories.Interfaces;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class ProductsNewService : IProductsNewService
    {
        private readonly IProductsNewRepository _repository;

        public ProductsNewService(
            IProductsNewRepository repository)
        {
            _repository = repository;
        }


        // =========================================
        // CREATE PRODUCT + VARIANTS + ATTRIBUTES
        // =========================================

        public async Task<long> CreateProductAsync(
            CreateProductNewRequest request)
        {
            await _repository.BeginTransactionAsync();

            try
            {
                // =====================================
                // 1. CREATE MAIN PRODUCT
                // =====================================

                var product = new ProductsNew
                {
                    BusCatId = request.BusCatId,

                    ProdSubcatId = request.ProdSubcatId,

                    ProductGroupId = request.ProductGroupId,

                    ProdTypeId = request.ProdTypeId,

                    ProductName = request.ProductName,

                    ProductDescription =
                        request.ProductDescription,

                    ProductStatus =
                        request.ProductStatus,

                    IsActive =
                        request.IsActive,

                    CreatedAt = DateTime.UtcNow,

                    UpdatedAt = DateTime.UtcNow
                };


                _repository.AddProduct(product);


                // =====================================
                // 2. CREATE ALL VARIANTS
                // =====================================

                foreach (var variantRequest
                         in request.Variants)
                {
                    var variant = new ProductVariantNew
                    {
                        // IMPORTANT:
                        // Connect variant to product
                        Product = product,

                        StockQuantity =
                            variantRequest.StockQuantity,

                        Weight =
                            variantRequest.Weight,

                        MeasurementUnit =
                            variantRequest.MeasurementUnit,

                        Price =
                            variantRequest.Price,

                        Discount =
                            variantRequest.Discount,

                        DiscountPrice =
                            variantRequest.DiscountPrice,

                        Brand =
                            variantRequest.Brand,

                        IsActive =
                            variantRequest.IsActive,

                        CreatedAt =
                            DateTime.UtcNow,

                        UpdatedAt =
                            DateTime.UtcNow
                    };


                    _repository.AddVariant(variant);


                    // =================================
                    // 3. CREATE VARIANT ATTRIBUTES
                    // =================================

                    foreach (
                        var attributeRequest
                        in variantRequest.Attributes)
                    {
                        var attribute =
                             new ProductVariantAttributeNew
                             {
                                 Variant = variant,

                                 AttributeId =
                                     attributeRequest.AttributeId,

                                 AttributeValueId =
                                     attributeRequest.AttributeValueId,

                                 AttributeValue =
                                     attributeRequest.AttributeValue,

                                 CreatedAt =
                                     DateTime.UtcNow
                             };


                        _repository.AddVariantAttribute(
                            attribute);
                    }
                }


                // =====================================
                // 4. SAVE EVERYTHING ONCE
                // =====================================

                await _repository.SaveChangesAsync();


                // =====================================
                // 5. COMMIT EVERYTHING
                // =====================================

                await _repository.CommitTransactionAsync();


                // Database generated ProductId
                return product.ProductId;
            }

            catch (Exception ex)
            {
                Console.WriteLine(
                    "SERVICE ERROR:");

                Console.WriteLine(
                    ex.ToString());

                await _repository.RollbackTransactionAsync();

                throw;
            }


        }
        }
    }
