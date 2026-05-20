using mytown.Models.DTOs;

namespace mytown.Services.Interfaces
{
    public interface IBusinessServiceService
    {
        Task<List<mytown.Models.BusinessService>> GetAllServicesAsync();
        Task<BusinessSubCategoriesDto?> GetSubCategoriesByBusRegIdAsync(int busRegId);
    }
}