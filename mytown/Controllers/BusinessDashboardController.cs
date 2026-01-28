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
        public async Task<IActionResult> GetSalesReport(int storeId)
        {
            var salesReport = await _dashboardRepository.GetSalesReportByStoreId(storeId);
            if (salesReport == null)
            {
                return NotFound();
            }

            return Ok(salesReport);
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
        public async Task<IActionResult> GetNewOrders(int storeId)
            => Ok(await _dasboardservice.GetNewOrdersAsync(storeId));

        [HttpGet("pendingorders")]
        public async Task<IActionResult> GetPendingOrders(int storeId)
            => Ok(await _dasboardservice.GetPendingOrdersAsync(storeId));

        [HttpGet("inprogress_shippedorders")]
        public async Task<IActionResult> GetInProgressOrders(int storeId)
            => Ok(await _dasboardservice.GetInProgressOrdersAsync(storeId));

        [HttpGet("completedorders")]
        public async Task<IActionResult> GetCompletedOrders(int storeId)
            => Ok(await _dasboardservice.GetCompletedOrdersAsync(storeId));

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
        public async Task<IActionResult> GetProducts(int storeId)
        {
            var result = await _dasboardservice.GetProductsAsync(storeId);
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
        public async Task<IActionResult> GetStoreTransactions(int storeId)
        {
            var data = await _dasboardservice.GetStoreTransactionsAsync(storeId);
            return Ok(data);
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

    }



}
