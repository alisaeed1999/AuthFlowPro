namespace AuthFlowPro.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
    public bool IsUsed { get; set; } = false;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
}
