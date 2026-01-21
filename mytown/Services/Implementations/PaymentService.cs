using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;

namespace mytown.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;

        public PaymentService(IPaymentRepository paymentRepo, IConfiguration configuration)
        {
            _paymentRepo = paymentRepo;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int orderId)
        {
            var order = await _paymentRepo.GetOrderWithShippingDetailsAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.OrderStatus == "Paid")
                throw new Exception("Order already paid");

            decimal totalAmount = order.TotalAmount + order.ShippingDetails.Sum(s => s.Cost);
            long stripeAmount = (long)(totalAmount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = stripeAmount,
                Currency = "inr",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return new PaymentIntentResponseDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            };
        }

        public Payments AddPayment(int orderId, string stripePaymentIntentId, string paymentMethod)
        {
            var order = _paymentRepo.GetOrderWithShippingDetailsAsync(orderId).Result;
            if (order == null)
                throw new Exception("Order not found");

            decimal totalAmount = order.TotalAmount + order.ShippingDetails.Sum(s => s.Cost);

            var payment = _paymentRepo.AddPayment(orderId, totalAmount, paymentMethod, stripePaymentIntentId);

            // Mark order as paid
            order.OrderStatus = "Paid";

            return payment;
        }



        //public Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod)
        //{
        //    return _paymentRepo.AddPayment(orderId, amountPaid, paymentMethod);
        //}

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
