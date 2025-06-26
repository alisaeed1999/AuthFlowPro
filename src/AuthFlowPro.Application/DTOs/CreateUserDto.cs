namespace AuthFlowPro.Application.DTOs;

public class CreateUserDto
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
