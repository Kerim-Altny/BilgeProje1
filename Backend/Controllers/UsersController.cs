using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Auth;
using Backend.Services;
using Backend.DTOs;
namespace Backend.Controllers;

[ApiController]
[Authorize] 
[Route("api/users")]
public class UsersController(IUserService userService): ControllerBase    
{
    // GET /api/users
    [HttpGet]
    [HasPermission("Users.View")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    // GET /api/users/{id}
    [HttpGet("{id:int}")]
    [HasPermission("Users.View")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user is not null ? Ok(user) : NotFound();
    }

    // POST /api/users 
    [HttpPost]
    [HasPermission("Users.Create")]
    public async Task<IActionResult> CreateUser(UserCreateRequest createRequest)
    {
        var result = await userService.CreateUserAsync(createRequest);
        return result.Status switch
        {
            UserResultStatus.Success => CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result.Data),
            UserResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

    // PUT /api/users/{id}

    [HttpPut("{id:int}")]
    [HasPermission("Users.Edit")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateRequest updateRequest)
    {
        var result = await userService.UpdateUserAsync(id, updateRequest);
        return result.Status switch
        {
            UserResultStatus.Success => Ok(result.Data),
            UserResultStatus.NotFound => NotFound(),
            UserResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id:int}")]
    [HasPermission("Users.Delete")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var success = await userService.DeleteUserAsync(id);
        return success ? NoContent() : NotFound();
    }

}