using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IConnectionsRepository
    {
        // Shopper Experinces

        Task<ShopperExperience> CreateExperienceAsync(ShopperExperience experience);
        Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId);
    }
}
