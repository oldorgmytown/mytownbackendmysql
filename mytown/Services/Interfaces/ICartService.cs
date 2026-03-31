using mytown.Models;
using mytown.Models.DTO_s;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Services.Interfaces
{
    public interface ICartService
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

        Task<ProdcVariantforShopperDto?> GetProductAndVariantforCartAsync(int productId);

        Task<bool> AddOrMoveToWishlistdirectlyAsync(int shopperId, int productId, int skuId);
    }
}
