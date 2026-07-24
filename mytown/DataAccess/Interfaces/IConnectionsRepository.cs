using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IConnectionsRepository
    {
        // Shopper Experinces

        Task<ShopperExperience> CreateExperienceAsync(ShopperExperience experience);
        Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId);

        // online visitors
        Task CaptureBusinessProfileViewAsync(CaptureBusinessProfileViewDto request);
        Task<List<CurrentBusinessProfileViewerDto>> GetCurrentBusinessProfileViewersAsync (int busRegId,int currentShopperRegId);

        Task<bool> ConnectBusinessAsync(BusinessConnection connection);
        Task<bool> IsBusinessConnectedAsync(int busRegId, int shopperRegId);

        Task<List<ConnectedShopperDto>> GetConnectedShoppersAsync(int busRegId);

        


    }
}
