using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StorePayoutController : ControllerBase
{
    private readonly IStorePayoutService _storePayoutService;

    public StorePayoutController(IStorePayoutService storePayoutService)
    {
        _storePayoutService = storePayoutService;
    }

    [HttpPost("triggerPayout/{storeOrderId}")]
    public async Task<IActionResult> TriggerPayout(int storeOrderId)
    {
        try
        {
            var result = await _storePayoutService.CreatePayoutAsync(storeOrderId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}