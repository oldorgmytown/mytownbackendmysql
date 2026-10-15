using mytown.DTOs;

//push
public interface IStorePayoutService
{
    Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId);
}