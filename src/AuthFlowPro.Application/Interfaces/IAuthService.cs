using AuthFlowPro.Application.DTOs.Auth;

namespace AuthFlowPro.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResult> RefreshTokenAsync(string token);
}
