using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/api/auth");

        // /api/auth/register
        authGroup.MapPost("/register", async (UserRegisterRequest registerDto, IAuthService authService) =>
        {
            var result = await authService.RegisterAsync(registerDto);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        // /api/auth/login
        authGroup.MapPost("/login", async (UserLoginRequest logindto, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(logindto);
            return result.Success ? Results.Ok(result) : Results.Json(result, statusCode: StatusCodes.Status401Unauthorized);
        });
        authGroup.MapGet("/me", [Authorize] async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
                return Results.Unauthorized();

            var profile = await authService.GetProfileAsync(userId);
            return profile is null ? Results.Unauthorized() : Results.Ok(profile);
        });
    }
}
