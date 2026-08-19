using Azure.Storage.Blobs;
using mytown.DTOs.ProductsNew;
using mytown.Models;
using mytown.Repositories.Interfaces;
using mytown.Services.Interfaces;

public class ProductsNewService : IProductsNewService
{
    private readonly IProductsNewRepository _repository;
    private readonly IConfiguration _configuration;

    public ProductsNewService(IProductsNewRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<long> CreateProductAsync(CreateProductNewRequest request)
    {
        await _repository.BeginTransactionAsync();
        try
        {
            var product = new ProductsNew
            {
                BusCatId = request.BusCatId,
                ProdSubcatId = request.ProdSubcatId,
                ProductGroupId = request.ProductGroupId,
                ProdTypeId = request.ProdTypeId,
                ProductName = request.ProductName,
                ProductDescription = request.ProductDescription,
                ProductStatus = request.ProductStatus,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _repository.AddProduct(product);
            await _repository.SaveChangesAsync();

            foreach (var variantRequest in request.Variants)
            {
                var variant = new ProductVariantNew
                {
                    Product = product,
                    StockQuantity = variantRequest.StockQuantity,
                    Weight = variantRequest.Weight,
                    MeasurementUnit = variantRequest.MeasurementUnit,
                    Price = variantRequest.Price,
                    Discount = variantRequest.Discount,
                    DiscountPrice = variantRequest.DiscountPrice,
                    Brand = variantRequest.Brand,
                    IsActive = variantRequest.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _repository.AddVariant(variant);
                await _repository.SaveChangesAsync();

                if (variantRequest.Images != null && variantRequest.Images.Any())
                {
                    int order = 1;
                    foreach (var file in variantRequest.Images)
                    {
                        var fileName = await UploadToBlobAsync(file, "product");
                        _repository.AddVariantImage(new ProductVariantImageNew
                        {
                            SkuId = variant.SkuId,
                            FileName = fileName,
                            SortOrder = order++,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (variantRequest.Attributes != null && variantRequest.Attributes.Any())
                {
                    foreach (var attributeRequest in variantRequest.Attributes)
                    {
                        _repository.AddVariantAttribute(new ProductVariantAttributeNew
                        {
                            Variant = variant,
                            AttributeId = attributeRequest.AttributeId,
                            AttributeValueId = attributeRequest.AttributeValueId,
                            AttributeValue = attributeRequest.AttributeValue,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await _repository.SaveChangesAsync();
            await _repository.CommitTransactionAsync();
            return product.ProductId;
        }
        catch
        {
            await _repository.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<string> UploadToBlobAsync(IFormFile file, string imageType)
    {
        var containerName = _configuration["AzureBlobStorage:ContainerName"];
        var connectionString = _configuration["AzureBlobStorage:ConnectionString"];
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        var fileExtension = Path.GetExtension(file.FileName);
        var newFileName = $"{imageType}_{fileNameWithoutExtension}_{timestamp}{fileExtension}";
        var blobClient = containerClient.GetBlobClient(newFileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        return newFileName;
    }

    public async Task DeleteFromBlobAsync(string fileName)
    {
        var containerName = _configuration["AzureBlobStorage:ContainerName"];
        var connectionString = _configuration["AzureBlobStorage:ConnectionString"];
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.GetBlobClient(fileName).DeleteIfExistsAsync();
    }
}