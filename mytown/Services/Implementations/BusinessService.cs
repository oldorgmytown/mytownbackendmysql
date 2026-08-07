using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using MyTown.Models;

namespace mytown.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _repo;

        public BusinessService(IBusinessRepository repo)
        {
            _repo = repo;
        }

        public Task<bool> IsEmailTaken(string email)
        {
            return _repo.IsEmailTaken(email);
        }

        public Task<PendingBusinessVerification?> FindPendingVerificationByToken(string token)
        {
            return _repo.FindPendingVerificationByToken(token);
        }

        public async Task<PendingBusinessVerification?> FindPendingVerificationByEmail(string email)
        {
            var verification = await _repo.FindPendingVerificationByEmail(email);
            return verification;
        }



        public Task SavePendingVerification(PendingBusinessVerification pending)
        {
            return _repo.SavePendingVerification(pending);
        }


        //public Task RemoveVerification(BusinessVerification verification)
        //{
        //    return _repo.RemoveVerification(verification);
        //}

        public Task DeletePendingVerification(string token)
        {
            return _repo.DeletePendingVerification(token);
        }

        public Task RegisterBusiness(BusinessRegister newBusiness)
        {
            return _repo.RegisterBusiness(newBusiness);
        }

        public Task CreateProfile(BusinessProfile profile)
        {
            return _repo.CreateProfile(profile);
        }

        public Task<BusinessRegister?> GetBusinessByIdAsync(int busRegId)
        {
            return _repo.GetBusinessByIdAsync(busRegId);
        }

        public async Task<IEnumerable<BusinessCategory>> GetBusinessCategories()
        {
            var result = await _repo.GetBusinessCategories();
            return result.Value ?? Enumerable.Empty<BusinessCategory>();
        }

        public Task<IEnumerable<ProductSubCategory>> BusinessSubCategoriesforStores(int buscatid)
        {
            return _repo.BusinessSubCategoriesforStores(buscatid);
        }

        //add bank account details
        public async Task SaveBusinessAccountDetails(BusinessAccountDetail businessAccountDetail)
        {
            await _repo.SaveBusinessAccountDetails(businessAccountDetail);
        }
    }
}
