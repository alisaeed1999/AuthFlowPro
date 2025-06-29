using AuthFlowPro.Application.DTOs.Organization;
using AuthFlowPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthFlowPro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailablePlans()
    {
        var plans = await _subscriptionService.GetAvailablePlansAsync();
        return Ok(plans);
    }

    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetOrganizationSubscription(Guid organizationId)
    {
        var subscription = await _subscriptionService.GetOrganizationSubscriptionAsync(organizationId);
        if (subscription == null)
            return NotFound();

        return Ok(subscription);
    }

    [HttpPost("organization/{organizationId}")]
    public async Task<IActionResult> CreateSubscription(Guid organizationId, [FromBody] CreateSubscriptionRequest request)
    {
        var result = await _subscriptionService.CreateSubscriptionAsync(organizationId, request);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpPost("organization/{organizationId}/cancel")]
    public async Task<IActionResult> CancelSubscription(Guid organizationId)
    {
        var result = await _subscriptionService.CancelSubscriptionAsync(organizationId);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpPut("organization/{organizationId}/plan/{newPlanId}")]
    public async Task<IActionResult> UpdateSubscription(Guid organizationId, string newPlanId)
    {
        var result = await _subscriptionService.UpdateSubscriptionAsync(organizationId, newPlanId);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}