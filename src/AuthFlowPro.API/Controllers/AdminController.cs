using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Application.DTOs.Identity;
using AuthFlowPro.Application.Permission;

namespace AuthFlowPro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Permissions.User.View)]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }


    [Authorize(Policy = Permissions.User.Edit)]
    [HttpPost("assign-roles")]
    public async Task<IActionResult> AssignRoles(AssignRolesRequest request)
    {
        var result = await _userService.AssignRolesAsync(request);
        return result ? Ok("Roles updated.") : BadRequest("Failed to update roles.");
    }
}
