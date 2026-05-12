using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mytown.DataAccess.Interfaces;
using mytown.DTOs;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;
using System.Text.Json;
using static mytown.Services.Implementations.SenderService;

namespace mytown.Services.Implementations
{
    public class SenderService : ISenderService
    {
        private readonly ISenderRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SenderService> _logger;
        private readonly IVerficationLinkBuildersender _verificationLinkBuilder;

        public SenderService(
            ISenderRepository repo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<SenderService> logger,
            IVerficationLinkBuildersender verificationLinkBuilder)
        {
            _repo = repo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _verificationLinkBuilder = verificationLinkBuilder;
        }

        // ---------------- REGISTER ----------------
        public async Task<(bool success, string message)> RegisterSenderAsync(SenderRegisterDto dto)
        {
            var (isTaken, statusMessage) = await _repo.IsEmailTaken(dto.Email);

            if (statusMessage != null)
                return (false, statusMessage);

            if (isTaken)
                return (false, "This email is already registered.");

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            var pending = new PendingSenderVerification
            {
                Email = dto.Email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = JsonSerializer.Serialize(dto)
            };

            await _repo.SavePendingSenderVerification(pending);
            await _emailService.SendVerificationEmail(dto.Email, link);

            return (true, "Verification email sent.");
        }

        // ---------------- VERIFY EMAIL ----------------
        public async Task<(bool success, string message, int? senderRegId)> VerifyEmailAsync(string token)
        {
            var pending = await _repo.FindPendingSenderVerificationByToken(token);

            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                return (false, "Invalid or expired verification link.", null);

            var dto = JsonSerializer.Deserialize<SenderRegisterDto>(pending.JsonPayload);

            var sender = new SenderRegister
            {
                SenderName = dto.SenderName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Address = dto.Address,
                Town = dto.Town,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                PhoneNumber = dto.PhoneNumber,
                IsEmailVerified = true,
                Status = "Active"
            };

            await _repo.RegisterSender(sender);
            await _repo.DeletePendingSenderVerification(token);

            return (true, "Email verified successfully.", sender.SenderRegId);
        }

        // ---------------- RESEND EMAIL ----------------
        public async Task<(bool success, string message)> ResendVerificationEmailAsync(string email)
        {
            var existing = await _repo.FindPendingSenderVerificationByEmail(email);

            if (existing == null)
                return (false, "No pending verification found.");

            await _repo.DeletePendingSenderVerification(existing.Token);

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            var pending = new PendingSenderVerification
            {
                Email = email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = existing.JsonPayload
            };

            await _repo.SavePendingSenderVerification(pending);

            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            await _emailService.SendVerificationEmail(email, link);

            return (true, "Verification email resent.");
        }

        // ---------------- SENDER ORDERS ----------------

        public async Task<int> CreateSenderOrderAsync(CreateSenderOrderDto dto)
        {
            return await _repo.CreateSenderOrderAsync(dto);
        }

        /// ---------------- MATCHING TRANSPORTERS ----------------
        public async Task<MatchingTransporterDto>
    GetMatchingTransportersAsync(int senderOrderId)
        {
            return await _repo
                .GetMatchingTransportersAsync(senderOrderId);
        }

        // odrer summary

        public async Task<SenderOrderSummaryDto>
    GetOrderSummaryAsync(
        SenderOrderSummaryRequestDto dto)
        {
            return await _repo
                .GetOrderSummaryAsync(dto);
        }

   

     

        // sender payment

        public async Task
           <SenderPaymentIntentResponseDto>
           CreatePaymentIntentAsync(
               int senderOrderId)
        {
            var order =
                await _repo
                .GetSenderOrderAsync(
                    senderOrderId);

            if (order == null)
                throw new Exception(
                    "Order not found");

            decimal amount = 50;

            decimal gstAmount =
                amount * 0.18m;

            decimal totalAmount =
                amount + gstAmount;

            long stripeAmount =
                (long)(totalAmount * 100);

            var options =
                new PaymentIntentCreateOptions
                {
                    Amount = stripeAmount,

                    Currency = "inr",

                    AutomaticPaymentMethods =
                        new PaymentIntentAutomaticPaymentMethodsOptions
                        {
                            Enabled = true
                        },

                    Metadata =
                        new Dictionary<string, string>
                        {
                            {
                                "senderOrderId",
                                senderOrderId
                                .ToString()
                            }
                        }
                };

            var stripeSecretKey = _configuration["Stripe:SecretKey"];
var stripeClient = new StripeClient(stripeSecretKey);
var service = new PaymentIntentService(stripeClient);

            var paymentIntent =
                await service
                .CreateAsync(options);

            return new
                SenderPaymentIntentResponseDto
            {
                ClientSecret =
                    paymentIntent.ClientSecret,

                PaymentIntentId =
                    paymentIntent.Id
            };
        }

