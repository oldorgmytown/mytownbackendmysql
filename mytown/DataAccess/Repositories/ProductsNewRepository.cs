using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using mytown.Models;
using mytown.Repositories.Interfaces;
using mytown.Models.mytown.DataAccess;


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
    }
}