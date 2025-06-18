using AuthFlowPro.Application.DTOs.Identity;

namespace AuthFlowPro.Application.Interfaces;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<bool> CreateRoleAsync(CreateRoleRequest request);
    Task<bool> AssignPermissionsAsync(AssignPermissionRequest request);
}
