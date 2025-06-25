using System.Security.Claims;
using AuthFlowPro.Application.DTOs.Auth;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using AuthFlowPro.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthFlowPro.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthService(UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IConfiguration configuration,
    AppDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
        _context = context;
    }
    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var existingEmailUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmailUser != null)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Errors = new List<string> { "Email is already registered" }
            };
        }

        var existingUsernameUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == request.UserName);

        if (existingUsernameUser != null)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Errors = new List<string> { "Username is already taken" }
            };
        }
        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.UserName
        };
        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        await _userManager.AddToRoleAsync(newUser, "Basic");

        var existingTokens = await _context.RefreshTokens
    .Where(rt => rt.UserId == newUser.Id && !rt.IsRevoked && !rt.IsUsed)
    .ToListAsync();

        foreach (var t in existingTokens)
        {
            t.IsUsed = true;
            t.IsRevoked = true;
        }

        var token = await _tokenService.GenerateTokenAsync(newUser);
        var refreshToken = _tokenService.GenerateRefreshToken(newUser.Id);

        // TODO: Store the refresh token in DB (later step)
        // Store refresh token in DB
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        // newUser.RefreshTokens.Add(refreshToken);
        // await _userManager.UpdateAsync(newUser);

        return new AuthResult
        {
            IsSuccess = true,
            AccessToken = token.AccessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.Expires
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return new AuthResult
            {
                IsSuccess = false,
                Errors = new List<string> { "invalid email or password" }
            };
        }

        var existingTokens = await _context.RefreshTokens
    .Where(rt => rt.UserId == user.Id && !rt.IsRevoked && !rt.IsUsed)
    .ToListAsync();

        foreach (var t in existingTokens)
        {
            t.IsUsed = true;
            t.IsRevoked = true;
        }

        var token = await _tokenService.GenerateTokenAsync(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        // You can store refresh token in DB here if needed.
        // Store refresh token in DB
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        // user.RefreshTokens.Add(refreshToken);
        // await _userManager.UpdateAsync(user);

        return new AuthResult
        {
            IsSuccess = true,
            AccessToken = token.AccessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.Expires
        };
    }

    public async Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return new AuthResult { IsSuccess = false, Errors = new() { "Invalid access token" } };
        }

        var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return new AuthResult { IsSuccess = false, Errors = new() { "Invalid user ID" } };
        }

        var user = await _userManager.Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userGuid);

        if (user == null)
            return new AuthResult { IsSuccess = false, Errors = new() { "User not found" } };

        var storedToken = user.RefreshTokens.FirstOrDefault(rt =>
            rt.Token == refreshToken &&
            rt.Expires > DateTime.UtcNow &&
            !rt.IsUsed &&
            !rt.IsRevoked);

        if (storedToken == null)
            return new AuthResult { IsSuccess = false, Errors = new() { "Invalid refresh token" } };

        storedToken.IsUsed = true;
        await _userManager.UpdateAsync(user);

        var newAccessToken = await _tokenService.GenerateTokenAsync(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();
        // user.RefreshTokens.Add(newRefreshToken);
        // await _userManager.UpdateAsync(user);

        return new AuthResult
        {
            IsSuccess = true,
            ExpiresAt = newAccessToken.Expires,
            AccessToken = newAccessToken.AccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }


}
