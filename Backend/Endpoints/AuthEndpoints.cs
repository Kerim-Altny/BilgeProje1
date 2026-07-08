using Backend.DTOs;
using Backend.Services;

namespace Backend.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/api/auth");

        // /api/auth/register
        authGroup.MapPost("/register", async (UserRegister registerDto, IAuthService authService) =>
        {
            var result = await authService.RegisterAsync(registerDto);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result.ErrorMessage);
        });

        // /api/auth/login
        authGroup.MapPost("/login", async (UserLogin logindto, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(logindto);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result.ErrorMessage);
        });
    }
}
