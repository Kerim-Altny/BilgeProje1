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
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return result.Success ? Ok(result) : Unauthorized(result);
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
}
