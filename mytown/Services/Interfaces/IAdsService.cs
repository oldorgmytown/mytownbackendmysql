using mytown.Models.DTO_s;

namespace mytown.Services.Interfaces
{
    public interface IAdsService
    {
        Task<object?> GetPromotionsAsync(int busRegId);
        Task<object?> CreatePromotionAsync(AdsCreatePromotionDto dto);
        Task<object?> UploadMediaAsync(IFormFile file);
        Task<object?> CreatePaymentOrderAsync(AdsPaymentOrderRequestDto dto);
        Task<object?> ConfirmPaymentAsync(AdsPaymentConfirmDto dto);
        Task<object?> CreateStripeOrderAsync(AdsPaymentOrderRequestDto dto);
        Task<object?> ConfirmStripePaymentAsync(AdsStripeConfirmDto dto);
        Task<object?> GetProductsAsync(int busRegId);
        Task<object?> GetStoreInfoAsync(int busRegId);
        Task<object?> DeletePromotionAsync(int busRegId, int promotionId);
Task<object?> GetStoreAdsAsync(int busRegId);
    }
}