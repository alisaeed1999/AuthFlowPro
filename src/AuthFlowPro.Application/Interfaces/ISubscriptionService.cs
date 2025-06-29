using AuthFlowPro.Application.DTOs.Organization;

namespace AuthFlowPro.Application.Interfaces;

public interface ISubscriptionService
{
    Task<List<PlanDto>> GetAvailablePlansAsync();
    Task<SubscriptionDto?> GetOrganizationSubscriptionAsync(Guid organizationId);
    Task<(bool Success, string Message)> CreateSubscriptionAsync(Guid organizationId, CreateSubscriptionRequest request);
    Task<(bool Success, string Message)> CancelSubscriptionAsync(Guid organizationId);
    Task<(bool Success, string Message)> UpdateSubscriptionAsync(Guid organizationId, string newPlanId);
    Task HandleStripeWebhookAsync(string eventType, object eventData);
}