using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;
using System.Text.Json;

namespace mytown.Controllers
{

    [ApiController]
    [Route("api/courier")]
    public class CourierDashboardController : ControllerBase
    {
        private readonly ICourierDashboardService _courierService;
        private readonly IEmailService _emailService;                 // still available for store notifications etc.
        private readonly IConfiguration _configuration;
        private readonly ILogger<CourierController> _logger;
        private readonly IBusinessRepository _businessRepo;
        private readonly IShopperRepository _shopperRepo;

        public CourierDashboardController(
            ICourierDashboardService courierService,
            IBusinessRepository businessRepo,
            IShopperRepository shopperRepo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<CourierController> logger)
        {
            _courierService = courierService;
            _businessRepo = businessRepo;
            _shopperRepo = shopperRepo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("Courierhead-orders")]
        public async Task<IActionResult> GetOrders(
      int courierId,
      [FromQuery] string status,
      [FromQuery] string? search,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("Shipping status is required");

            var orders = await _courierService.GetOrdersAsync(
                courierId,
                status,
                search,
                pageNumber,
                pageSize);

            return Ok(orders);
        }

        [HttpGet("branch-orders")]
        public async Task<IActionResult> GetBranchOrders(
   int branchId,
   string shippingStatus,
   string? search,
   int pageNumber = 1,
   int pageSize = 10)
        {
            var result = await _courierService.GetOrdersByBranchAsync(
                branchId,
                shippingStatus,
                search,
                pageNumber,
                pageSize
            );

            return Ok(result);
        }

        [HttpGet("Orderdetail_StoreOrder")]
        public async Task<IActionResult> GetCourierOrderDetail(int storeOrderId)
        {
            var result = await _courierService.GetCourierOrderDetailAsync(storeOrderId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("AssignTrackingId")]
        public async Task<IActionResult> AssignTracking(
    int storeOrderId,
    [FromBody] AssignTrackingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TrackingId))
                return BadRequest("Tracking ID is required");

