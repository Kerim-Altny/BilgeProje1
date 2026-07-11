using Backend.DTOs;

namespace Backend.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleResponse>> GetAllRolesAsync();
    Task<RoleResponse?> GetRoleByIdAsync(int roleId);
    Task<RoleResult> CreateRoleAsync(RoleCreateRequest request);
    Task<RoleResult> UpdateRoleAsync(int roleId, RoleUpdateRequest request);
    Task<bool> DeleteRoleAsync(int roleId);
}
