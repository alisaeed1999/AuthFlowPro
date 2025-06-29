using AuthFlowPro.Domain.Entities;

namespace AuthFlowPro.Application.DTOs.Organization;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PlanInterval Interval { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public bool AutoRenew { get; set; }
}

public class PlanDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PlanInterval Interval { get; set; }
    public int MaxUsers { get; set; }
    public int MaxProjects { get; set; }
    public bool HasAdvancedFeatures { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSubscriptionRequest
{
    public string PlanId { get; set; } = string.Empty;
    public string? PaymentMethodId { get; set; }
}