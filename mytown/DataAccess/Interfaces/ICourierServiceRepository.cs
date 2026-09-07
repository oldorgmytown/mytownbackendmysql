using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;
using System.Threading.Tasks;

public interface ICourierServiceRepository
{
    Task<bool> IsCourierEmailTaken(string email);

    Task SavePendingCourierVerification(PendingCourierVerification pending);

    Task<PendingCourierVerification> FindPendingCourierVerificationByToken(string token);

    Task DeletePendingCourierVerification(string token);

    Task<CourierService> RegisterCourier(CourierService courier);

    // resend email
    Task<PendingCourierVerification> FindPendingVerificationByEmail(string email);
    Task RemoveVerification(PendingCourierVerification verification);
    Task SavePendingVerification(PendingCourierVerification pending);

    // CSV upload
    Task<List<CourierBranchCsvRowDto>> ParseAndValidateCsv(IFormFile file);
    Task<string> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows);

    // Courier options
    Task<List<BestcourierinfoDto>> GetBestCourierOptions(
        string storeCity,
        string storeState,
        string storeCountry,
        string shopperCity,
        decimal productWeightKg);

    Task<ShopperRegister?> GetShopperByIdAsync(int shopperId);
    Task<ShopperAlternateAddress?> GetAlternateAddressByShopperIdAsync(int shopperId);
    Task<Dictionary<int, BusinessRegister>> GetStoresByIdsAsync(List<int> storeIds);
    Task<Dictionary<int, decimal>> GetStoreWeightsAsync(int shopperId, List<int> storeIds);

    Task<GuestDetailsDto> GetGuestDetailsByIdAsync(int guestRegId);
    // ✅ NEW — Find a matching transporter for P2P
    // Matches transporter who is going FROM storeCity TO shopperCity
    // on or after today's date and still has capacity
    Task<BestcourierinfoDto?> FindMatchingTransporterAsync(
      string storeTown,
      string storeCity,
      string storeState,
      string storeCountry,

      string shopperTown,
      string shopperCity,
      string shopperState,
      string shopperCountry,

      decimal packageWeightKg);

    //add courier bank details
    Task SaveCourierAccountDetails(CourierAccountDetail accountDetail);
}