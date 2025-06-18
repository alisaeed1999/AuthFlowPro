using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Application.DTOs.Identity;
using AuthFlowPro.Application.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthFlowPro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Permissions.Role.View)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
        => _roleService = roleService;

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [Authorize(Policy = Permissions.Role.Create)]
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var success = await _roleService.CreateRoleAsync(request);
        return success ? Ok("Role created.") : BadRequest("Failed or role exists.");
    }

    [Authorize(Policy = Permissions.Role.Edit)]
    [HttpPost("permissions")]
    public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionRequest request)
    {
        var success = await _roleService.AssignPermissionsAsync(request);
        return success ? Ok("Permissions updated.") : BadRequest("Failed.");
    }
}
