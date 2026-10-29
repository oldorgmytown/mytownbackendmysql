using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Interfaces
{
    public interface IShopperConnectionRepository
    {
        Task<ShopperHeaderDto?> GetShopperHeaderAsync(int shopperId);
        Task<int> GetConnectedCountAsync(int shopperId);

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
    }
}