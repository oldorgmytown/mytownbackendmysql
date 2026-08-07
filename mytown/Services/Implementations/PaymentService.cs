using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;
using Stripe.Climate;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;


namespace mytown.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly string _razorpayKeyId;
        private readonly string _razorpayKeySecret;

        public PaymentService(IPaymentRepository paymentRepo, IConfiguration configuration)
        {
            _paymentRepo = paymentRepo;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            _razorpayKeyId = configuration["Razorpay:KeyId"];
            _razorpayKeySecret = configuration["Razorpay:KeySecret"];

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
            decimal gstAmount = orderAmount * 0;//( made 18% into 0% for testing purpose) 0.18m;

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
            var service = new PaymentIntentService();
            var intent = await service.GetAsync(stripePaymentIntentId);

            if (intent.Status != "succeeded")
                throw new Exception("Payment not completed");

            // Check if this Payment Intent was already processed
            var existingPayment = await _paymentRepo.GetPaymentByStripePaymentIntentId(stripePaymentIntentId);

            if (existingPayment != null)
                throw new Exception("Payment has already been processed.");

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
            decimal gstAmount = orderAmount * 0;//( made 18% into 0% for testing purpose) 0.18m;

            // 5️ Final Amount
            decimal finalAmount = subTotal + gstAmount;

            var payment = await _paymentRepo.AddPaymentAsync(
                orderId,
                finalAmount,
                paymentMethod,
                stripePaymentIntentId
            );

            order.OrderStatus = "Paid";

            //  Update cart status after successful payment
            await _paymentRepo.UpdateCartStatusAsync(orderId);

            return payment;
        }

        //--------Razor Pay Payments--------------------//

     

    public async Task<RazorpayOrderResponseDto> CreateRazorpayOrderAsync(int orderId)
    {
        var order = await _paymentRepo.GetOrderWithShippingDetailsAsync(orderId);
        if (order == null)
            throw new Exception("Order not found");

        if (order.OrderStatus == "Paid")
            throw new Exception("Order already paid");

        decimal orderAmount = order.TotalAmount;
        decimal shippingTotal = order.ShippingDetails != null
            ? order.ShippingDetails.Sum(s => s.Cost)
            : 0;
        decimal subTotal = orderAmount + shippingTotal;
        decimal gstAmount = orderAmount * 0.18m;
        decimal finalAmount = subTotal + gstAmount;

        // Razorpay also expects amount in paise
        int amountInPaise = (int)(finalAmount * 100);

        RazorpayClient client = new RazorpayClient(_razorpayKeyId, _razorpayKeySecret);

        Dictionary<string, object> options = new Dictionary<string, object>
    {
        { "amount", amountInPaise },
        { "currency", "INR" },
        { "receipt", $"order_rcpt_{orderId}" },
        { "payment_capture", 1 } // auto-capture
    };

            Razorpay.Api.Order razorpayOrder = client.Order.Create(options);

        return new RazorpayOrderResponseDto
        {
            RazorpayOrderId = razorpayOrder["id"].ToString(),
            Amount = amountInPaise,
            Currency = "INR",
            KeyId = _razorpayKeyId  // frontend needs this to open checkout
        };
    }

 
public async Task<Payments> AddRazorpayPaymentAsync(
    int orderId,
    string razorpayOrderId,
    string razorpayPaymentId,
    string razorpaySignature)
    {
        var order = await _paymentRepo.GetOrderWithShippingDetailsAsync(orderId);
        if (order == null)
            throw new Exception("Order not found");

        // Check for replay/double processing
        var existingPayment = await _paymentRepo.GetPaymentByStripePaymentIntentId(razorpayPaymentId);
        if (existingPayment != null)
            throw new Exception("Payment has already been processed.");

        // Verify signature: HMAC-SHA256 of "order_id|payment_id" using your Key Secret
        string payload = $"{razorpayOrderId}|{razorpayPaymentId}";
        string generatedSignature;

        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_razorpayKeySecret)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        if (generatedSignature != razorpaySignature)
            throw new Exception("Payment signature verification failed.");

        decimal orderAmount = order.TotalAmount;
        decimal shippingTotal = order.ShippingDetails != null
            ? order.ShippingDetails.Sum(s => s.Cost)
            : 0;
        decimal subTotal = orderAmount + shippingTotal;
        decimal gstAmount = orderAmount * 0.18m;
        decimal finalAmount = subTotal + gstAmount;

        var payment = await _paymentRepo.AddPaymentAsync(
            orderId,
            finalAmount,
            "UPI", // or pass actual method from Razorpay response
            razorpayPaymentId
        );

        order.OrderStatus = "Paid";
        await _paymentRepo.UpdateCartStatusAsync(orderId);

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
                ShopperRegId = storeOrder.Order.ShopperRegId,
                GuestRegId = storeOrder.Order.GuestRegId,
                IsGuestOrder = storeOrder.Order.IsGuestOrder,
                OrderId          = orderId,
                StoreOrderId     = shipping.StoreOrderId,
                PickupLocation   = await _paymentRepo.GetStoreAddressAsync(storeOrder.StoreId),
                DropoffLocation  = shipping.DeliveryAddress,
                PackageWeightKg  = await _paymentRepo.GetStoreOrderWeightAsync(shipping.StoreOrderId),
                NumberOfPackages = await _paymentRepo.GetStoreOrderItemCountAsync(shipping.StoreOrderId),
                DeliveryFee      = shipping.Cost,
                PackageTags      = "NA"
            });
 
            // ✅ Notify transporter
            await CreateTransporterNotificationAsync(
                transporterRegId: transporterRegId,
                title: "New P2P Delivery Request",
                message: $"A new delivery for Order #{orderId} has been assigned to you. Check your dashboard."
            );

            await _paymentRepo.UpdateOrderStatusAsync(orderId, "Paid");
        }
        else
        {
                    if (shipping.BranchId.HasValue)
                    {
                        var courierId = await _paymentRepo
                            .GetCourierIdByBranchIdAsync(shipping.BranchId.Value);

                        await AddCourierNotificationAsync(
                            courierId: courierId,
                            branchId: shipping.BranchId.Value,
                            title: "New Order Assigned",
                            message: $"StoreOrder #{shipping.StoreOrderId} needs to be shipped."
                        );
                    }
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


        public async Task CreateTransporterNotificationAsync(
   int transporterRegId,
   string title,
   string message)
        {
            var notification = new TransporterDBNotifications
            {
                TransporterRegId = transporterRegId,
                //BranchId = branchId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            };

            await _paymentRepo.AddTransporterNotificationAsync(notification);
            // await _paymentRepo.SaveChangesAsync();
        }

    }
}
