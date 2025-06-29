using AuthFlowPro.Domain.Entities;

namespace AuthFlowPro.Application.DTOs.Organization;

public class OrganizationMemberDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public OrganizationRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}

public class InviteMemberRequest
{
    public string Email { get; set; } = string.Empty;
    public OrganizationRole Role { get; set; } = OrganizationRole.Member;
}

public class UpdateMemberRoleRequest
{
    public Guid MemberId { get; set; }
    public OrganizationRole Role { get; set; }
}