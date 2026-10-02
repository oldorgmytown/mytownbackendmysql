using mytown.DTOs;

namespace mytown.Services.Interfaces
{
    public interface ICourierPayoutService
    {
        Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId);
    }
}
