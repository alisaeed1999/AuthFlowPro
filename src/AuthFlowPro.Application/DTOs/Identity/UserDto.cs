namespace AuthFlowPro.Application.DTOs.Identity;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string userName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
