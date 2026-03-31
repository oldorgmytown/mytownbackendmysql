using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.DTOs;
using BusinessNotificationDto = mytown.Models.DTO_s.BusinessNotificationDto;

namespace mytown.Services.Interfaces
{
    public interface IBusinessDashboardService
    {
        Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId, string? search, int pageNumber, int pageSize);

        Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId, string? search, int pageNumber, int pageSize);

        Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId, string? search, int pageNumber, int pageSize);

        Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId, string? search, int pageNumber, int pageSize);

        Task<BusinessOrderDetailsDto> GetBusinessOrderDetailsAsync(int storeOrderId);

        Task<List<BusinessProductDashboardDto>> GetProductsAsync(
            int storeId,
            string? search,
            int pageNumber,
            int pageSize);
        // get variant details

        Task<List<Sku_ProductVariantDto>> GetVariantsByProductIdAsync(int productId);

        Task<List<BusinessNotificationDto>> GetNotificationsAsync(
       int busRegId,
       bool onlyUnread
   );
        Task MarkeachNotificationAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int busRegId);
        //  Task GetNotificationsAsync(int busRegId, bool onlyUnread);

        //Sales tab - transaction deatils

        Task<List<Salestab_storeTransactionsDto>> GetStoreTransactionsAsync(
       int storeId,
       string? search,
       int pageNumber,
       int pageSize);
        // Transaction details 
       
            Task<TransactionDetailsDto> GetTransactionDetailsAsync(int paymentId);
       


        // country wise sales

        Task<List<CountrySalesDto>> GetCountryWiseSalesAsync(int storeId);

        //product wise sales

        Task<List<ProductSalesDto>> GetTopProductsAsync(int storeId, int topCount = 5);

        //notification to coueir - ready to ship

        Task MarkReadyToShipAsync(int storeOrderId);
        // get monthly Revenue - for summary page

        Task<BusinessSalesSummaryDto> GetMonthlySalesAsync(int storeId, int? year, int? month, string? currency);

        //sales history

        Task<StoreSalesHistoryDto> GetSalesHistoryByStoreIdAsync(int storeId);

        //sales trend graph
        Task<List<SalesTrendDto>> GetSalesTrendAsync(int storeId, DateTime? fromDate, DateTime? toDate);

    }
}
