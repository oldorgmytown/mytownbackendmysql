using mytown.DTOs;

public interface ISenderTransporterPayoutService
{
    Task<TriggerPayoutResponseDto> CreatePayoutAsync(int senderOrderId);
}