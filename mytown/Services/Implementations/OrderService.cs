using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IEmailService _EmailService;

        public OrderService(IOrderRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _EmailService = emailService;
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

            // 2️⃣ Send email using data already present in DTO
            if (!string.IsNullOrEmpty(orderConfirmation.ShopperEmail))
            {
                await _EmailService.SendShopperNotification(
                    orderConfirmation.ShopperEmail,
                    orderConfirmation.ShopperName,
                    orderConfirmation
                );
            }

            foreach (var store in orderConfirmation.Stores)
            {
                if (!string.IsNullOrEmpty(store.BusinessEmail))
                {
                    await _EmailService.SendBusinessnotificationforOrderCnf(
                        store.BusinessEmail,
                        store.StoreName,
                        orderConfirmation,
                        store
                    );
                }
                // Courier
                if (!string.IsNullOrEmpty(store.CourierEmail))
                {
                    await _EmailService.SendEmailToCourierAsync(
                        store.CourierEmail,
                        store.CourierName,
                        orderConfirmation,
                        store
                    );
                }

                // Transporter
                if (!string.IsNullOrEmpty(store.TransporterEmail))
                {
                    await _EmailService.SendEmailToTransporterAsync(
                        store.TransporterEmail,
                        store.TransporterName,
                        orderConfirmation,
                        store
                    );
                }

            }
            //  Return DTO to controller
            return orderConfirmation;
        }

        public async Task<OrderConfirmationDto> GetOrderConfirmationforOrderHistoryAsync(int orderId)
        {
            // 1️⃣ Get everything from repository
            var orderConfirmation = await _repo.GetOrderConfirmationAsync(orderId);
            
            return orderConfirmation;
        }


    }
}

