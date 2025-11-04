using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.DTO_s;
using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/businessdashboard")]
    public class BusinessDashboardController : ControllerBase
    {
        private readonly IBusinessDashboardRepository _dashboardRepository;

        public BusinessDashboardController(IBusinessDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("orders/{storeId}")]
        public async Task<ActionResult<List<BusinessDashboardDto>>> GetStoreOrdersReport(int storeId)
        {
            var result = await _dashboardRepository.GetStoreOrdersReport(storeId);
            if (result == null || result.Count == 0)
                return NotFound("No orders found for this store.");

            return Ok(result);
        }
        //sales report with sort and search

        [HttpGet("orders-report")]
        public async Task<IActionResult> GetStoreOrdersReportsortsearch(
    int storeId,
    string? search = null,
    string? sortBy = null,
    bool descending = false)
        {
            var report = await _dashboardRepository.GetStoreOrdersReportsortsearch(storeId, search, sortBy, descending);
            return Ok(report);
        }


        // GET api/businessdashboard/locationcounts/{storeId}
        [HttpGet("locationcounts/{storeId}")]
        public async Task<ActionResult<LocationStatsDto>> GetLocationCountsByStoreId(int storeId)
        {
            var result = await _dashboardRepository.GetLocationCountsByStoreIdAsync(storeId);
            if (result == null)
                return NotFound("No shoppers found for this store.");

            return Ok(result);
        }

        [HttpGet("salesreport/{storeId}")]
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
    }
}
