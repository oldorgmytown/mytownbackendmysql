using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.Services.Interfaces
{
    public interface IServicesProfile
    {
        Task<List<mytown.Models.BusinessService>> GetAllServicesAsync();
        Task<List<ServiceSubCategory>> GetByBusServIdAsync(int busServId);
        Task<bool> AddServiceProfileAsync(CreateServiceProfileDto dto);
        Task<BusinessRegister?> GetByBusRegIdAsync(int busRegId);

        Task<ServiceProfileDetailsDto?> GetServiceProfileDetailsAsync(int busRegId);
        Task<List<BusinessServiceTypesDto>> GetBusinessServiceTypesAsync(int busRegId);

        //get allservices
        Task<List<Service>> GetServicesByBusRegIdAsync(int busRegId);

        // edit service type

        Task<bool> UpdateServiceAsync(UpdateServiceDto dto);
       
    }
}