            try
            {
                await _courierService.AssignTrackingAsync(
                    storeOrderId,
                    dto.TrackingId);

                return Ok("Tracking ID assigned and order moved to In Progress");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("MarkasDelivered")]
        public async Task<IActionResult> MarkAsDelivered(int storeOrderId)
        {
            try
            {
                await _courierService.MarkAsDeliveredAsync(storeOrderId);

                return Ok("Order marked as delivered");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Courierprofilesummary")]
        public async Task<IActionResult> GetProfileSummary(
    int courierId,
    [FromQuery] int? month,
    [FromQuery] int? year,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate)
        {
            var filter = new CourierDeliveryFilterDto
            {
                Month = month,
                Year = year,
                FromDate = fromDate,
                ToDate = toDate
            };

            var summary = await _courierService.GetProfileSummaryAsync(courierId, filter);

            return Ok(summary);
        }

        // 🔹 Today’s Deliveries
        [HttpGet("deliveries/today")]
        public async Task<IActionResult> GetTodayDeliveries(int courierId)
        {
           // int courierId = int.Parse(User.FindFirst("CourierId").Value);

            var result = await _courierService.GetCompletedDeliveriesAsync(
                courierId,
                DateTime.UtcNow);

            return Ok(result);
        }

        // 🔹 All Completed Deliveries
        [HttpGet("deliveries/completed")]
        public async Task<IActionResult> GetAllCompletedDeliveries(int courierId)
        {
          //  int courierId = int.Parse(User.FindFirst("CourierId").Value);

            var result = await _courierService.GetCompletedDeliveriesAsync(
                courierId,
                null);

            return Ok(result);
        }

        [HttpGet("Getcouriernotifications")]
        public async Task<IActionResult> GetUnreadNotifications(int courierId)
        {
            var notifications = await _courierService.GetUnreadNotificationsAsync(courierId);
            return Ok(notifications);
        }

        [HttpPost("MarkcouriernotificationsAsread")]
        public async Task<IActionResult> MarkNotificationsAsRead(int courierId)
        {
            await _courierService.MarkAsReadAsync(courierId);
            return Ok(new { message = "Notifications marked as read" });
        }

        [HttpGet("branches_info_courierId")]
        public async Task<IActionResult> GetBasicBranches(int courierId)
        {
            var result = await _courierService.GetBasicBranchesAsync(courierId);
            return Ok(result);
        }


        // API'S for courier branch dashboard

        [HttpGet("branch")]
        public async Task<IActionResult> GetBranch(int branchId)
        {
            var branch = await _courierService.GetBranchAsync(branchId);
            return Ok(branch);
        }

        [HttpGet("Branchprofilesummary")]
        public async Task<IActionResult> GetBranchProfileSummary(
     int branchId,
     [FromQuery] int? month,
     [FromQuery] int? year,
     [FromQuery] DateTime? fromDate,
     [FromQuery] DateTime? toDate)
        {
            var filter = new CourierDeliveryFilterDto
            {
                Month = month,
                Year = year,
                FromDate = fromDate,
                ToDate = toDate
            };

            var summary = await _courierService.GetBranchProfileSummaryAsync(branchId, filter);

            return Ok(summary);
        }

        [HttpGet("branch/completed-today")]
        public async Task<IActionResult> GetCompletedToday(int branchId)
        {
            var result = await _courierService.GetCompletedDeliveriesCountByBranchAsync(branchId, DateTime.Today);
            return Ok(result);
        }

        //[HttpGet("branch/completed-total")]
        //public async Task<IActionResult> GetTotalCompleted(int branchId)
        //{
        //    var result = await _courierService.GetTotalCompletedDeliveriesCountByBranchAsync(branchId);
        //    return Ok(result);
        //}

        //[HttpGet("branch/pending-tasks")]
        //public async Task<IActionResult> GetPendingTasks(int branchId)
        //{
        //    var result = await _courierService.GetPendingTasksCountByBranchAsync(branchId);
        //    return Ok(result);
        //}

        [HttpGet("branch/completed-deliveries")]
        public async Task<IActionResult> GetCompletedDeliveries(int branchId, DateTime? date)
        {
            var result = await _courierService.GetCompletedDeliveriesByBranchAsync(branchId, date);
            return Ok(result);
        }

        [HttpGet("branch/unread-notifications")]
        public async Task<IActionResult> GetUnreadNotificationsforbranch(int branchId)
        {
            var result = await _courierService.GetUnreadNotificationsByBranchAsync(branchId);
            return Ok(result);
        }

        [HttpPut("markeach-notification-read")]
        public async Task<IActionResult> MarkEachNotificationRead(int notificationId)
        {
            await _courierService.MarkEachNotificationReadAsync(notificationId);
            return Ok(new { message = "Notification marked as read successfully" });
        }

        [HttpPost("upload-delivery-proof")]
       // [HttpPost("upload-delivery-proof")]
        public async Task<IActionResult> UploadDeliveryProof([FromForm] UploadDeliveryProofDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is required");

            var fileName = await _courierService.UploadDeliveryProofAsync(dto.StoreOrderId, dto.File);

            return Ok(new
            {
                message = "Delivery proof uploaded successfully",
                fileName = fileName
            });
        }

        //update or edit bank details
       

        [HttpPut("update-courier-account/{courierId}")]
        public async Task<IActionResult> UpdateCourierAccount(
    int courierId,
    [FromBody] UpdateCourierAccountDetailDto dto)
        {
            try
            {
                var updated = await _courierService.UpdateCourierAccountDetailsAsync(courierId, dto);

                return Ok(new
                {
                    message = "Courier account details saved successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }

        [HttpGet("getcourier-bankaccount/{courierId}")]
        public async Task<IActionResult> GetCourierAccountDetails(int courierId)
        {
            var account = await _courierService.GetCourierAccountDetailsByCourierIdAsync(courierId);

            if (account == null)
                return NotFound(new { message = "Courier account details not found." });

            return Ok(account);
        }

    }

}
