using MyTown.DTOs.Razorpay;

namespace MyTown.Services.Interfaces
{
    public interface IRazorpayService
    {
        Task<CreateContactResponseDto> CreateContactAsync(
            CreateContactRequestDto request);
    }
}