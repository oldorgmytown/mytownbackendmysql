using mytown.DTOs;
using mytown.Models;
using MyTown.Models;

namespace mytown.Services.Interfaces
{
    public interface IServicesProfile
    {
        Task<List<BusinessService>> GetAllServicesAsync();
        Task<List<ServiceSubCategory>> GetByBusServIdAsync(int busServId);
        Task<bool> AddServiceProfileAsync(CreateServiceProfileDto dto);
        Task<BusinessRegister?> GetByBusRegIdAsync(int busRegId);

        Task<ServiceProfileDetailsDto?> GetServiceProfileDetailsAsync(int busRegId);
    }
}