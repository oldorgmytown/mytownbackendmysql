using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.mytown.DataAccess; // AppDbContext lives here
using mytown.DataAccess.Interfaces;

namespace mytown.DataAccess.Repositories
{
    public class ServiceSubCategoryRepository : IServiceSubCategoryRepository
    {
        private readonly AppDbContext _context;

        public ServiceSubCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceSubCategory>> GetAllAsync()
        {
            return await _context.ServiceSubCategory.ToListAsync();
        }
    }
}