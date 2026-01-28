using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;
//using mytown.Models.DTOs;
using mytown.Services.Interfaces;

namespace mytown.Services.Implementations
{
    public class BusinessDashboardService : IBusinessDashboardService
    {
        private readonly IBusinessDashboardRepository _repository;

        public BusinessDashboardService(IBusinessDashboardRepository repository)
        {
            _repository = repository;
        }

        public Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId)
            => _repository.GetNewOrdersAsync(storeId);

        public Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId)
            => _repository.GetPendingOrdersAsync(storeId);

        public Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId)
            => _repository.GetInProgressOrdersAsync(storeId);

        public Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId)
            => _repository.GetCompletedOrdersAsync(storeId);

        public async Task<BusinessOrderDetailsDto> GetBusinessOrderDetailsAsync(int storeOrderId)
        {
            // Simply call repository
            var details = await _repository.GetBusinessOrderDetailsAsync(storeOrderId);
            return details;
        }

        public async Task<List<BusinessProductDashboardDto>> GetProductsAsync(int storeId)
        {
            return await _repository.GetProductsForDashboardAsync(storeId);
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

        //Sales tab
        public async Task<List<Salestab_storeTransactionsDto>> GetStoreTransactionsAsync(int storeId)
        {
            return await _repository.GetStoreTransactionsAsync(storeId);
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
    }


}




