namespace AuthFlowPro.Application.DTOs.Identity;

public class EditRoleRequest
{
    public string OldRoleName { get; set; } = null!;
    public string NewRoleName { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
}
