using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.DTOs;
using BusinessNotificationDto = mytown.Models.DTO_s.BusinessNotificationDto;

namespace mytown.Services.Interfaces
{
    public interface IBusinessDashboardService
    {
        Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId);

        Task<List<BusinessProductDashboardDto>> GetProductsAsync(int storeId);

        Task<BusinessOrderDetailsDto> GetBusinessOrderDetailsAsync(int storeOrderId);

        // get variant details

        Task<List<Sku_ProductVariantDto>> GetVariantsByProductIdAsync(int productId);

        Task<List<BusinessNotificationDto>> GetNotificationsAsync(
       int busRegId,
       bool onlyUnread
   );

        Task MarkAllAsReadAsync(int busRegId);
        //  Task GetNotificationsAsync(int busRegId, bool onlyUnread);

        //Sales tab - transaction deatils

        Task<List<Salestab_storeTransactionsDto>> GetStoreTransactionsAsync(int storeId);

        // country wise sales

        Task<List<CountrySalesDto>> GetCountryWiseSalesAsync(int storeId);

        //product wise sales

        Task<List<ProductSalesDto>> GetTopProductsAsync(int storeId, int topCount = 5);

        //notification to coueir - ready to ship

        Task MarkReadyToShipAsync(int storeOrderId);


    }
}
