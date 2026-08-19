using mytown.Models;

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
    }
}