using Backend.DTOs;

namespace Backend.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleResponse>> GetAllRolesAsync();
    Task<RoleResponse?> GetRoleByIdAsync(int roleId);
    Task<Result<RoleResponse>> CreateRoleAsync(RoleCreateRequest request);
    Task<Result<RoleResponse>> UpdateRoleAsync(int roleId, RoleUpdateRequest request);
    Task<Result<bool>> DeleteRoleAsync(int roleId);
}
