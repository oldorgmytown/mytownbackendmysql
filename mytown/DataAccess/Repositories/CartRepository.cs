using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
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

        public async Task<addtocart> AddToCart(addtocart cartItem)
        {
            var existingCartItem = await _context.addtocart
         .FirstOrDefaultAsync(c =>
             c.product_id == cartItem.product_id &&
             c.BusRegId == cartItem.BusRegId && // Check if the product is from the same store
             c.ShopperRegId == cartItem.ShopperRegId &&
             c.orderstatus == "cart"); // Only check active cart items


            if (existingCartItem != null)
            {
                //Product exists, so increase quantity by 1
                existingCartItem.prod_qty += 1;
                await _context.SaveChangesAsync();
                return existingCartItem;
            }
            else
            {
                //New product, insert into cart
                cartItem.prod_qty = 1; // Ensure quantity starts at 1
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return cartItem;
                //}
            }
        }




        // Get all cart items for a specific user
        public async Task<IEnumerable<CartItemDto>> GetCartItems(int shopperRegId)
        {
            var cartItems = await (from cart in _context.addtocart
                                   join product in _context.products
            on cart.product_id equals product.product_id
                                   join business in _context.BusinessProfiles
                                   on product.BusRegId equals business.BusRegId
                                   where cart.ShopperRegId == shopperRegId && cart.orderstatus == "cart"
                                   select new CartItemDto
                                   {
                                       CartId = cart.CartId,
                                       ShopperRegId = cart.ShopperRegId,
                                       prod_qty = cart.prod_qty,
                                       orderstatus = cart.orderstatus,
                                       product_id = product.product_id,
                                       product_name = product.product_name,
                                       product_subject = product.product_subject,
                                       product_description = product.product_description,
                                       product_image = product.product_image,
                                       product_cost = product.product_cost ?? 0,
                                       StoreName = business.BusinessUsername, // Store name
                                       StoreLocation = business.business_location, // Store location
                                       StoreId = business.BusRegId
                                   }).ToListAsync();

            return cartItems;
        }



        // Remove an item from cart
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

            if (cartItem.prod_qty > 1)
            {
                cartItem.prod_qty -= 1; // Decrease quantity by 1
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

            cartItem.prod_qty += 1; // Increase quantity by 1

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
    }
}
