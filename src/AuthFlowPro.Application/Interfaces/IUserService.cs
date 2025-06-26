using AuthFlowPro.Application.DTOs;
using AuthFlowPro.Application.DTOs.Auth;
using AuthFlowPro.Application.DTOs.Identity;

namespace AuthFlowPro.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<(bool Success, string Message)> CreateUserAsync(CreateUserDto user);
    Task<(bool Success, string Message)> DeleteUserAsync(Guid userId);
    Task<bool> UpdateUserAsync(UserDto user);
    Task<bool> AssignRolesAsync(AssignRolesRequest request);
}

