using AuthFlowPro.Domain.Entities;

namespace AuthFlowPro.Application.DTOs.Organization;

public class InvitationDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public OrganizationRole Role { get; set; }
    public string InvitedByName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsAccepted { get; set; }
    public bool IsRevoked { get; set; }
}

public class AcceptInvitationRequest
{
    public string Token { get; set; } = string.Empty;
}