using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class ServiceSubCategoryService : IServiceSubCategoryService
    {
        private readonly IServiceSubCategoryRepository _repo;

        public ServiceSubCategoryService(IServiceSubCategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ServiceSubCategory>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }
    }
}