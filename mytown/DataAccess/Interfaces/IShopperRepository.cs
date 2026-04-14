using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IShopperRepository
    {
        Task<ShopperRegister> RegisterShopper(ShopperRegister shopper);
        Task<(bool isTaken, string message)> IsEmailTaken(string email);

        Task SavePendingVerification(PendingVerification pending);
        Task<PendingVerification> FindPendingVerificationByToken(string token);
        Task<PendingVerification> FindPendingVerificationByEmail(string email);
        Task DeletePendingVerification(string token);

        Task<ShopperRegister> GetShopperByIdAsync(int shopperRegId);
        Task<IEnumerable<object>> GetTownsWithStoreCountByCountryAsync(string country);

        Task<IEnumerable<ProdcVariantforShopperDto>> GetRecentlyViewedProductsAsync(
            int shopperId, int days = 7, int limit = 10);

        // Alternate Shopper address
        Task<IEnumerable<ShopperAlternateAddressDto>> GetAddressesByShopperIdAsync(int shopperRegId);
        Task<ShopperAlternateAddressDto?> GetAddressByIdAsync(int id);
        Task<ShopperAlternateAddressDto> AddAddressAsync(ShopperAlternateAddress addressDto);
        Task<bool> DeleteAddressAsync(int id);
    }
}