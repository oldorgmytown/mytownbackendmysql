using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;

namespace mytown.Controllers
{
    [Authorize]
    [Route("api/shoppingcart/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService service,
                               ILogger<OrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("CreateOrders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestddto request)
        {
            var orderId = await _service.CreateOrderAsync(request.ShopperRegId, request.ShippingSelections);

            if (orderId == 0)
                return BadRequest("No items in cart.");

            return Ok(new { Message = "Order placed successfully", OrderId = orderId });
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromQuery] int shopperRegId)
        {
            if (shopperRegId <= 0)
                return BadRequest("Invalid shopper ID.");

            var orderId = await _service.CreateOrderAndOrderDetailsAsync(shopperRegId);

            if (orderId == 0)
                return BadRequest("No items in cart.");

            return Ok(new { Message = "Order created successfully.", OrderId = orderId });
        }

        [HttpPost("SaveShippingSelections")]
        public async Task<IActionResult> SaveShippingSelections(
            [FromQuery] int orderId,
            [FromBody] List<StoreShippingSelection> selections)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest("Invalid order ID.");

                if (selections == null || !selections.Any())
                    return BadRequest("No shipping selections provided.");

                await _service.SaveShippingSelectionsAsync(orderId, selections);

                return Ok(new { Message = "Shipping details saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveShippingSelections failed");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
    }
}
