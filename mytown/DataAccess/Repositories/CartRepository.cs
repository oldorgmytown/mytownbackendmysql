using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AddToCart> AddToCart(AddToCart cartItem)
        {
            // Safety: if frontend sends 0 or negative
            if (cartItem.ProdQty <= 0)
                cartItem.ProdQty = 1;

            var existingCartItem = await _context.addtocart
                .FirstOrDefaultAsync(c =>
                    c.ProductId == cartItem.ProductId &&
                    c.SkuId == cartItem.SkuId &&
                    c.BusRegId == cartItem.BusRegId &&
                    c.ShopperRegId == cartItem.ShopperRegId && 
                    c.BuscatId == cartItem.BuscatId &&
                    c.ProdSubcatId == cartItem.ProdSubcatId &&
                    c.orderstatus == "cart");

            if (existingCartItem != null)
            {
                //  Add the quantity sent from frontend
                existingCartItem.ProdQty += cartItem.ProdQty;

                await _context.SaveChangesAsync();
                return existingCartItem;
            }
            else
            {
                //  Keep frontend quantity
                _context.addtocart.Add(cartItem);
                await _context.SaveChangesAsync();
                return cartItem;
            }
        }





        // Get all cart items for a specific user
        //public async Task<IEnumerable<CartItemDto>> GetCartItems(int shopperRegId)
        //{
        //    var cartItems = await (
        //        from cart in _context.addtocart

        //        join product in _context.products
        //            on cart.ProductId equals product.ProductId

        //        // 🔹 Store Profile (for logo)
        //        join storeProfile in _context.BusinessProfiles
        //            on product.BusRegId equals storeProfile.BusRegId

        //        // 🔹 SKU
        //        join sku in _context.Sku_ProductVariants
        //            on cart.SkuId equals sku.SkuId into skuGroup
        //        from skuVariant in skuGroup.DefaultIfEmpty()

        //            // 🔹 Size
        //        join size in _context.ProductSizes
        //            on skuVariant.SizeId equals size.SizeId into sizeGroup
        //        from sizeDetail in sizeGroup.DefaultIfEmpty()

        //            // 🔹 SKU Image (SortOrder = 1)
        //        join skuImg in _context.ProductImages
        //            .Where(i => i.SortOrder == 1)
        //            on skuVariant.SkuId equals skuImg.SkuId into skuImgGroup
        //        from skuImage in skuImgGroup.DefaultIfEmpty()

        //            // 🔹 Product Image fallback
        //        join prodImg in _context.ProductImages
        //            .Where(i => i.SortOrder == 1 && i.SkuId == null)
        //            on product.ProductId equals prodImg.ProductId into prodImgGroup
        //        from productImage in prodImgGroup.DefaultIfEmpty()

        //        where cart.ShopperRegId == shopperRegId
        //              && cart.orderstatus == "cart"

        //        select new CartItemDto
        //        {
        //            CartId = cart.CartId,
        //            ShopperRegId = cart.ShopperRegId,
        //            prod_qty = cart.ProdQty,
        //            orderstatus = cart.orderstatus,

        //            product_id = product.ProductId,
        //            product_name = product.ProductName,
        //            product_subject = product.ProductSubject,
        //            product_description = product.ProductDescription,

        //            // ✅ Image priority: SKU → Product
        //            product_image = skuImage.FileName ?? productImage.FileName,

        //            // ✅ Price from SKU
        //            product_cost = skuVariant.Sku_Cost,

        //            StoreId = storeProfile.BusRegId,
        //            StoreName = storeProfile.BusinessName,
        //            StoreLocation = storeProfile.BusinessLocation,

        //            // ✅ NEW: Store logo
        //            StoreLogo = storeProfile.LogoPath,

        //            Color = skuVariant.Color,
        //            SizeId = skuVariant.SizeId ?? 0,
        //            SizeName = sizeDetail.SizeName,
        //            Sku_Cost = skuVariant.Sku_Cost,
        //            Discount = skuVariant.Discount,
        //            DiscountPrice = skuVariant.DiscountPrice
        //        }
        //    ).ToListAsync();

        //    return cartItems;
        //}



        // Remove an item from cart

        public async Task<IEnumerable<CartItemDto>> GetCartItems(int shopperRegId)
        {
            var cartItems = await (
                from cart in _context.addtocart

                join product in _context.products
                    on cart.ProductId equals product.ProductId

                join storeProfile in _context.BusinessProfiles
                    on product.BusRegId equals storeProfile.BusRegId

                join sku in _context.Sku_ProductVariants
                    on cart.SkuId equals sku.SkuId

                join size in _context.ProductSizes
                    on sku.SizeId equals size.SizeId into sizeGroup
                from sizeDetail in sizeGroup.DefaultIfEmpty()

                where cart.ShopperRegId == shopperRegId
                      && cart.orderstatus == "cart"

                select new CartItemDto
                {
                    CartId = cart.CartId,
                    ShopperRegId = cart.ShopperRegId,
                    prod_qty = cart.ProdQty,
                    orderstatus = cart.orderstatus,

                    product_id = product.ProductId,
                    product_name = product.ProductName,
                    product_subject = product.ProductSubject,
                    product_description = product.ProductDescription,

                    product_image =
                        _context.ProductImages
                            .Where(i =>
                                i.SkuId == sku.SkuId &&
                                i.SortOrder == 1)
                            .Select(i => i.FileName)
                            .FirstOrDefault(),

                    // SKU-specific data
                    Sku_Id = sku.SkuId,
                    product_cost = sku.Sku_Cost,
                    Color = sku.Color,
                    SizeId = sku.SizeId ?? 0,
                    SizeName = sizeDetail.SizeName,
                    Discount = sku.Discount,
                    DiscountPrice = sku.DiscountPrice,

                    StoreId = storeProfile.BusRegId,
                    StoreName = storeProfile.BusinessName,
                    StoreLocation = storeProfile.BusinessLocation,
                    StoreLogo = storeProfile.LogoPath,
                    // 🔹 NEW
                    IsProductAvailable =
                        product.ProductStatus == "Approved"
                        && product.IsActive == true
                        && storeProfile.ProfileStatus == "approved"
                }
            ).ToListAsync();

            return cartItems;
        }

        public async Task<bool> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.addtocart.FindAsync(cartId);
            if (cartItem == null) return false;

            _context.addtocart.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DecreaseCartItemQty(int cartId)
        {
            var cartItem = await _context.addtocart.FindAsync(cartId);

            if (cartItem == null)
            {
                return false; // Item not found
            }

            if (cartItem.ProdQty > 1)
            {
                cartItem.ProdQty -= 1; // Decrease quantity by 1
            }
            else
            {
                _context.addtocart.Remove(cartItem); // Remove item if quantity reaches 0
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncreaseCartItemQty(int cartId)
        {
            var cartItem = await _context.addtocart.FindAsync(cartId);

            if (cartItem == null)
            {
                return false; // Item not found
            }

            cartItem.ProdQty += 1; // Increase quantity by 1

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveToWishlist(int cartId)
        {
            var item = await _context.addtocart.FindAsync(cartId);
            if (item != null)
            {
                item.orderstatus = "wishlist";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //Add to wishlist directly from prodcut detail page
        public async Task<bool> AddOrMoveToWishlistDirectlyAsync(int shopperId, int productId, int skuId)
        {
            // 1️⃣ Validate Product
            var product = await _context.products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == productId &&
                    p.ProductStatus == "Approved" &&
                    p.IsActive == true);

            if (product == null)
                throw new Exception("Product not available");

            // 2️⃣ Validate SKU
            var sku = await _context.Sku_ProductVariants
                .FirstOrDefaultAsync(s =>
                    s.SkuId == skuId &&
                    s.ProductId == productId);

            if (sku == null)
                throw new Exception("SKU not found");

            // 3️⃣ Check if already in wishlist
            var exists = await _context.Wishlist
                .AnyAsync(w =>
                    w.ShopperRegId == shopperId &&
                    w.ProductId == productId &&
                    w.SkuId == skuId);

            if (!exists)
            {
                var newWishlistItem = new Wishlist
                {
                    ShopperRegId = shopperId,
                    ProductId = productId,
                    SkuId = skuId,
                    BusRegId = product.BusRegId,
                    BuscatId = product.BuscatId,
                    ProdSubcatId = product.ProdSubcatId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Wishlist.Add(newWishlistItem);
            }

            // 4️⃣ Remove from cart if exists
            var cartItem = await _context.addtocart
                .FirstOrDefaultAsync(c =>
                    c.ShopperRegId == shopperId &&
                    c.ProductId == productId &&
                    c.SkuId == skuId);

            if (cartItem != null)
            {
                _context.addtocart.Remove(cartItem);
            }

            await _context.SaveChangesAsync();

            return !exists; // true if added, false if already existed
        }



        public async Task<bool> MoveBackToCart(int cartId)
        {
            var item = await _context.addtocart.FindAsync(cartId);
            if (item != null)
            {
                item.orderstatus = "cart";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateCartStatusAsync(int orderId)
        {
            // Find the order using OrderId
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false; // Order not found

            // Get cart items for the shopper related to the order
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == order.ShopperRegId && c.orderstatus == "cart")
                .ToListAsync();

            if (!cartItems.Any()) return false; // No cart items to update

            // Update cart status
            foreach (var item in cartItems)
            {
                item.orderstatus = "Ordered";
            }

            // Save changes
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCartStatusByShopperAsync(int shopperRegId)
        {
            // Get cart items for this shopper where the order status is still "In Cart"
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "In Cart")
                .ToListAsync();

            if (!cartItems.Any()) return false; // No cart items to update

            // Update status to "Ordered"
            foreach (var item in cartItems)
            {
                item.orderstatus = "Ordered";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ShopperRegister> GetShopperDetails(int shopperRegId)
        {
            return await _context.ShopperRegisters.FindAsync(shopperRegId);
        }

        // get product and variant details on cart for shopper

        public async Task<ProdcVariantforShopperDto?> GetProductAndVariantforCartAsync(int productId)
        {
            var product = await _context.products
                .Include(p => p.Sku_ProductVariants)
                    .ThenInclude(v => v.Images)
                .Include(p => p.Sku_ProductVariants)
                    .ThenInclude(v => v.Size)
                .Include(p => p.BusinessRegister)
                
                .Include(p => p.ProductType)
                .Include(p => p.Fabric)
                .Include(p => p.Design)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return null;

            return new ProdcVariantforShopperDto
            {
                ProductId = product.ProductId,
                BusRegId = product.BusRegId,
                BuscatId = product.BuscatId,
                ProdcatId = product.ProdSubcatId,

                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                SupplierName = product.SupplierName,
                ProductTypeId = product.ProductTypeId,
                FabricId = product.FabricId,
                DesignId = product.DesignId,

                ProductTypeName = product.ProductType != null ? product.ProductType.ProdTypeName : null,
                FabricName = product.Fabric != null ? product.Fabric.FabricName : null,
                DesignName = product.Design != null ? product.Design.DesignName : null,

                
                BusinessName = product.BusinessRegister != null ? product.BusinessRegister.BusinessName : null,
              
                //  Variants
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
                        }).ToList()
                }).ToList()
            };
        }

    }
}
