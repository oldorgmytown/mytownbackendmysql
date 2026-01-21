using mytown.Models;
using mytown.Models.DTO_s;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Services.Interfaces
{
    public interface IPaymentService
    {
        // Create Stripe PaymentIntent
        Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int orderId);

        // Save payment after Stripe confirms success
        Payments AddPayment(int orderId, string stripePaymentIntentId, string paymentMethod);
        // Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod);

        List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId);

        List<ShippingDetails> GetShippingDetailsByOrderId(int orderId);

        Task SendCourierEmailAsync(int branchId, int shippingDetailId);

        ShopperRegisterDto GetShopperDetailsByOrderId(int orderId);

    }
}
