using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        // ---------------- Product Main ----------------

        public async Task<Products> AddProductAsync(ProductCreateDto dto)
        {
            if (dto.BusRegId <= 0)
                throw new Exception("Invalid Business Registration Id.");

            //// Optional but recommended
            //var businessExists = await _repo.BusinessExistsAsync(dto.BusRegId);
            //if (!businessExists)
            //    throw new Exception("Business does not exist.");
            var entity = new Products
            {
                BusRegId = dto.BusRegId,
                BuscatId = dto.BuscatId,
                ProdSubcatId = dto.ProdSubcatId,
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                ProductTypeId = dto.ProductTypeId,
                FabricId = dto.FabricId == 0 ? null : dto.FabricId,
                DesignId = dto.DesignId == 0 ? null : dto.DesignId,
                SupplierName = dto.SupplierName
            };

            return await _repo.AddProductAsync(entity);
        }

        public async Task<Products?> UpdateProductAsync(ProductCreateDto dto)
        {
            if (dto.ProductId == 0)
                return null;

            return await _repo.UpdateProductAsync(dto.ProductId, dto);
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _repo.DeleteProductAsync(productId);
        }

        // ---------------- Variants ----------------

        public async Task<Sku_ProductVariant> AddProductVariantAsync(Sku_CreateVariantDto dto)
        {
            return await _repo.AddProductVariantAsync(dto);
        }

        public async Task<Sku_ProductVariant?> UpdateVariantAsync(Sku_ProductVariantDto dto)
        {
            return await _repo.UpdateVariantAsync(dto);
        }

        public async Task DeleteVariantAsync(int productId, int skuId)
        {
            await _repo.DeleteProductVariantAsync(productId, skuId);
        }

        // ---------------- Fetch ----------------

        public async Task<ProdVariantdetailsDto?> GetProductAndVariantAsync(int productId)
        {
            return await _repo.GetProductandVariantAsync(productId);
        }

        public async Task<ProductSizeMeasurementDto?> GetMeasurementBySizeIdAsync(int sizeId)
        {
            return await _repo.GetMeasurementBySizeIdAsync(sizeId);
        }

        public async Task<IEnumerable<ProdVariantdetailsDto>> GetAllProductsAsync(int busRegId)
        {
            return await _repo.GetAllProductsAsync(busRegId);
        }

        // ---------------- Shopper / Public ----------------

        public async Task<IEnumerable<ProdcVariantforShopperDto>> GetDiscountedProductsAsync()
        {
            return await _repo.GetDiscountedProductsAsync();
        }

        public async Task<IEnumerable<ProdcVariantforShopperDto>> GetProductsBySubCategoryAsync(int subCategoryId)
        {
            return await _repo.GetProductsBySubCategoryAsync(subCategoryId);
        }

        public async Task SaveProductViewAsync(int shopperId, int productId)
        {
            await _repo.SaveProductViewAsync(shopperId, productId);
        }

        public async Task<IEnumerable<ProdcVariantforShopperDto>> GetTopPurchasedProductsByLocation(
            string location,
            int minOrders)
        {
            return await _repo.GetTopPurchasedProductsByLocation(location, minOrders);
        }
    }
}
