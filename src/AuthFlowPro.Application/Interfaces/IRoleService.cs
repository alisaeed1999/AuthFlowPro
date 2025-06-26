using AuthFlowPro.Application.DTOs;
using AuthFlowPro.Application.DTOs.Identity;

namespace AuthFlowPro.Application.Interfaces;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<List<string>> GetAllPermissionsAsync();

    Task<bool> CreateRoleAsync(CreateRoleRequest request);
    Task<bool> EditRoleAsync(EditRoleRequest request);
    Task<bool> DeleteRoleAsync(string roleName);
    Task<bool> AssignPermissionsAsync(AssignPermissionRequest request);
}
