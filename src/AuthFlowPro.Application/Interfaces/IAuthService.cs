using AuthFlowPro.Application.DTOs.Auth;

namespace AuthFlowPro.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RefreshTokenAsync(string token , string refreshToken);
}
