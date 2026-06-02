using mytown.Models; // ADD THIS

namespace mytown.DataAccess.Interfaces
{
    public interface IServiceSubCategoryRepository
    {
        Task<IEnumerable<ServiceSubCategory>> GetAllAsync();
    }
}