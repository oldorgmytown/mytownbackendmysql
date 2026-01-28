using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessDashboardRepository
    {
       // Task<List<mytown.Models.Order>> GetAllOrdersForStoreAsync(int storeId);
        Task<SalesReportDTO> GetSalesReportByStoreId(int storeId);
       // Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId);
        Task<LocationStatsDto> GetLocationCountsByStoreIdAsync(int storeId);

         //Task<List<ProductDto>> GetProductsWithPurchasedCountAsync(
         //int busRegId,
         //string searchText = null,
         //string sortBy = "id",
         //string sortDirection = "asc",
         //int page = 1,
         //int pageSize = 10);

        Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(
         int storeId,
         string? search = null,
         string? sortBy = null,
         bool descending = false);

        // order sales history with search and sort
        // Task<List<BusinessDashboardDto>> GetStoreOrdersReportsortsearch(
        //int storeId,
        //string? search = null,
        //string? sortBy = null,
        //bool descending = false);


        Task<List<BusinessOrderListDto>> GetNewOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(int storeId);
        Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(int storeId);

        // show details of orders -- click on storeorderid

        Task<BusinessOrderDetailsDto> GetBusinessOrderDetailsAsync(int storeOrderId);

        // Products tab

        Task<List<BusinessProductDashboardDto>> GetProductsForDashboardAsync(int storeId);
        // get variant deatils
        Task<List<Sku_ProductVariantDto>> GetVariantsByProductIdAsync(int productId);

        //Business notifications
        Task<List<BusinessDBNotifications>> GetNotificationsAsync(int busRegId, bool onlyUnread);

        Task MarkAllAsReadAsync(int busRegId);

        //sales tab

        Task<List<Salestab_storeTransactionsDto>> GetStoreTransactionsAsync(int storeId);

        //country wise sales
        Task<List<CountrySalesDto>> GetCountryWiseSalesAsync(int storeId);

        //product wise sales

        Task<List<ProductSalesDto>> GetTopProductsAsync(int storeId, int topCount);

    }
}
