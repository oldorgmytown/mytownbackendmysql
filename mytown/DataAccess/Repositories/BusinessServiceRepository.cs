using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace mytown.DataAccess.Repositories
{
    public class BusinessServiceRepository : IBusinessServiceRepository
    {
        private readonly AppDbContext _context;
        public BusinessServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<mytown.Models.BusinessService>> GetAllServicesAsync()
        {
            return await _context.BusinessServices.ToListAsync();
        }

        public async Task<BusinessSubCategoriesDto?> GetSubCategoriesByBusRegIdAsync(int busRegId)
        {
            // Get business profile by busRegId
            var profile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(p => p.BusRegId == busRegId);

            if (profile == null) return null;

            // Get category name
            var category = await _context.BusinessCategories
                .FirstOrDefaultAsync(c => c.BusCatId == profile.BusCatId);

            if (category == null) return null;

            var result = new BusinessSubCategoriesDto
            {
                BusRegId = busRegId,
                CategoryName = category.BusinessCategoryName,
                SubCategories = new List<SubCategoryItemDto>()
            };

            // Category 1 = products, Category 2 = services
            if (profile.BusCatId == 1)
            {
                var productSubs = await _context.product_sub_categories
                    .Where(p => p.BuscatId == profile.BusCatId)
                    .ToListAsync();

                result.SubCategories = productSubs.Select(p => new SubCategoryItemDto
                {
                    SubCatId = p.ProdSubcatId,
                    SubCatName = p.ProdSubcatName,
                    SubCatImage = p.ProdSubcatImage
                }).ToList();
            }
            else if (profile.BusCatId == 2)
            {
                var serviceSubs = await _context.services_sub_categories
                    .Where(s => s.BusservId == profile.BusServId)
                    .ToListAsync();

                result.SubCategories = serviceSubs.Select(s => new SubCategoryItemDto
                {
                    SubCatId = s.serv_subcat_id,
                    SubCatName = s.serv_subcat_name,
                    SubCatImage = s.serv_subcat_image
                }).ToList();
            }

            return result;
        }
    }
}