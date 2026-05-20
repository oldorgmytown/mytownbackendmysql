using mytown.Models.DTOs;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessServiceRepository
    {
        Task<List<mytown.Models.BusinessService>> GetAllServicesAsync();
        Task<BusinessSubCategoriesDto?> GetSubCategoriesByBusRegIdAsync(int busRegId);
    }
}