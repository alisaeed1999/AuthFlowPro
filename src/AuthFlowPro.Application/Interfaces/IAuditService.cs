using AuthFlowPro.Application.DTOs.Audit;

namespace AuthFlowPro.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(Guid organizationId, Guid? userId, string action, string entityType, string? entityId = null, object? details = null, string? ipAddress = null, string? userAgent = null);
    Task<(List<AuditLogDto> Logs, int TotalCount)> GetAuditLogsAsync(Guid organizationId, AuditLogQuery query);
}