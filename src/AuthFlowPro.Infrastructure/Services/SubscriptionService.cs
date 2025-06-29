using AuthFlowPro.Application.DTOs.Organization;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using AuthFlowPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthFlowPro.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public SubscriptionService(AppDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<List<PlanDto>> GetAvailablePlansAsync()
    {
        return await _context.Plans
            .Where(p => p.IsActive)
            .Select(p => new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Currency = p.Currency,
                Interval = p.Interval,
                MaxUsers = p.MaxUsers,
                MaxProjects = p.MaxProjects,
                HasAdvancedFeatures = p.HasAdvancedFeatures,
                IsActive = p.IsActive
            })
            .ToListAsync();
    }

    public async Task<SubscriptionDto?> GetOrganizationSubscriptionAsync(Guid organizationId)
    {
        return await _context.Subscriptions
            .Where(s => s.OrganizationId == organizationId)
            .Include(s => s.Plan)
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                PlanId = s.PlanId,
                PlanName = s.Plan.Name,
                Price = s.Plan.Price,
                Currency = s.Plan.Currency,
                Interval = s.Plan.Interval,
                Status = s.Status,
                CurrentPeriodStart = s.CurrentPeriodStart,
                CurrentPeriodEnd = s.CurrentPeriodEnd,
                AutoRenew = s.AutoRenew
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CreateSubscriptionAsync(Guid organizationId, CreateSubscriptionRequest request)
    {
        var plan = await _context.Plans.FindAsync(request.PlanId);
        if (plan == null)
        {
            return (false, "Plan not found");
        }

        // Check if organization already has a subscription
        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.OrganizationId == organizationId);

        if (existingSubscription != null)
        {
            return (false, "Organization already has a subscription");
        }

        // TODO: Integrate with Stripe to create subscription
        // For now, create a basic subscription
        var subscription = new Subscription
        {
            OrganizationId = organizationId,
            PlanId = request.PlanId,
            Status = SubscriptionStatus.Active,
            CurrentPeriodStart = DateTime.UtcNow,
            CurrentPeriodEnd = plan.Interval == PlanInterval.Monthly 
                ? DateTime.UtcNow.AddMonths(1) 
                : DateTime.UtcNow.AddYears(1)
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, null, "subscription.created", "Subscription", subscription.Id.ToString(), new { PlanId = request.PlanId });

        return (true, "Subscription created successfully");
    }

    public async Task<(bool Success, string Message)> CancelSubscriptionAsync(Guid organizationId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.OrganizationId == organizationId);

        if (subscription == null)
        {
            return (false, "No active subscription found");
        }

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.CancelledAt = DateTime.UtcNow;
        subscription.AutoRenew = false;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, null, "subscription.cancelled", "Subscription", subscription.Id.ToString());

        return (true, "Subscription cancelled successfully");
    }

    public async Task<(bool Success, string Message)> UpdateSubscriptionAsync(Guid organizationId, string newPlanId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.OrganizationId == organizationId);

        if (subscription == null)
        {
            return (false, "No active subscription found");
        }

        var newPlan = await _context.Plans.FindAsync(newPlanId);
        if (newPlan == null)
        {
            return (false, "Plan not found");
        }

        var oldPlanId = subscription.PlanId;
        subscription.PlanId = newPlanId;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, null, "subscription.updated", "Subscription", subscription.Id.ToString(), new { OldPlanId = oldPlanId, NewPlanId = newPlanId });

        return (true, "Subscription updated successfully");
    }

    public async Task HandleStripeWebhookAsync(string eventType, object eventData)
    {
        // TODO: Implement Stripe webhook handling
        // This would handle events like:
        // - invoice.payment_succeeded
        // - invoice.payment_failed
        // - customer.subscription.updated
        // - customer.subscription.deleted
        await Task.CompletedTask;
    }
}