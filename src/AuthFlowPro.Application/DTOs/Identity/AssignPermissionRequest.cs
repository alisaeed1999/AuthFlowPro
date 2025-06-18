namespace AuthFlowPro.Application.DTOs.Identity;

public class AssignPermissionRequest
{
    public string RoleName { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}
