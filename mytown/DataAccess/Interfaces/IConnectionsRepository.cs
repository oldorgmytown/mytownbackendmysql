using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IConnectionsRepository
    {
        // Shopper Experinces

        Task<ShopperExperience> CreateExperienceAsync(
     ShopperExperience experience);

        Task CreateExperiencePhotosAsync(
            List<ShopperExperiencePhoto> photos);
        Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId, int shopperRegId);


        // online visitors
        Task CaptureBusinessProfileViewAsync(CaptureBusinessProfileViewDto request);
        Task<List<CurrentBusinessProfileViewerDto>> GetCurrentBusinessProfileViewersAsync (int busRegId,int currentShopperRegId);

        Task<bool> ConnectBusinessAsync(BusinessConnection connection);
        Task<bool> IsBusinessConnectedAsync(int busRegId, int shopperRegId);

        Task<List<ConnectedShopperDto>> GetConnectedShoppersAsync(int busRegId);


        Task<ShopperExperienceLike> AddExperienceLikeAsync(
    ShopperExperienceLike like);

        Task<bool> RemoveExperienceLikeAsync(
            int shopperExperienceId,
            int shopperRegId);

        Task<bool> IsExperienceLikedAsync(
            int shopperExperienceId,
            int shopperRegId);

        Task<int> GetExperienceLikeCountAsync(
            int shopperExperienceId);

        Task<ShopperExperienceComment> AddExperienceCommentAsync(
            ShopperExperienceComment comment);

        Task<List<ShopperExperienceCommentDto>> GetExperienceCommentsAsync(
            int shopperExperienceId);




    }
}
