using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IConnectionsService
    {

        // Shopper Experiences

        Task<ShopperExperienceDto> CreateExperienceAsync(CreateShopperExperienceDto dto);
        Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId);

        Task CaptureBusinessProfileViewAsync(CaptureBusinessProfileViewDto request);

        Task<List<CurrentBusinessProfileViewerDto>> GetCurrentBusinessProfileViewersAsync(int busRegId, int currentShopperRegId);

        Task<bool> ConnectBusinessAsync(BusinessConnection connection);
        Task<bool> IsBusinessConnectedAsync(int busRegId, int shopperRegId);
        Task<List<ConnectedShopperDto>> GetConnectedShoppersAsync(int busRegId);



    }
}
