namespace AuthFlowPro.Application.DTOs.Identity;

public class AssignRolesRequest
{
    public Guid UserId { get; set; }
    public List<string> Roles { get; set; } = new();
}
