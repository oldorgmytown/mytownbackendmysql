using mytown.DTOs;

public interface IStorePayoutService
{
    Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId);
}