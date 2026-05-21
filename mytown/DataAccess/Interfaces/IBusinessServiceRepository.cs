using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTOs;
using MyTown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessServiceRepository
    {
        Task<List<mytown.Models.BusinessService>> GetAllServicesAsync();
        
        Task<List<ServiceSubCategory>> GetByBusServIdAsync(int busServId);
        Task<bool> AddOrUpdateServiceProfileAsync(CreateServiceProfileDto dto);

        Task<BusinessRegister?> GetByBusRegIdAsync(int busRegId);

        Task<ServiceProfileDetailsDto?> GetServiceProfileDetailsAsync(int busRegId);
    }
}