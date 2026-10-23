using mytown.DTOs;

namespace mytown.Services.Interfaces
{
    //push
    public interface ICourierPayoutService
    {
        Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId);
    }
}
