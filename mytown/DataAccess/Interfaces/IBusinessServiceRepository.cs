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

        Task<Service?> GetEnquiryServiceAsync(int serviceId, int busRegId);

        Task<ServiceProfile?> GetEnquiryProfileAsync(int busRegId, int busServId);

        Task<List<ServiceBlockedSlot>> GetBlockedSlotsAsync(
            int serviceProfileId, DateTime startDate, DateTime endDate);

        Task<List<BookedSlotDto>> GetBookedSlotsAsync(
            int serviceProfileId, DateTime startDate, DateTime endDate);

        Task AddServiceEnquiryAsync(ServiceEnquiry enquiry);

        Task<EnquiryNotificationInfo?> GetEnquiryNotificationInfoAsync(ServiceEnquiry enquiry);

        Task<List<BusinessServiceEnquiryListDto>> GetBusinessServiceEnquiriesAsync(int busRegId);

    }
}