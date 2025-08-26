using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.DataAccess.Interfaces;
using mytown.Models.mytown.DataAccess;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<products> CreateProductAsync(products product)
        {
            await _context.products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }



        //edit or update product details 
        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == productId);
            if (product != null)
            {
                _context.products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<products> UpdateProductAsync(products product)
        {
            _context.products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
        //edit or update product details 
        public bool UpdateProduct(products updatedProduct)
        {
            var existingProduct = _context.products.FirstOrDefault(p => p.product_id == updatedProduct.product_id);

            if (existingProduct == null)
                return false;

            // Update fields
            existingProduct.product_name = updatedProduct.product_name;
            existingProduct.product_subject = updatedProduct.product_subject;
            existingProduct.product_description = updatedProduct.product_description;
            existingProduct.product_image = updatedProduct.product_image;
            existingProduct.product_cost = updatedProduct.product_cost;
            existingProduct.product_length = updatedProduct.product_length;
            existingProduct.product_width = updatedProduct.product_width;
            existingProduct.product_weight = updatedProduct.product_weight;
            existingProduct.product_quantity = updatedProduct.product_quantity;

            _context.SaveChanges(); // Commit changes to the database
            return true;
        }

        public async Task<products> GetProductById(int productId)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == productId);

            return product ?? new products(); // Return an empty product if not found
        }



        // Get all products from the database based on busregid
        public async Task<IEnumerable<products>> GetAllProductsAsync(int BusRegId)
        {
            return await _context.products
                                 .Where(p => p.BusRegId == BusRegId) // Filter by BusRegId
                                 .ToListAsync(); // Fetch matching products
        }

        public async Task<IEnumerable<ProductDto>> GetDiscountedProductsAsync()
        {
            return await _context.products
                .Include(p => p.BusinessRegister)
                .Where(p => p.discount != null) //filter only products with discount
                .Select(p => new ProductDto
                {
                    ProductId = p.product_id,
                    BusRegId = p.BusRegId,
                    BuscatId = p.BuscatId,
                    ProdSubcatId = p.prod_subcat_id,
                    ProductName = p.product_name,
                    ProductSubject = p.product_subject,
                    ProductDescription = p.product_description,
                    ProductImage = p.product_image,
                    ProductAmount = p.product_cost,
                    ProductLength = p.product_length,
                    ProductWidth = p.product_width,
                    ProductWeight = p.product_weight,
                    Quantity = p.product_quantity,
                    ProductHeight = p.product_height,
                    Color = p.color,
                    Size = p.size,

                    Discount = p.discount,
                    DiscountPrice = p.discount_price,

                    BusinessName = p.BusinessRegister.Businessname
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<ProductDto>> GetProductsBySubCategoryAsync(int subCategoryId)
        {
            var products = await _context.products
                .Where(p => p.prod_subcat_id == subCategoryId)
                .Include(p => p.BusinessRegister)
                .Select(p => new ProductDto
                {
                    ProductId = p.product_id,
                    BusRegId = p.BusRegId,
                    BuscatId = p.BuscatId,
                    ProdSubcatId = p.prod_subcat_id,
                    ProductName = p.product_name,
                    ProductSubject = p.product_subject,
                    ProductDescription = p.product_description,
                    ProductImage = p.product_image,
                    ProductAmount = p.product_cost,
                    ProductLength = p.product_length,
                    ProductWidth = p.product_width,
                    ProductWeight = p.product_weight,
                    Quantity = p.product_quantity,
                    ProductHeight = p.product_height,
                    Size = p.size,
                    Color = p.color,
                    Discount = p.discount,
                    DiscountPrice = p.discount_price,
                    BusinessName = p.BusinessRegister.Businessname,

                    // optional fields
                    PurchasedCount = 0, // you can calculate later if needed
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


    }
}

