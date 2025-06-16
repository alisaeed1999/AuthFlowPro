using AuthFlowPro.Application.DTOs.Auth;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AuthFlowPro.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
    }
    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Errors = new List<string> { "Email already is registered" }
            };
        }

        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email
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

        //TODO : Generate JWT and refreshToken 
        return new AuthResult
        {
            IsSuccess = true,
            AccessToken = "accessToken",
            RefreshToken = "refreshToken"
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        throw new UnauthorizedAccessException("Invalid credentials");

    var token = _tokenService.GenerateToken(user);
    var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

    // You can store refresh token in DB here if needed.

    return new AuthResponse
    {
        Token = token.AccessToken,
        RefreshToken = refreshToken.Token,
        ExpiresAt = token.Expires
    };
    }

    public Task<AuthResult> RefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

}
