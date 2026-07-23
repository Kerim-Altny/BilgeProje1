using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Auth;
using Backend.Services;
using Backend.DTOs;
namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
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
            ResultStatus.Success => CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result.Data),
            ResultStatus.Conflict => Conflict(new { message = result.Message }),
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
            ResultStatus.Success => Ok(result.Data),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id:int}")]
    [HasPermission("Users.Delete")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await userService.DeleteUserAsync(id);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

    // DELETE /api/users
    [HttpDelete]
    [HasPermission("Users.Delete")]
    public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<int> ids)
    {
        var result = await userService.DeleteUsersAsync(ids);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

}