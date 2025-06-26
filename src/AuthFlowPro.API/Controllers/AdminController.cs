using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Application.DTOs.Identity;
using AuthFlowPro.Application.Permission;
using AuthFlowPro.Application.DTOs;

namespace AuthFlowPro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Policy = Permissions.User.View)]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    // [Authorize(Policy = Permissions.User.View)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPost("create-user")]
    // [Authorize(Policy = Permissions.User.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
    {
        var (success, message) = await _userService.CreateUserAsync(user);
        if (!success)
            return BadRequest(new { message = message });
        return Ok(new { message = message });
    }


    [HttpDelete("delete-user/{userId}")]
    // [Authorize(Policy = Permissions.User.Delete)]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var (success, message) = await _userService.DeleteUserAsync(userId);
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }

    [HttpPut("edit-user")]
    // [Authorize(Policy = Permissions.User.Edit)]
    public async Task<IActionResult> EditUser([FromBody] EditUserDto dto)
    {
        var success = await _userService.UpdateUserAsync(dto);
        if (!success) return BadRequest("Failed to update user");
        return Ok(new { Message = "User updated successfully" });
    }

    // [Authorize(Policy = Permissions.User.Edit)]
    [HttpPost("assign-roles")]
    public async Task<IActionResult> AssignRoles(AssignRolesRequest request)
    {
        var result = await _userService.AssignRolesAsync(request);
        return result ? Ok("Roles updated.") : BadRequest("Failed to update roles.");
    }
}
