namespace AuthFlowPro.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string PlanId { get; set; } = string.Empty;
    public Plan Plan { get; set; } = null!;
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
    public bool AutoRenew { get; set; } = true;
}

public enum SubscriptionStatus
{
    Active = 1,
    Cancelled = 2,
    PastDue = 3,
    Unpaid = 4,
    Trialing = 5
}