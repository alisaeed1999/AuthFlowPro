using AuthFlowPro.Application.DTOs.Identity;

namespace AuthFlowPro.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<bool> AssignRolesAsync(AssignRolesRequest request);
}

