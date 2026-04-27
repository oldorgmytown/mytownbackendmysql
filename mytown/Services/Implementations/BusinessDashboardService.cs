using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
//using mytown.Models.DTOs;
using mytown.Services.Interfaces;
using mytown.Services.Implementations;
using Microsoft.EntityFrameworkCore;

namespace mytown.Services.Implementations
{
    public class BusinessDashboardService : IBusinessDashboardService
    {
        private readonly IBusinessDashboardRepository _repository;
        private readonly IEmailService _emailService;

        public BusinessDashboardService(IBusinessDashboardRepository repository,IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService; // Consider using DI for better testability
        }

        public Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId, string? search, int pageNumber, int pageSize)
       => _repository.GetNewOrdersAsync(storeId, search, pageNumber, pageSize);

        public Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId, string? search, int pageNumber, int pageSize)
            => _repository.GetPendingOrdersAsync(storeId, search, pageNumber, pageSize);

        public Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId, string? search, int pageNumber, int pageSize)
            => _repository.GetInProgressOrdersAsync(storeId, search, pageNumber, pageSize);

        public Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId, string? search, int pageNumber, int pageSize)
            => _repository.GetCompletedOrdersAsync(storeId, search, pageNumber, pageSize);

        public async Task<BusinessOrderDetailsDto> GetBusinessOrderDetailsAsync(int storeOrderId)
        {
            return await _repository.GetBusinessOrderDetailsAsync(storeOrderId);
        }

        public async Task<List<BusinessProductDashboardDto>> GetProductsAsync(
            int storeId,
            string? search,
            int pageNumber,
            int pageSize)
        {
            return await _repository.GetProductsForDashboardAsync(storeId, search, pageNumber, pageSize);
        }
        public async Task<List<Sku_ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
        {
            return await _repository.GetVariantsByProductIdAsync(productId);
        }

        //get notifications to business dashboard

        public async Task<List<BusinessNotificationDto>> GetNotificationsAsync(
    int busRegId, bool onlyUnread)
        {
            var notifications = await _repository.GetNotificationsAsync(busRegId, onlyUnread);

            return notifications.Select(n => new BusinessNotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedDate
            }).ToList();
        }

        public async Task MarkAllAsReadAsync(int busRegId)
        {
            await _repository.MarkAllAsReadAsync(busRegId);
        }

        public async Task MarkeachNotificationAsReadAsync(int notificationId)
        {
            await _repository.MarkeachNotificationAsReadAsync(notificationId);
        }

        //Sales tab
        public async Task<List<Salestab_storeTransactionsDto>> GetStoreTransactionsAsync(
      int storeId,
      string? search,
      int pageNumber,
      int pageSize)
        {
            return await _repository.GetStoreTransactionsAsync(storeId, search, pageNumber, pageSize);
        }


        //transaction deatils
        public async Task<TransactionDetailsDto> GetTransactionDetailsAsync(int paymentId)
        {
            var transaction = await _repository.GetTransactionDetailsAsync(paymentId);

            if (transaction == null)
            {
                throw new Exception("Transaction not found");
            }

            return transaction;
        }


        //country wise sales

        public async Task<List<CountrySalesDto>> GetCountryWiseSalesAsync(int storeId)
        {
            return await _repository.GetCountryWiseSalesAsync(storeId);
        }

        //product wise sales
        public async Task<List<ProductSalesDto>> GetTopProductsAsync(int storeId, int topCount = 5)
        {
            return await _repository.GetTopProductsAsync(storeId, topCount);
        }


        //  Saving package details

        public async Task<ShippingPackageDetailsDto> SavePackageDetailsAsync(ShippingPackageDetailsDto dto)
        {
            var model = new ShippingPackageDetails
            {
                StoreOrderId = dto.StoreOrderId,

                PackageLength = dto.PackageLength,
                PackageWidth = dto.PackageWidth,
                PackageHeight = dto.PackageHeight,
                PackageWeight = dto.PackageWeight,

                DimensionUnit = dto.DimensionUnit ?? "cm",
                WeightUnit = dto.WeightUnit ?? "kg"
            };

            var result = await _repository.AddOrUpdateShippingPackageDetailsAsync(model);
            return new ShippingPackageDetailsDto
            {
                StoreOrderId = result.StoreOrderId,

                PackageLength = result.PackageLength,
                PackageWidth = result.PackageWidth,
                PackageHeight = result.PackageHeight,
                PackageWeight = result.PackageWeight,

                DimensionUnit = result.DimensionUnit,
                WeightUnit = result.WeightUnit
            };
        }

        public async Task<ShippingPackageDetailsDto?> GetPackageDetailsAsync(int storeOrderId)
        {
            var result = await _repository.GetShippingPackageDetailsByStoreOrderIdAsync(storeOrderId);

            if (result == null)
                return null;

            return new ShippingPackageDetailsDto
            {
                StoreOrderId = result.StoreOrderId,

                PackageLength = result.PackageLength,
                PackageWidth = result.PackageWidth,
                PackageHeight = result.PackageHeight,
                PackageWeight = result.PackageWeight,

                DimensionUnit = result.DimensionUnit,
                WeightUnit = result.WeightUnit,
                Notified = result.Notified
            };
        }

        //Notification and email  to coueir and transporter - Reday to ship 

        // Service
        public async Task MarkReadyToShipAsync(ReadyToShipDto dto)
        {
            int storeOrderId = dto.StoreOrderId;

            // 1️⃣ Update shipping status
            await _repository.UpdateShippingStatusAsync(
                storeOrderId,
                "Ready to Ship"
            );

            // 2️⃣ Update store order status
            await _repository.UpdateStoreOrderStatusAsync(
                storeOrderId,
                "Ready to Ship"
            );

            // 3️⃣ Get shipping details
            var shipping = await _repository.GetShippingByStoreOrderIdAsync(storeOrderId);
            if (shipping == null)
                return;

           

            // 5️⃣ Courier notification
            if (shipping.BranchId.HasValue && shipping.CourierBranch != null)
            {
                var notification = new CourierDBNotifications
                {
                    CourierId = shipping.CourierBranch.CourierId,
                    BranchId = shipping.BranchId.Value,
                    Title = "Order Ready to Ship",
                    Message = $"StoreOrder #{storeOrderId} is ready for pickup."
                };

                await _repository.AddCourierNotificationAsync(notification);
            }

            // 6️⃣ Transporter notification
            if (shipping.TransporterRegId.HasValue &&
                shipping.TransporterRegister != null)
            {
                var transporterNotification = new TransporterDBNotifications
                {
                    TransporterRegId = shipping.TransporterRegId.Value,
                    Title = "Order Ready to Ship",
                    Message = $"StoreOrder #{storeOrderId} is ready for pickup."
                };

                await _repository.AddTransporterNotificationAsync(
                    transporterNotification
                );
            }


            //  mark notified for thia package 
            await _repository.MarkPackageNotifiedByOrderIdAsync(storeOrderId);
            // 7️⃣ Save all DB changes once
            await _repository.SaveChangesAsync();

            // 8️⃣ Send emails
            var orderDetails = await _repository.GetBusinessOrderDetailsAsync(storeOrderId);
            string packageSummary =
                $"{dto.PackageLength} × {dto.PackageWidth} × {dto.PackageHeight} {dto.DimensionUnit}, " +
                $"{dto.PackageWeight} {dto.WeightUnit}";

            if (orderDetails != null &&
                shipping.BranchId.HasValue &&
                shipping.CourierBranch != null)
            {
                await _emailService.SendPackagerdyEmailToCourierAsync(
                    orderDetails.CourierEmail,
                    orderDetails.CourierServiceName,
                    orderDetails, packageSummary
                );
            }

            if (orderDetails != null &&
                shipping.TransporterRegId.HasValue &&
                shipping.TransporterRegister != null)
            {
                await _emailService.SendPackagerdyEmailToTransporterAsync(
                    orderDetails.TransporterEmail,
                    orderDetails.TransporterName,
                    orderDetails,packageSummary
                );
            }
        }
        //public async Task MarkReadyToShipAsync(int storeOrderId)
        //{
        //    // 1️⃣ Update shipping status
        //    await _repository.UpdateShippingStatusAsync(
        //        storeOrderId,
        //        "Ready to Ship"
        //    );

        //    // 2️⃣ Update store order status (recommended)
        //    await _repository.UpdateStoreOrderStatusAsync(
        //        storeOrderId,
        //        "Ready to Ship"
        //    );

        //    // 3️⃣ Get shipping to notify courier
        //    var shipping = await _repository.GetShippingByStoreOrderIdAsync(storeOrderId);
        //    if (shipping == null)
        //        return;

        //    // 4️⃣ Create courier notification
        //    var notification = new CourierDBNotifications
        //    {
        //        CourierId = shipping.CourierBranch.CourierId,
        //        BranchId = shipping.BranchId,
        //        Title = "Order Ready to Ship",
        //        Message = $"StoreOrder #{storeOrderId} is ready for pickup."
        //    };

        //    await _repository.AddCourierNotificationAsync(notification);

        //    // 5️⃣ Persist everything
        //    await _repository.SaveChangesAsync();
        //}


        // Get monthly Revenue for summary page

        public async Task<BusinessSalesSummaryDto> GetMonthlySalesAsync(
        int storeId, int? year, int? month, string? currency)
        {
            return await _repository.GetMonthlySalesAsync(storeId, year, month, currency);
        }

        // sales history

        public async Task<StoreSalesHistoryDto> GetSalesHistoryByStoreIdAsync(int storeId)
        {
            return await _repository.GetSalesHistoryByStoreIdAsync(storeId);
        }

        //sales trend graph

        public async Task<List<SalesTrendDto>> GetSalesTrendAsync(int storeId, DateTime? fromDate, DateTime? toDate)
        {
            return await _repository.GetSalesTrendAsync(storeId, fromDate, toDate);
        }
    }

    }




