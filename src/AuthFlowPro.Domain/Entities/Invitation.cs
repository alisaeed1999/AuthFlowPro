namespace AuthFlowPro.Domain.Entities;

public class Invitation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public OrganizationRole Role { get; set; } = OrganizationRole.Member;
    public string Token { get; set; } = string.Empty;
    public Guid InvitedByUserId { get; set; }
    public ApplicationUser InvitedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool IsAccepted { get; set; } = false;
    public DateTime? AcceptedAt { get; set; }
    public bool IsRevoked { get; set; } = false;
}