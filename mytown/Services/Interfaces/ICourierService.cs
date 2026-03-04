using mytown.Models;
using mytown.Models.DTO_s;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Services.Interfaces
{
    public interface ICourierServiceHandler
    {
       
        Task<CourierService?> RegisterCourierAsync(CourierServiceDto courierDto, bool sendVerification = true);

      
        Task<CourierService?> VerifyCourierEmailAsync(string token);

        Task<CourierVerification?> FindPendingVerificationByEmail(string email);

        Task RemoveVerification(CourierVerification verification);
        
        Task SavePendingVerification(PendingCourierVerification pending);
        Task<List<CourierBranchCsvRowDto>> ParseAndValidateCsvAsync(IFormFile file);

       
        Task<bool> SaveCourierBranchesAsync(List<CourierBranchCsvRowDto> rows);


        Task<List<StoreCourierResultDto>> GetBestCourierOptionsByStoresAsync(int shopperId,
      List<int> storeIds);


        // Task<List<AssignedOrderDto>> GetAssignedOrdersByCourierIdAsync(int courierId);


        Task<bool> IsCourierEmailTakenAsync(string email);

       


    }
}


