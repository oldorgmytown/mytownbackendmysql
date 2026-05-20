using mytown.DataAccess.Interfaces;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class BusinessServiceService : IBusinessServiceService
    {
        private readonly IBusinessServiceRepository _repo;
        public BusinessServiceService(IBusinessServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<mytown.Models.BusinessService>> GetAllServicesAsync()
        {
            return await _repo.GetAllServicesAsync();
        }
    }
}