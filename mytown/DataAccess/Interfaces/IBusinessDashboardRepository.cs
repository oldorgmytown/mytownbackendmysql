using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessDashboardRepository
    {

        Task<SalesReportDTO> GetSalesReportByStoreId(
      int storeId,
      DateTime? startDate,
      DateTime? endDate,
      int? month,
      int? year);

        // Get monthly revenue for summary page 
        Task<BusinessSalesSummaryDto> GetMonthlySalesAsync(
        int storeId, int? year, int? month, string? currency);
       // Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId);
        Task<LocationStatsDto> GetLocationCountsByStoreIdAsync(int storeId);

        

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


        Task<List<BusinessOrderListDto>> GetNewOrdersAsync(
        int storeId,
        string? search,
        int pageNumber,
        int pageSize);

        Task<List<BusinessOrderListDto>> GetPendingOrdersAsync(
            int storeId,
            string? search,
            int pageNumber,
            int pageSize);

        Task<List<BusinessOrderListDto>> GetInProgressOrdersAsync(
            int storeId,
            string? search,
            int pageNumber,
            int pageSize);

        Task<List<BusinessOrderListDto>> GetCompletedOrdersAsync(
            int storeId,
            string? search,
            int pageNumber,
            int pageSize);

        // show details of orders -- click on storeorderid

        Task<BusinessOrderDetailsDto> GetBusinessOrderDetailsAsync(int storeOrderId);

        // get products on store
        Task<List<BusinessProductDashboardDto>> GetProductsForDashboardAsync(
            int storeId,
            string? search,
            int pageNumber,
            int pageSize);
        // get variant deatils
        Task<List<Sku_ProductVariantDto>> GetVariantsByProductIdAsync(int productId);

        //Business notifications
        Task<List<BusinessDBNotifications>> GetNotificationsAsync(int busRegId, bool onlyUnread);

        Task MarkAllAsReadAsync(int busRegId);

        Task MarkeachNotificationAsReadAsync(int notificationId);

        //sales tab

        Task<List<Salestab_storeTransactionsDto>> GetStoreTransactionsAsync(
     int storeId,
     string? search,
     int pageNumber,
     int pageSize);


        //transcation deatils 

        Task<TransactionDetailsDto> GetTransactionDetailsAsync(int paymentId);
        //country wise sales
        Task<List<CountrySalesDto>> GetCountryWiseSalesAsync(int storeId);

        //product wise sales

        Task<List<ProductSalesDto>> GetTopProductsAsync(int storeId, int topCount);


        //Notofication to courier -  Ready to ship 

        Task<ShippingDetails?> GetShippingByStoreOrderIdAsync(int storeOrderId);
        Task UpdateShippingStatusAsync(int storeOrderId, string status);
        Task UpdateStoreOrderStatusAsync(int storeOrderId, string status);
        Task AddCourierNotificationAsync(CourierDBNotifications notification);
        Task AddTransporterNotificationAsync(TransporterDBNotifications notification);
        Task SaveChangesAsync();

        //sales history

        Task<StoreSalesHistoryDto> GetSalesHistoryByStoreIdAsync(int storeId);

        //sales trend graph

        Task<List<SalesTrendDto>> GetSalesTrendAsync(int storeId, DateTime? fromDate, DateTime? toDate);

        // store package details
        Task AddShippingPackageDetailsAsync(ShippingPackageDetails packageDetails);
    }
}
