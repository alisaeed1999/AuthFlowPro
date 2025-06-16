namespace AuthFlowPro.Application.DTOs.Auth;

public class AuthResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; }
}
