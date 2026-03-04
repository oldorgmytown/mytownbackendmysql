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

    //for resend email

    Task<PendingCourierVerification> FindPendingVerificationByEmail(string email);
    Task RemoveVerification(PendingCourierVerification verification);

    Task SavePendingVerification(PendingCourierVerification pending);

    // upload csv file for branch details
    Task<List<CourierBranchCsvRowDto>> ParseAndValidateCsv(IFormFile file);

    Task<bool> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows);

   // Task <List<BestcourierinfoDto>> GetBestCourierOptions(BusinessRegister business, ShopperRegister shopper, decimal productWeightKg);

    Task<List<BestcourierinfoDto>> GetBestCourierOptions(string storeCity, string storeState, string storeCountry, string shopperCity, decimal productWeightKg);
    //   Task<List<AssignedOrderDto>> GetAssignedOrdersByCourierIdAsync(int courierId);

    Task<ShopperRegister?> GetShopperByIdAsync(int shopperId);
    Task<Dictionary<int, BusinessRegister>> GetStoresByIdsAsync(List<int> storeIds);
    Task<Dictionary<int, decimal>> GetStoreWeightsAsync(int shopperId, List<int> storeIds);


}

