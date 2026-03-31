using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface ICartRepository
    {
        Task<AddToCart> AddToCart(AddToCart cartItem);

        Task<IEnumerable<CartItemDto>> GetCartItems(int shopperRegId);

        Task<bool> RemoveFromCart(int cartId);

        Task<bool> DecreaseCartItemQty(int cartId);

        Task<bool> IncreaseCartItemQty(int cartId);

        Task<bool> MoveToWishlist(int cartId);

        Task<bool> MoveBackToCart(int cartId);

        Task<bool> UpdateCartStatusAsync(int orderId);

        Task<bool> UpdateCartStatusByShopperAsync(int shopperRegId);
        Task<ShopperRegister> GetShopperDetails(int shopperRegId);

        //get prodcut and variant details for cart
        Task<ProdcVariantforShopperDto?> GetProductAndVariantforCartAsync(int productId);

        //add directlt to wishlist

        Task<bool> AddOrMoveToWishlistdirectlyAsync(int shopperId, int productId, int skuId);



    }
}

  