using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Interfaces
{
    public interface IShopperConnectionRepository
    {
        Task<ShopperHeaderDto?> GetShopperHeaderAsync(int shopperId);

        Task<int> GetBusinessConnectedCountAsync(int shopperId);
        Task<int> GetConnectedCountAsync(int shopperId);
        Task<int> GetPostsCountAsync(int shopperId);

        Task<(List<ReceivedRequestDto> Items, int TotalCount)> GetReceivedRequestsAsync(
       int shopperId, string? search, int page, int pageSize);
        Task<ShopperConnection?> GetByIdAsync(int connectionId);

        Task<bool> ShopperExistsAsync(int shopperId);
        Task<ShopperConnection?> GetPairAsync(int shopperA, int shopperB);
        Task AddAsync(ShopperConnection connection);
        Task SaveAsync();

        Task<(List<NotConnectedShopperDto> Items, int TotalCount)> GetNotConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize);

     
        Task<(List<SentRequestDto> Items, int TotalCount)> GetSentRequestsAsync(
    int shopperId, string? search, int page, int pageSize);
        void Remove(ShopperConnection connection);

        Task<(List<ReceivedRequestDto> Items, int TotalCount)> GetConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize);

        Task<(List<ShopperBusinessConnectionDto> Items, int TotalCount)> GetBusinessConnectionsAsync(
    int shopperId, string? search, int page, int pageSize);

        Task<(List<AllConnectionDto> Items, int TotalCount)> GetAllConnectionsAsync(
     int shopperId, string? search, int page, int pageSize);

        Task<List<BusinessNameDto>> GetBusinessNamesAsync();

        Task AddAsync(ShopperPost post);
        //  Task SaveAsync();

        Task<ShopperPost?> GetByPostIdAsync(int postId);
        void Remove(ShopperPost post);

        Task<bool> PostExistsAsync(int postId);
        Task<ShopperPostLike?> GetLikeAsync(int postId, int shopperId);
        Task AddLikeAsync(ShopperPostLike like);
        void RemoveLike(ShopperPostLike like);
        Task<int> CountLikesAsync(int postId);

        Task<(List<FeedPostDto> Items, int TotalCount)> GetFeedAsync(
    int shopperId, string filter, string? search, int page, int pageSize);
    }
}