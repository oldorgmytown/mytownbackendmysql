using mytown.Models;

namespace mytown.Services.Interfaces
{
    public interface IServiceSubCategoryService
    {
        Task<IEnumerable<ServiceSubCategory>> GetAllAsync();
    }
}