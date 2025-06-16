namespace AuthFlowPro.Application.DTOs;

public class JwtTokenResult
{
     public string AccessToken { get; set; } = default!;
    public DateTime Expires { get; set; }
}
