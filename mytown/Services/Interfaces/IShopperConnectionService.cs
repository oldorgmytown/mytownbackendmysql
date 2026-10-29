using mytown.DTOs;
using mytown.Models.DTO_s;

namespace mytown.Interfaces
{
    public interface IShopperConnectionService
    {
        Task<ShopperHeaderDto> GetHeaderAsync(int shopperId);
        Task<mytown.DTOs.PagedResultDto<ReceivedRequestDto>> GetReceivedRequestsAsync(
    int shopperId, string? search, int page, int pageSize);
        Task ApproveAsync(int shopperId, int connectionId);

        Task<ConnectionActionDto> SendRequestAsync(int shopperId, int receiverShopperId);

        Task<mytown.DTOs.PagedResultDto<NotConnectedShopperDto>> GetNotConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize);

        Task<mytown.DTOs.PagedResultDto<SentRequestDto>> GetSentRequestsAsync(
    int shopperId, string? search, int page, int pageSize);
        Task WithdrawAsync(int shopperId, int connectionId);
        Task DisconnectAsync(int shopperId, int connectionId);

        Task<mytown.DTOs.PagedResultDto<ReceivedRequestDto>> GetConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize);

    }
}