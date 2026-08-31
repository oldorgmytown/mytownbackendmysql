using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;
using System.Threading.Tasks;

namespace mytown.Services.Interfaces
{
    public interface IBusinessService
    {
        Task<bool> IsEmailTaken(string email);

        Task<PendingBusinessVerification?> FindPendingVerificationByToken(string token);

        Task<PendingBusinessVerification?> FindPendingVerificationByEmail(string email);

        Task SavePendingVerification(PendingBusinessVerification pending);

        //  Task RemoveVerification(BusinessVerification verification);

        Task DeletePendingVerification(string token);

        Task RegisterBusiness(BusinessRegister newBusiness);

        Task CreateProfile(BusinessProfile profile);

        Task<BusinessRegister?> GetBusinessByIdAsync(int busRegId);

        Task<IEnumerable<BusinessCategory>> GetBusinessCategories();

        Task<IEnumerable<ProductSubCategory>> BusinessSubCategoriesforStores(int buscatId);

        Task<IEnumerable<ProductGroupResponseDto>> GetProductGroupsBySubCategoryId(int prodSubcatId);

        Task<IEnumerable<ProductType>> GetProductTypesByGroupAndSubCategory(int prodSubcatId, int prodGroupId);

        Task<IEnumerable<ProductAttributeDto>> GetAttributesBySubCategoryId(int prodSubcatId, int busCatId, int productGroupId);

        // add bank details
        Task SaveBusinessAccountDetails(BusinessAccountDetail businessAccountDetail);

    }
}
