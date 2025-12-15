using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;

        public PaymentService(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod)
        {
            return _paymentRepo.AddPayment(orderId, amountPaid, paymentMethod);
        }

        public List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId)
        {
            return _paymentRepo.GetStoreDetailsByOrderId(orderId);
        }

        public List<ShippingDetails> GetShippingDetailsByOrderId(int orderId)
        {
            return _paymentRepo.GetShippingDetailsByOrderId(orderId);
        }

        public async Task SendCourierEmailAsync(int branchId, int shippingDetailId)
        {
            await _paymentRepo.SendEmailToCourier(branchId, shippingDetailId);
        }

        public ShopperRegisterDto GetShopperDetailsByOrderId(int orderId)
        {
            return _paymentRepo.GetShopperDetailsByOrderId(orderId);
        }
    }
}
