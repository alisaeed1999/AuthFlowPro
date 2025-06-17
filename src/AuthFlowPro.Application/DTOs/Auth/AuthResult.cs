namespace AuthFlowPro.Application.DTOs.Auth;

public class AuthResult
{
    public string AccessToken { get; set; } = default;
    public string RefreshToken { get; set; } = default;
    public DateTime ExpiresAt { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new();
}