        public async Task<bool>
            ConfirmPaymentAsync(
                ConfirmSenderPaymentDto dto)
        {
            var order =
                await _repo
                .GetSenderOrderAsync(
                    dto.SenderOrderId);

            if (order == null)
                throw new Exception(
                    "Order not found");

            decimal amount = 50;

            decimal gstAmount =
                amount * 0.18m;

            decimal totalAmount =
                amount + gstAmount;

            var payment =
                new SenderOrderPayment
                {
                    SenderOrderId =
                        dto.SenderOrderId,

                    StripePaymentIntentId =
                        dto.StripePaymentIntentId,

                    Amount =
                        amount,

                    GstAmount =
                        gstAmount,

                    TotalAmount =
                        totalAmount,

                    PaymentMethod =
                        dto.PaymentMethod,

                    PaymentStatus =
                        "Paid",

                    PaidAt =
                        DateTime.UtcNow
                };

await _repo
                .AddSenderOrderPaymentAsync(
                    payment);

            order.TransporterRegId = dto.TransporterRegId;
            order.TransporterPlanId = dto.TransporterPlanId;
            order.OrderStatus = "Booked";

            await _repo
                .SaveChangesAsync();

            return true;
        }

        // sender order confirmation

        public async Task
     <SenderOrderConfirmationDto>
     GetOrderConfirmationAsync(
         int senderOrderId)
        {
            var result =
                await _repo
                .GetOrderConfirmationAsync(
                    senderOrderId);

            var order =
                await _repo
                .GetSenderOrderAsync(
                    senderOrderId);

            if (order == null)
                throw new Exception(
                    "Order not found");

            if (!order.TransporterRegId.HasValue)
                throw new Exception(
                    "Transporter not assigned");

            // Notify Sender

            await _repo.AddSenderNotificationAsync(
                new SenderDBNotifications
                {
                    SenderRegId =
                        order.SenderRegId,

                    Title =
                        "Booking Confirmed",

                    Message =
                        $"Your shipment #{order.SenderOrderId} has been booked successfully.",

                    IsRead = false,

                    CreatedDate =
                        DateTime.UtcNow
                });

            // Notify Transporter

            await _repo.AddTransporterNotificationAsync(
                new TransporterDBNotifications
                {
                    TransporterRegId =
                        order.TransporterRegId.Value,

                    Title =
                        "New Shipment Assigned",

                    Message =
                        $"New shipment #{order.SenderOrderId} has been assigned to you.",

                    IsRead = false,

                    CreatedDate =
                        DateTime.UtcNow
                });

            await _repo.SaveChangesAsync();


            // EMAIL TRIGGER
            try
            {
                var sender =
                    await _repo.GetSenderByIdAsync(order.SenderRegId);

                var transporter =
                    await _repo.GetTransporterByIdAsync(
                        order.TransporterRegId.Value);

                // Sender mail
                await _emailService.SendSenderOrderConfirmationAsync(
                    sender.Email,
                    sender.SenderName,
                    result
                );

                // Transporter mail
                await _emailService.SendTransporterAssignmentAsync(
                    transporter.Email,
                    transporter.TransporterName,
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
       ex,
       "Failed to send order confirmation emails for SenderOrderId: {SenderOrderId}",
       senderOrderId
   );
            }

            return result;
        }

        // sender package delivery status

        public async Task<bool>
    UpdateSenderPackageDeliveryStatusAsync(
        UpdateSenderPackageDeliveryStatusDto dto)
        {
            return await _repo
                .UpdateSenderPackageDeliveryStatusAsync(dto);
        }

       

            public async Task<List<SenderOrdersTabDto>>
            GetSenderOrdersAsync(
                int senderId,
                string orderType)
            {
                return await _repo
                    .GetSenderOrdersAsync(
                        senderId,
                        orderType);
            }

        public async Task<SenderRegisterDto?> GetSenderProfileAsync(int senderRegId)
        {
            return await _repo.GetSenderProfileAsync(senderRegId);
        }

        public async Task<bool> UpdateSenderProfileAsync(
    int senderRegId,
    UpdateSenderProfileDto dto)
        {
            return await _repo.UpdateSenderProfileAsync(
                senderRegId,
                dto);
        }

        public async Task<List<SenderDBNotifications>>
GetUnreadNotificationsAsync(int senderId)
    => await _repo.GetUnreadNotificationsAsync(senderId);

        public async Task MarkAsReadAsync(int senderId)
            => await _repo.MarkAllAsReadAsync(senderId);

        public async Task MarkEachNotificationReadAsync(int notificationId)
            => await _repo.MarkEachNotificationReadAsync(notificationId);

        // ---------------- ALTERNATE ADDRESS ----------------

        public Task<IEnumerable<SenderAlternateAddressDto>>
        GetAddressesAsync(int senderRegId)
            => _repo.GetAddressesBySenderIdAsync(senderRegId);

        public async Task<SenderAlternateAddressDto>
        AddAddressAsync(SenderAlternateAddressDto dto)
        {
            var entity = new SenderAlternateAddress
            {
                AltAddressId = dto.AltAddressId,
                SenderRegId = dto.SenderRegId,
                AltName = dto.AltName,
                AltPhoneNumber = dto.AltPhoneNumber,
                AltAddress = dto.AltAddress,
                AltTown = dto.AltTown,
                AltCity = dto.AltCity,
                AltState = dto.AltState,
                AltCountry = dto.AltCountry,
                AltPostalCode = dto.AltPostalCode,
                DeliveryNotes = dto.DeliveryNotes
            };

            return await _repo.AddAddressAsync(entity);
        }

        public Task<bool>
        DeleteAddressAsync(int id)
            => _repo.DeleteAddressAsync(id);

    }
}