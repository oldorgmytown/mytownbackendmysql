using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.DataAccess.Interfaces;   // ADD THIS LINE
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

        public async Task<List<BusinessService>> GetAllServicesAsync()
        {
            return await _context.BusinessServices.ToListAsync();
        }
    }
}