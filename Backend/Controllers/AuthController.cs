using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterRequest registerDto)
    {
        var result = await authService.RegisterAsync(registerDto);
        if (!result.Success) return BadRequest(result);

        SetTokenCookies(result.Token!, result.RefreshToken!, rememberMe: false);
        return Ok(result);
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        if (!result.Success) return Unauthorized(result);

        SetTokenCookies(result.Token!, result.RefreshToken!, loginDto.RememberMe);
        return Ok(result);
    }

    // GET /api/auth/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var profile = await authService.GetProfileAsync(userId);
        return profile is null ? Unauthorized() : Ok(profile);
    }

    // POST /api/auth/refresh-token
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest? refreshTokenRequest)
    {
        var token = string.IsNullOrEmpty(refreshTokenRequest?.Token) ? Request.Cookies["token"] : refreshTokenRequest.Token;
        var refreshToken = string.IsNullOrEmpty(refreshTokenRequest?.RefreshToken) ? Request.Cookies["refreshToken"] : refreshTokenRequest.RefreshToken;

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new AuthResponse { Success = false, ErrorMessage = "Token bilgileri eksik." });

        var request = new RefreshTokenRequest { Token = token, RefreshToken = refreshToken };
        var result = await authService.RefreshTokenAsync(request);
        if (!result.Success) return Unauthorized(result);

        SetTokenCookies(result.Token!, result.RefreshToken!, refreshTokenRequest?.RememberMe ?? false);
        return Ok(result);
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token", new CookieOptions { Path = "/", Secure = true, SameSite = SameSiteMode.None });
        Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/api/auth", Secure = true, SameSite = SameSiteMode.None });
        return Ok(new { message = "Çıkış yapıldı." });
    }

    private void SetTokenCookies(string token, string refreshToken, bool rememberMe)
    {
        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = rememberMe ? DateTimeOffset.UtcNow.AddHours(1) : null
        });

        // "Beni Hatırla" işaretliyse tarayıcı kapansa da kalıcı, değilse sadece o oturum boyunca (session cookie).
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/api/auth",
            Expires = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
        });
    }
}
