using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Controllers
{
  //  [Authorize]
    [ApiController]
    [Route("api/businessdashboard")]
    public class BusinessDashboardController : ControllerBase
    {
        private readonly IBusinessDashboardRepository _dashboardRepository;
        private readonly IBusinessDashboardService _dasboardservice;

        public BusinessDashboardController(IBusinessDashboardRepository dashboardRepository, IBusinessDashboardService dashboardService)
        {
            _dashboardRepository = dashboardRepository;
            _dasboardservice = dashboardService;
        }

        //[HttpGet("orders/{storeId}")]
        //public async Task<ActionResult<List<BusinessDashboardDto>>> GetStoreOrdersReport(int storeId)
        //{
        //    var result = await _dashboardRepository.GetStoreOrdersReport(storeId);
        //    if (result == null || result.Count == 0)
        //        return NotFound("No orders found for this store.");

        //    return Ok(result);
        //}
        ////sales report with sort and search

        //    [HttpGet("orders-report")]
        //    public async Task<IActionResult> GetStoreOrdersReportsortsearch(
        //int storeId,
        //string? search = null,
        //string? sortBy = null,
        //bool descending = false)
        //    {
        //        var report = await _dashboardRepository.GetStoreOrdersReportsortsearch(storeId, search, sortBy, descending);
        //        return Ok(report);
        //    }


        // GET api/businessdashboard/locationcounts/{storeId}
        [HttpGet("locationcounts/{storeId}")]
        public async Task<ActionResult<LocationStatsDto>> GetLocationCountsByStoreId(int storeId)
        {
            var result = await _dashboardRepository.GetLocationCountsByStoreIdAsync(storeId);
            if (result == null)
                return NotFound("No shoppers found for this store.");

            return Ok(result);
        }

        [HttpGet("dashboardsummary")]
       
        public async Task<IActionResult> GetSalesReport(
    int storeId,
    DateTime? startDate,
    DateTime? endDate,
    int? month,
    int? year)
        {
            var result = await _dashboardRepository.GetSalesReportByStoreId(storeId, startDate, endDate, month, year);
            return Ok(result);
        }

        [HttpGet("monthly-revenue-summary")]
        public async Task<IActionResult> GetMonthlySalesSummary(
    int storeId,
    int? year,
    int? month,
    string? currency)
        {
            var result = await _dasboardservice.GetMonthlySalesAsync(storeId, year, month, currency);
            return Ok(result);
        }

        //[HttpGet("dashboardproducts")]
        //public async Task<IActionResult> GetProductsByStore(
        //int busRegId,
        //[FromQuery] string searchText = null,
        //[FromQuery] string sortBy = "id",
        //[FromQuery] string sortDirection = "asc",
        //[FromQuery] int page = 1,
        //[FromQuery] int pageSize = 10)
        //{
        //  //  var products = await _dashboardRepository.GetProductsWithPurchasedCountAsync(busRegId, searchText, sortBy, sortDirection, page, pageSize);
        //   // return Ok(products);
        //}


        [HttpGet("GetCustomerAnalytics")]
        public async Task<IActionResult> GetCustomerAnalytics(
    int storeId,
    string? search = null,
    string? sortBy = null,
    bool descending = false)
        {
            var result = await _dashboardRepository.GetCustomerAnalyticsAsync(storeId, search, sortBy, descending);
            return Ok(result);
        }


        //latest 05-01-26 Orders - new, pending,in progress, complete
        [HttpGet("neworders")]
        public async Task<IActionResult> GetNewOrders(
            int storeId,
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _dasboardservice.GetNewOrdersAsync(storeId, search, pageNumber, pageSize));
        }

        [HttpGet("pendingorders")]
        public async Task<IActionResult> GetPendingOrders(
            int storeId,
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _dasboardservice.GetPendingOrdersAsync(storeId, search, pageNumber, pageSize));
        }

        [HttpGet("inprogress_shippedorders")]
        public async Task<IActionResult> GetInProgressOrders(
            int storeId,
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _dasboardservice.GetInProgressOrdersAsync(storeId, search, pageNumber, pageSize));
        }

        [HttpGet("completedorders")]
        public async Task<IActionResult> GetCompletedOrders(
            int storeId,
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _dasboardservice.GetCompletedOrdersAsync(storeId, search, pageNumber, pageSize));
        }

        //OrderDeatils for storeorderid

        [HttpGet("order-details_Storeorderid")]
        public async Task<IActionResult> GetOrderDetails(int storeOrderId)
        {
            var details = await _dasboardservice.GetBusinessOrderDetailsAsync(storeOrderId);

            if (details == null)
                return NotFound(new { message = "Order not found" });

            return Ok(details);
        }

        [HttpGet("Productsonstore")]
        public async Task<IActionResult> GetProducts(
            int storeId,
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _dasboardservice.GetProductsAsync(storeId, search, pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("productvariantsdeatils")]
        public async Task<IActionResult> GetVariants(int productId)
        {
            var variants = await _dasboardservice.GetVariantsByProductIdAsync(productId);

            if (variants == null || variants.Count == 0)
                return NotFound("No variants found for this product");

            return Ok(variants);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications(
    int busRegId,
    [FromQuery] bool onlyUnread = false)
        {
            var result = await _dasboardservice.GetNotificationsAsync(busRegId, onlyUnread);
            return Ok(result);
        }

        [HttpPut("notifications_mark-read")]
        public async Task<IActionResult> MarkNotificationsAsRead(int busRegId)
        {
            await _dasboardservice.MarkAllAsReadAsync(busRegId);
            return Ok(new { message = "Notifications marked as read" });
        }

        //sales tab - Store Transaction details

        [HttpGet("salestab_storetransactions")]
        public async Task<IActionResult> GetStoreTransactions(
     int storeId,
     string? search,
     int pageNumber = 1,
     int pageSize = 10)
        {
            var data = await _dasboardservice.GetStoreTransactionsAsync(storeId, search, pageNumber, pageSize);
            return Ok(data);
        }


        // Transaction id Details 
        [HttpGet("transaction-details")]
        public async Task<IActionResult> GetTransactionDetails(int paymentId)
        {
            var result = await _dasboardservice.GetTransactionDetailsAsync(paymentId);

            if (result == null)
                return NotFound("Transaction not found");

            return Ok(result);
        }

        //countrywise_sales
        [HttpGet("Businessdb_country-sales")]
        public async Task<IActionResult> GetCountryWiseSales(int storeId)
        {
            var data = await _dasboardservice.GetCountryWiseSalesAsync(storeId);
            return Ok(data);
        }

        //product wise sales - top5

        [HttpGet("businessdb_top-productsSales")]
        public async Task<IActionResult> GetTopProducts(
    int storeId,
    int top = 5)
        {
            var result = await _dasboardservice.GetTopProductsAsync(storeId, top);
            return Ok(result);
        }


        // Notificatio to courier - Ready to ship

        [HttpPost("ready-to-ship_NotificationtoCourier")]
        public async Task<IActionResult> MarkReadyToShip(int storeOrderId)
        {
            await _dasboardservice.MarkReadyToShipAsync(storeOrderId);
            return Ok(new
            {
                message = "Order marked as Ready to Ship"
            });
        }

        //sales history

        [HttpGet("sales-history")]
        public async Task<IActionResult> GetSalesHistory(int storeId)
        {
            var result = await _dasboardservice.GetSalesHistoryByStoreIdAsync(storeId);
            return Ok(result);
        }

        //sales trend graph
        [HttpGet("sales-trend-graph")]
        public async Task<IActionResult> GetSalesTrend(
    int storeId,
    DateTime? fromDate,
    DateTime? toDate)
        {
            var result = await _dasboardservice.GetSalesTrendAsync(storeId, fromDate, toDate);
            return Ok(result);
        }
    }



}
