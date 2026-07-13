using Backend.DTOs;

namespace Backend.Services;

public interface IPermissionService
{
   Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync();
   Task<IReadOnlyList<PermissionDto>> GetPermissionsByRoleIdAsync(int roleId);
   Task<bool> SetPermissionsForRoleAsync(int roleId, IEnumerable<int> permissionIds);
}