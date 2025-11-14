using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Services;
using Stripe;

namespace mytown.Controllers
{
    [Authorize]
    [Route("api/shoppingcart/payment")]
    [ApiController]
    public class PaymentController: ControllerBase
    {
        private readonly IPaymentRepository _paymentRepo;

        private readonly ILogger<OrderController> _logger;
        private readonly IEmailService _emailService;

        public PaymentController(IPaymentRepository paymentRepo,
                                ILogger<OrderController> logger, IEmailService emailService)
        {
            _paymentRepo = paymentRepo ?? throw new ArgumentNullException(nameof(paymentRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(_emailService));
        }
        [HttpPost("AddPayment")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentRequestModel model)
        {
            if (model == null || model.OrderId <= 0 || model.AmountPaid <= 0 || string.IsNullOrEmpty(model.PaymentMethod))
            {
                return BadRequest("Invalid payment details.");
            }

            var payment = _paymentRepo.AddPayment(model.OrderId, model.AmountPaid, model.PaymentMethod);

            var storeDetails = _paymentRepo.GetStoreDetailsByOrderId(model.OrderId);

            // Send email to each store
            foreach (var store in storeDetails)
            {
                if (!string.IsNullOrEmpty(store.BusEmail))
                {
                    await _emailService.SendBusinessnotification(store.BusEmail, store.Businessname,model.OrderId);
                }
            }

            // ✅ Get ShippingDetails for the order and notify couriers
            var shippingDetails = _paymentRepo.GetShippingDetailsByOrderId(model.OrderId);

            foreach (var shipping in shippingDetails)
            {
                //email to courier main with the branch id
                await _paymentRepo.SendEmailToCourier(shipping.BranchId, shipping.ShippingDetailId);
            }
            // 4️⃣ Notify shopper
            var shopper = _paymentRepo.GetShopperDetailsByOrderId(model.OrderId);
            if (shopper != null && !string.IsNullOrEmpty(shopper.Email))
            {
                await _emailService.SendShopperNotification(shopper.Email, shopper.Username, model.OrderId, model.AmountPaid);
            }

            return Ok(new { message = "Payment successful!", paymentId = payment.PaymentId });
        }

        

        private string GetCurrencyFromCountry(string countryName)
        {
            // Example: Map country name to currency code
            var countryCurrencyMapping = new Dictionary<string, string>
    {
        { "United States", "usd" },  // Country -> Currency code
        { "India", "inr" },
        { "United Kingdom", "gbp" },
        { "European Union", "eur" },
        { "Japan", "jpy" },
        // Add other countries and currencies as needed
    };

            // Return the currency code if found, otherwise return null
            if (countryCurrencyMapping.ContainsKey(countryName))
            {
                return countryCurrencyMapping[countryName];
            }

            return null;  // Return null if no valid currency is found
        }
        [HttpPost("create-payment-intent")]
        public ActionResult CreatePaymentIntent([FromBody] PaymentRequestDto paymentRequest)
        {
            try
            {
                //removed stripe link as its creating git push issues
               
                // Get the currency code based on the country name
                string currency = GetCurrencyFromCountry(paymentRequest.CountryName);

                // If no valid currency is found, default to USD
                if (currency == null)
                {
                    currency = "usd";
                }

                // Create the PaymentIntent options
                var options = new PaymentIntentCreateOptions
                {
                    Amount = paymentRequest.Amount,  // Amount in cents (e.g., $10 = 1000)
                    Currency = currency,             // Currency based on country name
                    PaymentMethodTypes = new List<string> { "card" },
                };

                // Create the payment intent using Stripe's service
                //var service = new PaymentIntentService();
                //PaymentIntent intent = service.Create(options);

                // Return the client secret to the frontend for confirming payment
                //return Ok(new { clientSecret = intent.ClientSecret });
                return Ok(new { clientSecret = "pi-secrect" });
            }
            catch (StripeException e)
            {
                // Return error if Stripe API call fails
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
