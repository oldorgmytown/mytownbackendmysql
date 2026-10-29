using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mytown.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.DTOs;

namespace mytown.Controllers
{
    [ApiController]
  //  [Authorize]
    [Route("api/shopper-connections")]
    public class ShopperConnectionsController : ControllerBase
    {
        private readonly IShopperConnectionService _service;

        public ShopperConnectionsController(IShopperConnectionService service)
        {
            _service = service;
        }

        // Top banner: profile details + connected shoppers count
        // GET api/shopper-connections/header?shopperId=1
        [HttpGet("header")]
        public async Task<IActionResult> GetHeader([FromQuery] int shopperId)
        {
            try
            {
                var result = await _service.GetHeaderAsync(shopperId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Right panel: requests sent to me (panel uses pageSize=3)
        // GET api/shopper-connections/received-requests?shopperId=1&search=&page=1&pageSize=3
        [HttpGet("received-requests")]
        public async Task<IActionResult> GetReceivedRequests(
            [FromQuery] int shopperId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            var result = await _service.GetReceivedRequestsAsync(shopperId, search, page, pageSize);
            return Ok(result);
        }

        // "+ Connect" button in the right panel
        // PATCH api/shopper-connections/approve-request?shopperId=1&connectionId=5
        [HttpPut("approve-request")]
        public async Task<IActionResult> Approve([FromQuery] int shopperId, [FromQuery] int connectionId)
        {
            try
            {
                await _service.ApproveAsync(shopperId, connectionId);
                return Ok(new { message = "Connection approved." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        // "Connect" button in View All: send a new request
        // POST api/shopper-connections/send-request   body: { "shopperId": 1, "receiverShopperId": 7 }
        [HttpPost("send-request")]
        public async Task<IActionResult> SendRequest([FromBody] SendConnectionRequestDto dto)
        {
            try
            {
                var result = await _service.SendRequestAsync(dto.ShopperId, dto.ReceiverShopperId);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        // View All, Tab 1: shoppers not yet connected, with search and paging
        // GET api/shopper-connections/not-connected?shopperId=1&search=pune&page=1&pageSize=5
        [HttpGet("not-connected")]
        public async Task<IActionResult> GetNotConnected(
            [FromQuery] int shopperId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var result = await _service.GetNotConnectedShoppersAsync(shopperId, search, page, pageSize);
            return Ok(result);
        }

        // View All, Tab 3: requests I sent, with search and paging
        // GET api/shopper-connections/sent-requests?shopperId=17&search=&page=1&pageSize=5
        [HttpGet("sent-requests")]
        public async Task<IActionResult> GetSentRequests(
            [FromQuery] int shopperId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var result = await _service.GetSentRequestsAsync(shopperId, search, page, pageSize);
            return Ok(result);
        }

        // Withdraw button
        // DELETE api/shopper-connections/withdraw-request?shopperId=17&connectionId=5
        [HttpDelete("withdraw-request")]
        public async Task<IActionResult> Withdraw([FromQuery] int shopperId, [FromQuery] int connectionId)
        {
            try
            {
                await _service.WithdrawAsync(shopperId, connectionId);
                return Ok(new { message = "Request withdrawn." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        // Disconnect button (left panel and All Connections)
        // DELETE api/shopper-connections/disconnect?shopperId=17&connectionId=5
        [HttpDelete("disconnect")]
        public async Task<IActionResult> Disconnect([FromQuery] int shopperId, [FromQuery] int connectionId)
        {
            try
            {
                await _service.DisconnectAsync(shopperId, connectionId);
                return Ok(new { message = "Disconnected." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        // Left panel (pageSize=4) and View All "All Connections" tab (search + paging)
        // GET api/shopper-connections/connected-shoppers?shopperId=17&search=&page=1&pageSize=4
        [HttpGet("connected-shoppers")]
        public async Task<IActionResult> GetConnectedShoppers(
            [FromQuery] int shopperId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 4)
        {
            var result = await _service.GetConnectedShoppersAsync(shopperId, search, page, pageSize);
            return Ok(result);
        }

        // All businesses this shopper is connected to, with search and paging
        // GET api/shopper-connections/business-connections?shopperId=17&search=&page=1&pageSize=5
        [HttpGet("business-connections")]
        public async Task<IActionResult> GetBusinessConnections(
            [FromQuery] int shopperId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var result = await _service.GetBusinessConnectionsAsync(shopperId, search, page, pageSize);
            return Ok(result);
        }

        // Shopper connections and business connections together, with search and paging
        // GET api/shopper-connections/all-connections?shopperId=17&search=&page=1&pageSize=5
        [HttpGet("all-connections")]
        public async Task<IActionResult> GetAllConnections(
            [FromQuery] int shopperId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var result = await _service.GetAllConnectionsAsync(shopperId, search, page, pageSize);
            return Ok(result);
        }
    }
}
