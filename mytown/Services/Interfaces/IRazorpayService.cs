using MyTown.DTOs.Razorpay;

namespace mytown.Services.Interfaces
{
    public interface IRazorpayService
    {
        Task<CreateContactResponseDto> CreateContactAsync(
            CreateContactRequestDto request);
    }
}