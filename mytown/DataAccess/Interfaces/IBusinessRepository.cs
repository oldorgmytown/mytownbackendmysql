using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using MyTown.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.DataAccess
{
    public interface IBusinessRepository
    {
        Task<bool> IsEmailTaken(string email);
        Task SavePendingVerification(PendingBusinessVerification pending);       
        Task<PendingBusinessVerification> FindPendingVerificationByToken(string token);    
        Task DeletePendingVerification(string token);     
        Task RegisterBusiness(BusinessRegister business);

        //FOR resend email verifcation
        Task<BusinessVerification> FindPendingVerificationByEmail(string email);
        Task RemoveVerification(BusinessVerification verification);
        Task<BusinessRegister> GetBusinessByIdAsync(int busRegId);

        //add profile after email verification
        Task CreateProfile(BusinessProfile profile);
        Task<ActionResult<IEnumerable<BusinessCategory>>> GetBusinessCategories();
        Task<ActionResult<IEnumerable<BusinessService>>> GetBusinessServices();
        Task<IEnumerable<ProductSubCategory>> BusinessSubCategoriesforStores(int buscatId);
       
       
        Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath);

      
        List<ProductSubCategory> GetProductSubCategoriesByBusRegId(int busRegId);
       
        IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId);
    }
}
