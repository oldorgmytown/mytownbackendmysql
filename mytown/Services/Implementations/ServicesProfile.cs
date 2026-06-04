using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using MyTown.Models;

namespace mytown.Services.Implementations
{
    public class ServicesProfile : IServicesProfile
    {
        private readonly IBusinessServiceRepository _repo;
        public ServicesProfile(IBusinessServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<mytown.Models.BusinessService>> GetAllServicesAsync()
        {
            return await _repo.GetAllServicesAsync();


        }

        public async Task<List<ServiceSubCategory>> GetByBusServIdAsync(int busServId)
        {
            return await _repo.GetByBusServIdAsync(busServId);
        }
        public async Task<bool> AddServiceProfileAsync(CreateServiceProfileDto dto)
        {
            return await _repo.AddOrUpdateServiceProfileAsync(dto);
        }

        public async Task<BusinessRegister?> GetByBusRegIdAsync(int busRegId)
        {
            return await _repo.GetByBusRegIdAsync(busRegId);
        }

        public async Task<ServiceProfileDetailsDto?> GetServiceProfileDetailsAsync(int busRegId)
        {
            return await _repo.GetServiceProfileDetailsAsync(busRegId);
        }

        public async Task<List<BusinessServiceTypesDto>> GetBusinessServiceTypesAsync(int busRegId)
        {
            return await _repo.GetBusinessServiceTypesAsync(busRegId);
        }

        public async Task<List<Service>> GetServicesByBusRegIdAsync(int busRegId)
        {
            return await _repo.GetServicesByBusRegIdAsync(busRegId);
        }
    }


}

