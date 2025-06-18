namespace AuthFlowPro.Application.DTOs.Identity;

public class RoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}
