using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;

        public CartService(ICartRepository repo)
        {
            _repo = repo;
        }

        public Task<AddToCart> AddToCart(AddToCart cartItem)
            => _repo.AddToCart(cartItem);

        public Task<IEnumerable<CartItemDto>> GetCartItems(int shopperRegId)
            => _repo.GetCartItems(shopperRegId);

        public Task<bool> RemoveFromCart(int cartId)
            => _repo.RemoveFromCart(cartId);

        public Task<bool> DecreaseCartItemQty(int cartId)
            => _repo.DecreaseCartItemQty(cartId);

        public Task<bool> IncreaseCartItemQty(int cartId)
            => _repo.IncreaseCartItemQty(cartId);

        public Task<bool> MoveToWishlist(int cartId)
            => _repo.MoveToWishlist(cartId);

        public Task<bool> MoveBackToCart(int cartId)
            => _repo.MoveBackToCart(cartId);

        public Task<bool> UpdateCartStatusAsync(int orderId)
            => _repo.UpdateCartStatusAsync(orderId);

        public Task<bool> UpdateCartStatusByShopperAsync(int shopperRegId)
            => _repo.UpdateCartStatusByShopperAsync(shopperRegId);

        public Task<ShopperRegister> GetShopperDetails(int shopperRegId)
            => _repo.GetShopperDetails(shopperRegId);

        public Task<ProdcVariantforShopperDto?> GetProductAndVariantforCartAsync(int productId)
            => _repo.GetProductAndVariantforCartAsync(productId);

        public Task<bool> AddOrMoveToWishlistdirectlyAsync(int shopperId, int productId, int skuId)
           => _repo.AddOrMoveToWishlistdirectlyAsync(shopperId, productId, skuId);
    
    }
}
