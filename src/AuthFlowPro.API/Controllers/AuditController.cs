using AuthFlowPro.Application.DTOs.Audit;
using AuthFlowPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthFlowPro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetAuditLogs(Guid organizationId, [FromQuery] AuditLogQuery query)
    {
        var result = await _auditService.GetAuditLogsAsync(organizationId, query);
        
        return Ok(new 
        { 
            logs = result.Logs, 
            totalCount = result.TotalCount,
            page = query.Page,
            pageSize = query.PageSize
        });
    }
}