using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthFlowPro.Application.DTOs;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace AuthFlowPro.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public TokenService(IConfiguration configuration,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<JwtTokenResult> GenerateTokenAsync(ApplicationUser user)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Get roles
        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));

            // Get role claims and add them to the token
            var roleEntity = await _roleManager.FindByNameAsync(role);
            if (roleEntity != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
                authClaims.AddRange(roleClaims);
            }
        }

        var jwtSettings = _configuration.GetSection("Jwt");
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtTokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            Expires = token.ValidTo
        };

    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false,
            UserId = userId,
        };
    }

    // In TokenService.cs

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // allow expired tokens
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }



}
