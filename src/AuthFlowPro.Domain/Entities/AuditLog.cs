namespace AuthFlowPro.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string Action { get; set; } = string.Empty; // e.g., "user.created", "role.updated"
    public string EntityType { get; set; } = string.Empty; // e.g., "User", "Role"
    public string? EntityId { get; set; }
    public string? Details { get; set; } // JSON string with additional details
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}