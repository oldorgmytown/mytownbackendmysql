using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;

namespace mytown.DataAccess.Repositories
{
    public class ConnectionsRepository : IConnectionsRepository
    {
        private readonly AppDbContext _context;

        public ConnectionsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShopperExperience> CreateExperienceAsync(ShopperExperience experience)
        {
            _context.ShopperExperiences.Add(experience);
            await _context.SaveChangesAsync();
            return experience;
        }

        public async Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId)
        {
            return await (from e in _context.ShopperExperiences
                          join s in _context.ShopperRegisters on e.ShopperRegId equals s.ShopperRegId
                          join b in _context.BusinessRegisters on e.BusRegId equals b.BusRegId
                          where e.BusRegId == busRegId && e.Status == "Approved"
                          orderby e.CreatedDate descending
                          select new ShopperExperienceDto
                          {
                              ShopperExperienceId = e.ShopperExperienceId,
                              ShopperRegId = e.ShopperRegId,
                              ShopperName = e.IsAnonymous ? "Anonymous" : s.Username,
                              BusRegId = e.BusRegId,
                              BusinessName = b.BusinessName,
                              PostType = e.PostType,
                              Rating = e.Rating,
                              Title = e.Title,
                              Experience = e.Experience,
                              IsAnonymous = e.IsAnonymous,
                              Status = e.Status,
                              CreatedDate = e.CreatedDate
                          }).ToListAsync();
        }
    }
}
