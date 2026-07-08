using Backend.DTOs;
using Backend.Services;

namespace Backend.EndPoints;


public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group=app.MapGroup("/api/auth");

        group.MapPost("/register", async (UserRegister dto, IAuthService authService) => {var registerResult= await authService.RegisterAsync(dto);
        return registerResult.Success ? Results.Ok(registerResult) : Results.BadRequest(registerResult);});
        group.MapPost("/login", async (UserLogin dto, IAuthService authService) =>
        {
            var loginResult = await authService.LoginAsync(dto);
          return loginResult.Success ? Results.Ok(loginResult) : Results.Unauthorized();
        });
       
    }
}