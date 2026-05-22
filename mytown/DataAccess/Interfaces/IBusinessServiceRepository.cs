using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTOs;

namespace mytown.DataAccess.Repositories
{
    public class BusinessServiceRepository : IBusinessServiceRepository
    {
        private readonly AppDbContext _context;

        public BusinessServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessService>> GetAllServicesAsync()
        {
            return await _context.BusinessServices.ToListAsync();
        }

        public async Task<List<SubCategoryItemDto>> GetSubCategoriesByBusServIdAsync(int busServId)
        {
            return await _context.services_sub_categories
                .Where(s => s.BusservId == busServId)
                .Select(s => new SubCategoryItemDto
                {
                    SubCatId = s.serv_subcat_id,
                    SubCatName = s.serv_subcat_name,
                    SubCatImage = s.serv_subcat_image
                })
                .ToListAsync();
        }
    }
}