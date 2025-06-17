using AuthFlowPro.Application.DTOs.Identity;

namespace AuthFlowPro.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<List<string>> GetAllRolesAsync();
    Task<bool> AssignRolesAsync(AssignRolesRequest request);
}

