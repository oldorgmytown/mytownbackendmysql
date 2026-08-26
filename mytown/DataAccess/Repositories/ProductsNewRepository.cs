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


        public async Task<ProductMasterNamesDto> GetProductMasterNamesByBusinessAsync(int busRegId)
        {
            // 1. Get the IDs first and completely execute this query
            var productIds = await _context.ProductsNew
                .Where(p => p.BusRegId == busRegId && p.IsActive)
                .Select(p => new
                {
                    p.BusCatId,
                    p.ProductGroupId,
                    p.ProdTypeId
                })
                .ToListAsync();

            // 2. Extract IDs in memory
            var categoryIds = productIds
                .Where(x => x.BusCatId.HasValue)
                .Select(x => x.BusCatId!.Value)
                .Distinct()
                .ToList();

            var groupIds = productIds
                .Where(x => x.ProductGroupId.HasValue)
                .Select(x => x.ProductGroupId!.Value)
                .Distinct()
                .ToList();

            var typeIds = productIds
                .Where(x => x.ProdTypeId.HasValue)
                .Select(x => x.ProdTypeId!.Value)
                .Distinct()
                .ToList();

            // 3. Categories
            var categories = await _context.BusinessCategories
                .Where(c => categoryIds.Contains((long)c.BusCatId))
                .Select(c => new ProductCategoryDto
                {
                    BusCatId = c.BusCatId,
                    BusCatName = c.BusinessCategoryName
                })
                .OrderBy(c => c.BusCatName)
                .ToListAsync();

            // 4. Product Groups
            var groups = await _context.Product_Groups
                .Where(g => groupIds.Contains((long)g.ProdGroupId))
                .Select(g => new ProductGroupDto
                {
                    ProductGroupId = g.ProdGroupId,
                    ProductGroupName = g.ProdGroupName
                })
                .OrderBy(g => g.ProductGroupName)
                .ToListAsync();

            // 5. Product Types
            var types = await _context.Product_Types
                .Where(t => typeIds.Contains((long)t.ProdTypeId))
                .Select(t => new ProductTypeDto
                {
                    ProdTypeId = t.ProdTypeId,
                    ProductTypeName = t.ProdTypeName
                })
                .OrderBy(t => t.ProductTypeName)
                .ToListAsync();

            return new ProductMasterNamesDto
            {
                ProductCategories = categories,
                ProductGroups = groups,
                ProductTypes = types
            };
        }
    }
}