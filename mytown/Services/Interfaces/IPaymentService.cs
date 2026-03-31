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
        Task<Payments> AddPaymentAsync(int orderId, string stripePaymentIntentId, string paymentMethod); //New
        // Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod);

        List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId);

        List<ShippingDetails> GetShippingDetailsByOrderId(int orderId);

        // POST PAYMENT (EMAIL + NOTIFY)
        // ================================
        Task ProcessPostPaymentAsync(int orderId);

      //  Task SendCourierEmailAsync(int branchId, int storeOrderId);

        ShopperRegisterDto GetShopperDetailsByOrderId(int orderId);

        Task AddCourierNotificationAsync(
    int courierId,
    int branchId,
    string title,
    string message);
       

    }
}
