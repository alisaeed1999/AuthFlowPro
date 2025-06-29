namespace AuthFlowPro.Domain.Entities;

public class Plan
{
    public string Id { get; set; } = string.Empty; // e.g., "starter", "pro", "enterprise"
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public PlanInterval Interval { get; set; } = PlanInterval.Monthly;
    public string? StripePriceId { get; set; }
    public bool IsActive { get; set; } = true;
    public int MaxUsers { get; set; } = 5;
    public int MaxProjects { get; set; } = 10;
    public bool HasAdvancedFeatures { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

public enum PlanInterval
{
    Monthly = 1,
    Yearly = 2
}