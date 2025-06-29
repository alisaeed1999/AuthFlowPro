using AuthFlowPro.Application.DTOs.Organization;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Application.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthFlowPro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet("my-organizations")]
    public async Task<IActionResult> GetMyOrganizations()
    {
        var userId = GetCurrentUserId();
        var organizations = await _organizationService.GetUserOrganizationsAsync(userId);
        return Ok(organizations);
    }

    [HttpGet("{organizationId}")]
    public async Task<IActionResult> GetOrganization(Guid organizationId)
    {
        var organization = await _organizationService.GetOrganizationAsync(organizationId);
        if (organization == null)
            return NotFound();

        return Ok(organization);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> GetOrganizationBySlug(string slug)
    {
        var organization = await _organizationService.GetOrganizationBySlugAsync(slug);
        if (organization == null)
            return NotFound();

        return Ok(organization);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _organizationService.CreateOrganizationAsync(userId, request);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message, organization = result.Organization });
    }

    [HttpPut("{organizationId}")]
    public async Task<IActionResult> UpdateOrganization(Guid organizationId, [FromBody] UpdateOrganizationRequest request)
    {
        var result = await _organizationService.UpdateOrganizationAsync(organizationId, request);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpDelete("{organizationId}")]
    public async Task<IActionResult> DeleteOrganization(Guid organizationId)
    {
        var result = await _organizationService.DeleteOrganizationAsync(organizationId);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    // Member management endpoints
    [HttpGet("{organizationId}/members")]
    public async Task<IActionResult> GetOrganizationMembers(Guid organizationId)
    {
        var members = await _organizationService.GetOrganizationMembersAsync(organizationId);
        return Ok(members);
    }

    [HttpPost("{organizationId}/members/invite")]
    public async Task<IActionResult> InviteMember(Guid organizationId, [FromBody] InviteMemberRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _organizationService.InviteMemberAsync(organizationId, userId, request);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpPut("{organizationId}/members/role")]
    public async Task<IActionResult> UpdateMemberRole(Guid organizationId, [FromBody] UpdateMemberRoleRequest request)
    {
        var result = await _organizationService.UpdateMemberRoleAsync(organizationId, request);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpDelete("{organizationId}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(Guid organizationId, Guid memberId)
    {
        var result = await _organizationService.RemoveMemberAsync(organizationId, memberId);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    // Invitation endpoints
    [HttpGet("{organizationId}/invitations")]
    public async Task<IActionResult> GetPendingInvitations(Guid organizationId)
    {
        var invitations = await _organizationService.GetPendingInvitationsAsync(organizationId);
        return Ok(invitations);
    }

    [HttpPost("invitations/accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _organizationService.AcceptInvitationAsync(request.Token, userId);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpPost("invitations/{invitationId}/revoke")]
    public async Task<IActionResult> RevokeInvitation(Guid invitationId)
    {
        var result = await _organizationService.RevokeInvitationAsync(invitationId);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}