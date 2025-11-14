using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models.DTO_s;

namespace mytown.Controllers
{
    [Authorize]
    [Route("api/shoppingcart/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderRepository orderRepo,
                                 ILogger<OrderController> logger)
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
      


        [HttpPost("CreateOrders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestddto request)
        {
            var orderId = await _orderRepo.CreateOrderAsync(request.ShopperRegId, request.ShippingSelections);

            if (orderId == 0)
                return BadRequest("No items in cart.");

            return Ok(new { Message = "Order placed successfully", OrderId = orderId });
        }


        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromQuery] int shopperRegId)
        {
            if (shopperRegId <= 0)
                return BadRequest("Invalid shopper ID.");
            var orderId = await _orderRepo.CreateOrderAndOrderDetailsAsync(shopperRegId);
            if (orderId == 0)
                return BadRequest("No items in cart.");

            return Ok(new { Message = "Order created successfully.", OrderId = orderId });
        }

        [HttpPost("SaveShippingSelections")]
        public async Task<IActionResult> SaveShippingSelections([FromQuery] int orderId, [FromBody] List<StoreShippingSelection> selections)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest("Invalid order ID.");

                if (selections == null || !selections.Any())
                    return BadRequest("No shipping selections provided.");

                await _orderRepo.SaveShippingSelectionsAsync(orderId, selections);

                return Ok(new { Message = "Shipping details saved successfully." });
            }
            catch (Exception ex)
            {
                // Log exception here
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

    }
}
