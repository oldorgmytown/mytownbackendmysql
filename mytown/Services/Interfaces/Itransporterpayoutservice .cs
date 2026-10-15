using mytown.DTOs;
using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface ITransporterPayoutService
    {
        Task<TriggerPayoutResponseDto> CreatePayoutAsync(int storeOrderId);
    }
}