using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Auth;
using Backend.Services;
using Backend.DTOs;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/roles")]
public class RolesController(IRoleService roleService, IPermissionService permissionService) : ControllerBase
{
    [HttpGet]
    [HasPermission("Roles.View")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    [HasPermission("Roles.View")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var role = await roleService.GetRoleByIdAsync(id);
        return role is not null ? Ok(role) : NotFound();
    }

    [HttpPost]
    [HasPermission("Roles.Create")]
    public async Task<IActionResult> CreateRole(RoleCreateRequest request)
    {
        var result = await roleService.CreateRoleAsync(request);
        return result.Status switch
        {
            RoleResultStatus.Success => CreatedAtAction(nameof(GetRoleById), new { id = result.Data!.Id }, result.Data),
            RoleResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

    [HttpPut("{id:int}")]
    [HasPermission("Roles.Edit")]
    public async Task<IActionResult> UpdateRole(int id, RoleUpdateRequest request)
    {
        var result = await roleService.UpdateRoleAsync(id, request);
        return result.Status switch
        {
            RoleResultStatus.Success => Ok(result.Data),
            RoleResultStatus.NotFound => NotFound(),
            RoleResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("{id:int}")]
    [HasPermission("Roles.Delete")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var success = await roleService.DeleteRoleAsync(id);
        return success ? NoContent() : NotFound();
    }
    [HttpGet("{id:int}/permissions")]
    [HasPermission("Permissions.View")]
    public async Task<IActionResult> GetRolePermissions(int id)
    {
        var permissions = await permissionService.GetPermissionsByRoleIdAsync(id);
        return Ok(permissions);
    }
    [HttpPut("{id:int}/permissions")]
    [HasPermission("Permissions.Assign")]
    public async Task<IActionResult> UpdateRolePermissions(int id, RolePermissionUpdateRequest request)
    {
        var success = await permissionService.SetPermissionsForRoleAsync(id, request.Permissions);
        return success ? Ok() : NotFound();
    }
}
