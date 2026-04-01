using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;
using Stripe.Climate;

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

            //decimal totalAmount = order.TotalAmount + order.ShippingDetails.Sum(s => s.Cost);
            //long stripeAmount = (long)(totalAmount * 100);


            // 1️ Order Amount
            decimal orderAmount = order.TotalAmount;

            // 2️ Total Shipping Cost (all records for this order)
            decimal shippingTotal = order.ShippingDetails != null
                ? order.ShippingDetails.Sum(s => s.Cost)
                : 0;

            // 3️ Subtotal (Order + Shipping)
            decimal subTotal = orderAmount + shippingTotal;

            // 4️ 18% GST
            decimal gstAmount = subTotal * 0.18m;

            // 5️ Final Amount
            decimal finalAmount = subTotal + gstAmount;
            long stripeAmount = (long)(finalAmount * 100);
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

        public async Task<Payments> AddPaymentAsync(
     int orderId,
     string stripePaymentIntentId,
     string paymentMethod)
        {
            var order = await _paymentRepo.GetOrderWithShippingDetailsAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            // verify stripe payment first (important)
            //var service = new PaymentIntentService();
            //var intent = await service.GetAsync(stripePaymentIntentId);

            //if (intent.Status != "succeeded")
            //    throw new Exception("Payment not completed");

            // decimal totalAmount = order.TotalAmount + order.ShippingDetails.Sum(s => s.Cost);

            // 1️ Order Amount
            decimal orderAmount = order.TotalAmount;

            // 2️ Total Shipping Cost (all records for this order)
            decimal shippingTotal = order.ShippingDetails != null
                ? order.ShippingDetails.Sum(s => s.Cost)
                : 0;

            // 3️ Subtotal (Order + Shipping)
            decimal subTotal = orderAmount + shippingTotal;

            // 4️ 18% GST
            decimal gstAmount = subTotal * 0.18m;

            // 5️ Final Amount
            decimal finalAmount = subTotal + gstAmount;

            var payment = await _paymentRepo.AddPaymentAsync(
                orderId,
                finalAmount,
                paymentMethod,
                stripePaymentIntentId
            );

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

        //public async Task SendCourierEmailAsync(int branchId, int shippingDetailId)
        //{
        //    await _paymentRepo.SendEmailToCourier(branchId, shippingDetailId);
        //}

        public ShopperRegisterDto GetShopperDetailsByOrderId(int orderId)
        {
            return _paymentRepo.GetShopperDetailsByOrderId(orderId);
        }

public async Task ProcessPostPaymentAsync(int orderId)
{
    var shippingDetails = _paymentRepo.GetShippingDetailsByOrderId(orderId);

    var storeWiseShipments = shippingDetails
        .GroupBy(s => s.StoreOrderId)
        .Select(g => g.First())
        .ToList();

    foreach (var shipping in storeWiseShipments)
    {
        if (shipping.ShippingType?.Trim().ToLower() == "p2p")
        {
            // ✅ Read TransporterRegId directly from 
            if (!shipping.TransporterRegId.HasValue || shipping.TransporterRegId <= 0)
                continue;

            int transporterRegId = shipping.TransporterRegId.Value;

            var storeOrder = await _paymentRepo.GetStoreOrderByIdAsync(shipping.StoreOrderId);
            if (storeOrder == null) continue;

            await _paymentRepo.CreateP2PDeliveryRequestAsync(new CreateP2PDeliveryRequestDto
            {
                PlanId = shipping.TransporterPlanId ?? 0,
                TransporterRegId = transporterRegId,
                ShopperRegId     = storeOrder.Order.ShopperRegId,
                OrderId          = orderId,
                StoreOrderId     = shipping.StoreOrderId,
                PickupLocation   = await _paymentRepo.GetStoreAddressAsync(storeOrder.StoreId),
                DropoffLocation  = shipping.DeliveryAddress,
                PackageWeightKg  = await _paymentRepo.GetStoreOrderWeightAsync(shipping.StoreOrderId),
                NumberOfPackages = await _paymentRepo.GetStoreOrderItemCountAsync(shipping.StoreOrderId),
                DeliveryFee      = shipping.Cost,
                PackageTags      = "NA"
            });

            await _paymentRepo.CreateTransporterNotificationAsync(
                transporterRegId: transporterRegId,
                title: "New P2P Delivery Request",
                message: $"You have a new delivery request for Order #{orderId}. Please check your dashboard to accept."
            );

            await _paymentRepo.UpdateOrderStatusAsync(orderId, "Paid");
        }
        else
        {
            var courierId = await _paymentRepo.GetCourierIdByBranchIdAsync(shipping.BranchId);

            await AddCourierNotificationAsync(
                courierId: courierId,
                branchId: shipping.BranchId,
                title: "New Order Assigned",
                message: $"StoreOrder #{shipping.StoreOrderId} needs to be shipped."
            );
        }
    }
}

        public async Task AddCourierNotificationAsync(
    int courierId,
    int branchId,
    string title,
    string message)
        {
            var notification = new CourierDBNotifications
            {
                CourierId = courierId,
                BranchId = branchId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            };

           await _paymentRepo.AddCourierNotificationAsync(notification);
           // await _paymentRepo.SaveChangesAsync();
        }

    }
}
