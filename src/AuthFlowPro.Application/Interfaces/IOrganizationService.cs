using AuthFlowPro.Application.DTOs.Organization;

namespace AuthFlowPro.Application.Interfaces;

public interface IOrganizationService
{
    Task<List<OrganizationDto>> GetUserOrganizationsAsync(Guid userId);
    Task<OrganizationDto?> GetOrganizationAsync(Guid organizationId);
    Task<OrganizationDto?> GetOrganizationBySlugAsync(string slug);
    Task<(bool Success, string Message, OrganizationDto? Organization)> CreateOrganizationAsync(Guid userId, CreateOrganizationRequest request);
    Task<(bool Success, string Message)> UpdateOrganizationAsync(Guid organizationId, UpdateOrganizationRequest request);
    Task<(bool Success, string Message)> DeleteOrganizationAsync(Guid organizationId);
    
    // Member management
    Task<List<OrganizationMemberDto>> GetOrganizationMembersAsync(Guid organizationId);
    Task<(bool Success, string Message)> InviteMemberAsync(Guid organizationId, Guid invitedByUserId, InviteMemberRequest request);
    Task<(bool Success, string Message)> UpdateMemberRoleAsync(Guid organizationId, UpdateMemberRoleRequest request);
    Task<(bool Success, string Message)> RemoveMemberAsync(Guid organizationId, Guid memberId);
    
    // Invitations
    Task<List<InvitationDto>> GetPendingInvitationsAsync(Guid organizationId);
    Task<(bool Success, string Message)> AcceptInvitationAsync(string token, Guid userId);
    Task<(bool Success, string Message)> RevokeInvitationAsync(Guid invitationId);
}