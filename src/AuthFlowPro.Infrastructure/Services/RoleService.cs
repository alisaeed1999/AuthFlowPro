using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Application.DTOs.Identity;
using Microsoft.AspNetCore.Identity;
using AuthFlowPro.Application.Permission;


namespace AuthFlowPro.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleService(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var roles = _roleManager.Roles.ToList();
        var result = new List<RoleDto>();

        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            result.Add(new RoleDto
            {
                RoleName = role.Name!,
                Permissions = claims.Select(c => c.Type == "permission" ? c.Value : null)
                                     .Where(v => v != null).ToList()!
            });
        }
        return result;
    }

    public async Task<bool> CreateRoleAsync(CreateRoleRequest request)
    {
        if (await _roleManager.RoleExistsAsync(request.RoleName))
            return false;

        var role = new IdentityRole<Guid>(request.RoleName);
        var createResult = await _roleManager.CreateAsync(role);
        if (!createResult.Succeeded) return false;

        foreach (var perm in request.Permissions)
            await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", perm));

        return true;
    }

    public async Task<bool> AssignPermissionsAsync(AssignPermissionRequest request)
    {
        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role == null) return false;

        var current = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in current)
            if (claim.Type == "permission")
                await _roleManager.RemoveClaimAsync(role, claim);

        foreach (var perm in request.Permissions)
            await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", perm));

        return true;
    }

    public async Task<bool> EditRoleAsync(EditRoleRequest request)
    {
        var role = await _roleManager.FindByNameAsync(request.OldRoleName);
        if (role == null) return false;

        if (!string.Equals(role.Name, request.NewRoleName, StringComparison.OrdinalIgnoreCase))
        {
            role.Name = request.NewRoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded) return false;
        }

        // Clear old permissions and assign new ones
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in currentClaims.Where(c => c.Type == "permission"))
            await _roleManager.RemoveClaimAsync(role, claim);

        foreach (var perm in request.Permissions)
            await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", perm));

        return true;
    }

    public async Task<bool> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return false;

        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }


    public Task<List<string>> GetAllPermissionsAsync()
{
    // This can be replaced with a database or static source
    return Task.FromResult(Permissions.PermissionHelper.GetAllPermissions());
}
}
