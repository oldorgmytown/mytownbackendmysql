using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IShopperService
    {
        // Registration + Verification
        Task<(bool success, string message)> RegisterShopperAsync(ShopperRegisterDto dto);
        Task<(bool success, string message, int? shopperRegId)> VerifyEmailAsync(string token);
        Task<(bool success, string message)> ResendVerificationEmailAsync(string email);

        // Shopper data
        Task<IEnumerable<object>> GetTownsWithStoreCountByCountryAsync(string country);
        Task<IEnumerable<ProdcVariantforShopperDto>> GetRecentlyViewedProductsAsync(
            int shopperId, int days, int limit);

        // Alternate address
        Task<IEnumerable<ShopperAlternateAddressDto>> GetAddressesAsync(int shopperRegId);
        Task<ShopperAlternateAddressDto> AddAddressAsync(ShopperAlternateAddressDto dto);
        Task<bool> DeleteAddressAsync(int id);

        Task<(bool exists, string message)> CheckEmailExistsAsync(string email);

        
    }
}
