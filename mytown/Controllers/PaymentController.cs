using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Services.Interfaces;
using Stripe;

namespace mytown.Controllers
{
    [Authorize]
    [Route("api/shoppingcart/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentController(
            IPaymentService paymentService,
            ILogger<OrderController> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _paymentService = paymentService;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
        }

        //[HttpPost("AddPayment")]
        //public async Task<IActionResult> AddPayment([FromBody] PaymentRequestModel model)
        //{
        //    if (model == null || model.OrderId <= 0 || model.AmountPaid <= 0 || string.IsNullOrEmpty(model.PaymentMethod))
        //    {
        //        return BadRequest("Invalid payment details.");
        //    }

        //    var payment = _paymentService.AddPayment(model.OrderId, model.AmountPaid, model.PaymentMethod);

        //    //// STORE DETAILS
        //    //var storeDetails = _paymentService.GetStoreDetailsByOrderId(model.OrderId);

        //    //foreach (var store in storeDetails)
        //    //{
        //    //    if (!string.IsNullOrEmpty(store.BusEmail))
        //    //    {
        //    //        await _emailService.SendBusinessnotification(store.BusEmail, store.Businessname, model.OrderId);
        //    //    }
        //    //}

        //    // SHIPPING DETAILS
        //    var shippingDetails = _paymentService.GetShippingDetailsByOrderId(model.OrderId);

        //    foreach (var shipping in shippingDetails)
        //    {
        //        await _paymentService.SendCourierEmailAsync(shipping.BranchId, shipping.ShippingDetailId);
        //    }

        //    //// SHOPPER DETAILS
        //    //var shopper = _paymentService.GetShopperDetailsByOrderId(model.OrderId);

        //    //if (shopper != null && !string.IsNullOrEmpty(shopper.Email))
        //    //{
        //    //    await _emailService.SendShopperNotification(
        //    //        shopper.Email,
        //    //        shopper.Username,
        //    //        model.OrderId,
        //    //        model.AmountPaid
        //    //    );
        //    //}

        //    return Ok(new { message = "Payment successful!", paymentId = payment.PaymentId });
        //}



        // ------------------ Payment Intent ------------------------

        private string GetCurrencyFromCountry(string countryName)
        {
            var countryCurrencyMapping = new Dictionary<string, string>
            {
                { "United States", "usd" },
                { "India", "inr" },
                { "United Kingdom", "gbp" },
                { "European Union", "eur" },
                { "Japan", "jpy" }
            };

            return countryCurrencyMapping.ContainsKey(countryName)
                ? countryCurrencyMapping[countryName]
                : null;
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(CreatePaymentIntentRequestDto request)
        {
            try
            {
                var result = await _paymentService.CreatePaymentIntentAsync(request.OrderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("add-payment")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentRequestModel model)
        {
            if (model == null || model.OrderId <= 0 || string.IsNullOrEmpty(model.PaymentMethod) || string.IsNullOrEmpty(model.StripePaymentIntentId))
                return BadRequest("Invalid payment details.");

            var payment = _paymentService.AddPayment(model.OrderId, model.StripePaymentIntentId, model.PaymentMethod);

            // Shipping emails
            var shippingDetails = _paymentService.GetShippingDetailsByOrderId(model.OrderId);
            foreach (var shipping in shippingDetails)
            {
                await _paymentService.SendCourierEmailAsync(shipping.BranchId, shipping.ShippingDetailId);
            }

            return Ok(new { message = "Payment successful!", paymentId = payment.PaymentId });
        }

    }
}
