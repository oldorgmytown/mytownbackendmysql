using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
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

        Task<List<BusinessServiceTypesDto>> GetBusinessServiceTypesAsync(int busRegId);

        //get allservices
        Task<List<Service>> GetServicesByBusRegIdAsync(int busRegId);

        // edit service types
        Task<bool> UpdateServiceAsync(UpdateServiceDto dto);

        // ---------- Service enquiry ----------
        Task<mytown.Models.ServiceProfile?> GetEnquiryProfileAsync(int serviceProfileId);

        Task<List<mytown.Models.Service>> GetEnquiryServicesAsync(int busRegId, int busServId);

        Task<mytown.Models.Service?> GetEnquiryServiceAsync(int serviceId, int busRegId, int busServId);

        Task<List<ServiceBlockedSlot>> GetBlockedSlotsAsync(int serviceProfileId, DateTime from, DateTime to);

        // slots held by enquiries with one of the given statuses, between from and to (inclusive)
        Task<List<BookedSlotDto>> GetBookedSlotsAsync(int serviceProfileId, DateTime from, DateTime to, string[] statuses);

        Task AddServiceEnquiryAsync(ServiceEnquiry enquiry);

        Task<EnquiryNotificationInfo?> GetEnquiryNotificationInfoAsync(ServiceEnquiry enquiry);

    }
}