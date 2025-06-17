using System.Security.Claims;
using AuthFlowPro.Application.DTOs;
using AuthFlowPro.Domain.Entities;

namespace AuthFlowPro.Application.Interfaces;

public interface ITokenService
{
    Task<JwtTokenResult> GenerateTokenAsync(ApplicationUser user);
    RefreshToken GenerateRefreshToken(Guid userId);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
}
