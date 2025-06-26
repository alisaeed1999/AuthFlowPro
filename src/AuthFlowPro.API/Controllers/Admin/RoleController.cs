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
    [Authorize(Policy = Permissions.Role.View)]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Role.Create)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var result = await _roleService.CreateRoleAsync(request);
        if (!result) return BadRequest("Role already exists or creation failed.");
        return Ok(new {message = "Role created successfully." });
    }

    [HttpPut]
    [Authorize(Policy = Permissions.Role.Edit)]
    public async Task<IActionResult> EditRole([FromBody] EditRoleRequest request)
    {
        var result = await _roleService.EditRoleAsync(request);
        if (!result) return BadRequest("Failed to update role.");
        return Ok(new { message = "Role updated successfully" });
    }

    [HttpDelete("{roleName}")]
    [Authorize(Policy = Permissions.Role.Delete)]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var result = await _roleService.DeleteRoleAsync(roleName);
        if (!result) return NotFound("Role not found or delete failed.");
        return Ok(new { message = "Deleted" });
    }

    [HttpPost("assign-permissions")]
    [Authorize(Policy = Permissions.Role.Edit)]
    public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionRequest request)
    {
        var result = await _roleService.AssignPermissionsAsync(request);
        if (!result) return BadRequest("Assigning permissions failed.");
        return Ok("Permissions assigned successfully.");
    }

    [HttpGet("permissions")]
    [Authorize(Policy = Permissions.Role.View)]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _roleService.GetAllPermissionsAsync();
        return Ok(permissions);
    }
}
