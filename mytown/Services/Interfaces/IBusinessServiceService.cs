using System.Collections.Generic;
using System.Threading.Tasks;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTOs;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class BusinessServiceService : IBusinessServiceService
    {
        private readonly IBusinessServiceRepository _repo;

        public BusinessServiceService(IBusinessServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<BusinessService>> GetAllServicesAsync()
        {
            return await _repo.GetAllServicesAsync();
        }

        public async Task<List<SubCategoryItemDto>> GetSubCategoriesByBusServIdAsync(int busServId)
        {
            return await _repo.GetSubCategoriesByBusServIdAsync(busServId);
        }
    }
}