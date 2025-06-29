using AuthFlowPro.Application.DTOs.Audit;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using AuthFlowPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AuthFlowPro.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(Guid organizationId, Guid? userId, string action, string entityType, string? entityId = null, object? details = null, string? ipAddress = null, string? userAgent = null)
    {
        var auditLog = new AuditLog
        {
            OrganizationId = organizationId,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details != null ? JsonSerializer.Serialize(details) : null,
            IpAddress = ipAddress ?? "",
            UserAgent = userAgent ?? ""
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<AuditLogDto> Logs, int TotalCount)> GetAuditLogsAsync(Guid organizationId, AuditLogQuery query)
    {
        var queryable = _context.AuditLogs
            .Where(al => al.OrganizationId == organizationId)
            .Include(al => al.User);

        // Apply filters
        if (!string.IsNullOrEmpty(query.Action))
        {
            queryable = queryable.Where(al => al.Action.Contains(query.Action));
        }

        if (!string.IsNullOrEmpty(query.EntityType))
        {
            queryable = queryable.Where(al => al.EntityType == query.EntityType);
        }

        if (query.UserId.HasValue)
        {
            queryable = queryable.Where(al => al.UserId == query.UserId);
        }

        if (query.FromDate.HasValue)
        {
            queryable = queryable.Where(al => al.CreatedAt >= query.FromDate);
        }

        if (query.ToDate.HasValue)
        {
            queryable = queryable.Where(al => al.CreatedAt <= query.ToDate);
        }

        var totalCount = await queryable.CountAsync();

        var logs = await queryable
            .OrderByDescending(al => al.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(al => new AuditLogDto
            {
                Id = al.Id,
                Action = al.Action,
                EntityType = al.EntityType,
                EntityId = al.EntityId,
                UserName = al.User != null ? al.User.FullName : "System",
                Details = al.Details,
                IpAddress = al.IpAddress,
                CreatedAt = al.CreatedAt
            })
            .ToListAsync();

        return (logs, totalCount);
    }
}