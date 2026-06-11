using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using MyTown.Models;

namespace mytown.Services.Implementations
{
    public class ConnectionsService : IConnectionsService
    {
        private readonly IConnectionsRepository _Connectrepo;

        public ConnectionsService(IConnectionsRepository repo)
        {
            _Connectrepo = repo;
        }

        public async Task<ShopperExperienceDto> CreateExperienceAsync(CreateShopperExperienceDto dto)
        {
            var experience = new ShopperExperience
            {
                ShopperRegId = dto.ShopperRegId,
                BusRegId = dto.BusRegId,
                PostType = dto.PostType,
                Rating = dto.Rating,
                Title = dto.Title,
                Experience = dto.ExperienceText,
                IsAnonymous = dto.IsAnonymous,
                Status = "Approved",
                CreatedDate = DateTime.UtcNow
            };

            var result = await _Connectrepo.CreateExperienceAsync(experience);

            return new ShopperExperienceDto
            {
                ShopperExperienceId = result.ShopperExperienceId,
                ShopperRegId = result.ShopperRegId,
                BusRegId = result.BusRegId,
                PostType = result.PostType,
                Rating = result.Rating,
                Title = result.Title,
                Experience = result.Experience,
                IsAnonymous = result.IsAnonymous,
                Status = result.Status,
                CreatedDate = result.CreatedDate
            };
        }

        public async Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId)
        {
            return await _Connectrepo.GetExperiencesByBusinessAsync(busRegId);
        }
    }
}
