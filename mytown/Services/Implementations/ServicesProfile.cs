using mytown.DataAccess.Interfaces;
using mytown.Services.Interfaces;
using mytown.DTOs;
using mytown.Models;

namespace mytown.Services.Implementations
{
    public class ServicesProfile : IServicesProfile
    {
        private readonly IBusinessServiceRepository _repo;
        public ServicesProfile(IBusinessServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<BusinessService>> GetAllServicesAsync()
        {
            return await _repo.GetAllServicesAsync();
        }
    }
}

