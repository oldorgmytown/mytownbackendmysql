using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IEmailService _EmailService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderService> _logger;


        public OrderService(IOrderRepository repo, IEmailService emailService, IServiceScopeFactory scopeFactory,
                    ILogger<OrderService> logger)
        {
            _repo = repo;
            _EmailService = emailService;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        //public Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> selections)
        //    => _repo.CreateOrderAsync(shopperRegId, selections);


        // public async Task<int> CreateOrderAsync(
        //int shopperRegId,
        //int? selectedAltAddressId,
        //List<StoreShippingSelection> shippingSelections)
        // {
        //     return await _repo.CreateOrderAsync(
        //         shopperRegId,
        //         selectedAltAddressId,
        //         shippingSelections
        //     );
        // }

        public async Task<int> CreateOrderAsync(CreateOrderRequestddto request)
        {
            return await _repo.CreateOrderAsync(request);
        }
        public Task<int> CreateOrderAndOrderDetailsAsync(int shopperRegId)
            => _repo.CreateOrderAndOrderDetailsAsync(shopperRegId);

        public Task SaveShippingSelectionsAsync(int orderId, List<StoreShippingSelection> selections)
            => _repo.SaveShippingSelectionsAsync(orderId, selections);


        //public async Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId)
        //{
        //    return await _repo.GetOrderConfirmationAsync(orderId);
        //}

        public async Task<OrderConfirmationDto> GetOrderConfirmationAsync(int orderId)
        {
            // 1️⃣ Get everything from repository
            var orderConfirmation = await _repo.GetOrderConfirmationAsync(orderId);

            if (orderConfirmation == null)
                return null;

            //reduce stock for each product in the order
            await _repo.ReduceStockAsync(orderId);
            // Create notification ONLY for registered shoppers
            if (!orderConfirmation.IsGuestOrder &&
                orderConfirmation.ShopperRegId.HasValue)
            {
                var shopperNotification = new ShopperDBNotifications
                {
                    ShopperRegId = orderConfirmation.ShopperRegId.Value,
                    Title = "Order Confirmation",
                    Message = $"Your order #{orderId} has been successfully placed.",
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow
                };

                await _repo.AddShopperNotificationAsync(shopperNotification);
            }

            // Fire emails in the background, then return immediately
            _ = Task.Run(() => SendOrderEmailsAsync(orderConfirmation, orderId));

            return orderConfirmation;
        }

        private async Task SendOrderEmailsAsync(OrderConfirmationDto orderConfirmation, int orderId)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>(); // use your real interface/class name

                if (!string.IsNullOrEmpty(orderConfirmation.ShopperEmail))
                {
                    await emailService.SendShopperNotification(
                        orderConfirmation.ShopperEmail,
                        orderConfirmation.ShopperName,
                        orderConfirmation);
                }

                foreach (var store in orderConfirmation.Stores)
                {
                    if (!string.IsNullOrEmpty(store.BusinessEmail))
                        await emailService.SendBusinessnotificationforOrderCnf(
                            store.BusinessEmail, store.StoreName, orderConfirmation, store);

                    if (!string.IsNullOrEmpty(store.CourierEmail))
                        await emailService.SendEmailToCourierAsync(
                            store.CourierEmail, store.CourierName, orderConfirmation, store);

                    if (!string.IsNullOrEmpty(store.TransporterEmail))
                        await emailService.SendEmailToTransporterAsync(
                            store.TransporterEmail, store.TransporterName, orderConfirmation, store);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed sending emails for order {OrderId}", orderId);
            }
        }

        //    // Send confirmation email for BOTH Shopper and Guest
        //    if (!string.IsNullOrEmpty(orderConfirmation.ShopperEmail))
        //    {
        //        await _EmailService.SendShopperNotification(
        //            orderConfirmation.ShopperEmail,
        //            orderConfirmation.ShopperName,
        //            orderConfirmation
        //        );
        //    }

        //    foreach (var store in orderConfirmation.Stores)
        //    {
        //        if (!string.IsNullOrEmpty(store.BusinessEmail))
        //        {
        //            await _EmailService.SendBusinessnotificationforOrderCnf(
        //                store.BusinessEmail,
        //                store.StoreName,
        //                orderConfirmation,
        //                store
        //            );
        //        }
        //        // Courier
        //        if (!string.IsNullOrEmpty(store.CourierEmail))
        //        {
        //            await _EmailService.SendEmailToCourierAsync(
        //                store.CourierEmail,
        //                store.CourierName,
        //                orderConfirmation,
        //                store
        //            );
        //        }

        //        // Transporter
        //        if (!string.IsNullOrEmpty(store.TransporterEmail))
        //        {
        //            await _EmailService.SendEmailToTransporterAsync(
        //                store.TransporterEmail,
        //                store.TransporterName,
        //                orderConfirmation,
        //                store
        //            );
        //        }

        //    }
        //    //  Return DTO to controller
        //    return orderConfirmation;
        //}

        public async Task<OrderConfirmationDto> GetOrderConfirmationforOrderHistoryAsync(int orderId)
        {
            // 1️⃣ Get everything from repository
            var orderConfirmation = await _repo.GetOrderConfirmationAsync(orderId);
            
            return orderConfirmation;
        }


    }
}

