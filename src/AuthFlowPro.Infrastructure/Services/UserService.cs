using AuthFlowPro.Application.DTOs;
using AuthFlowPro.Application.DTOs.Identity;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthFlowPro.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                userName = user.UserName ?? "",
                Roles = roles.ToList()
            });
        }

        return result;
    }


    public async Task<(bool Success, string Message)> CreateUserAsync(CreateUserDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return (false, "Email is already taken");

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        // foreach (var role in dto.Roles.Distinct())
        // {
        //     if (!await _roleManager.RoleExistsAsync(role))
        //     {
        //         var roleCreate = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
        //         if (!roleCreate.Succeeded)
        //             return (false, $"Failed to create role: {role}");
        //     }
        // }

        var assignRoles = await _userManager.AddToRolesAsync(user, dto.Roles);
        if (!assignRoles.Succeeded)
            return (false, "User created, but failed to assign roles");

        return (true, "User created successfully");
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, "User not found");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, "User deleted successfully");
    }

    public async Task<bool> UpdateUserAsync(UserDto dto)
{
    var user = await _userManager.FindByIdAsync(dto.Id.ToString());
    if (user == null) return false;

    user.UserName = dto.userName;
    user.Email = dto.Email;

    var updateResult = await _userManager.UpdateAsync(user);
    if (!updateResult.Succeeded) return false;

    // Update roles
    var currentRoles = await _userManager.GetRolesAsync(user);
    

    await _userManager.RemoveFromRolesAsync(user, currentRoles);
    await _userManager.AddToRolesAsync(user, dto.Roles);

    return true;
}


    public async Task<bool> AssignRolesAsync(AssignRolesRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) return false;

        var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
        return addResult.Succeeded;
    }
}
