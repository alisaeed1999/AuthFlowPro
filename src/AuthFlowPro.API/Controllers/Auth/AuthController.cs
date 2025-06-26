using AuthFlowPro.Application.DTOs.Auth;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthFlowPro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(IAuthService authService,
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _tokenService = tokenService;
            _userManager = userManager;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt);

            return Ok(new
            {
                result.AccessToken,
                result.IsSuccess,
                result.ExpiresAt
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt);

            return Ok(new
            {
                result.AccessToken,
                result.IsSuccess,
                result.ExpiresAt
            });
        }

        // File: API/Controllers/AuthController.cs

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest? request)
        {

            var accessToken = request?.AccessToken;
            var refreshToken = Request?.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Missing tokens.");

            var result = await _authService.RefreshTokenAsync(accessToken, refreshToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt);

            return Ok(new
            {
                result.AccessToken,
                result.ExpiresAt,
                result.IsSuccess
            });
        }

        private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            if (expires < DateTime.UtcNow)
                expires = DateTime.UtcNow.AddDays(7);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Changed to false for local development to allow cookie over HTTP
                SameSite = SameSiteMode.Lax, // Changed to Lax for better compatibility
                Expires = expires
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }


    }
}

