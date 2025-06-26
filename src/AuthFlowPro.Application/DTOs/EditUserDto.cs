namespace AuthFlowPro.Application.DTOs;

public class EditUserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
