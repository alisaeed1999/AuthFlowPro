namespace AuthFlowPro.Application.DTOs.Auth;

public class AuthResponse
{
     public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
