using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Repositories.Interfaces
{
    public interface IProductsNewRepository
    {
        Task BeginTransactionAsync();

        void AddProduct(ProductsNew product);

        void AddVariant(ProductVariantNew variant);

        void AddVariantImage(ProductVariantImageNew image);

        void AddVariantAttribute(
            ProductVariantAttributeNew variantAttribute);

        Task SaveChangesAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();

        Task<ProductMasterNamesDto> GetProductMasterNamesByBusinessAsync(int busRegId);
    }
}