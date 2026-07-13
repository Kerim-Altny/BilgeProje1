using Backend.DTOs;

namespace Backend.Services;

public interface IPermissionService
{
   Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync();
   Task<IReadOnlyList<string>> GetPermissionsByRoleIdAsync(int roleId);
   Task<bool> SetPermissionsForRoleAsync(int roleId, IReadOnlyList<string> permissions);
}