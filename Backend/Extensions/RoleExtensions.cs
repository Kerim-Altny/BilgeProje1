using Backend.Auth;
using Backend.Models;

namespace Backend.Extensions;

public static class RoleExtensions
{
    private static readonly Dictionary<string, string> LegacyPermissionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Users.Create"] = Permissions.CanAdd,
        ["Users.Edit"] = Permissions.CanEdit,
        ["Users.Delete"] = Permissions.CanDelete,
        ["Dashboard.Access"] = Permissions.CanAccessDashboard,
    };

    public static bool HasLegacyPermission(this Role? role, string legacyPermission)
    {
        if (role is null) return false;

        return role.RolePermissions.Any(rp =>
        {
            if (rp.Permission?.Name is not { } name) return false;

            if (string.Equals(name, legacyPermission, StringComparison.OrdinalIgnoreCase))
                return true;

            return LegacyPermissionMap.TryGetValue(name, out var mapped) &&
                   string.Equals(mapped, legacyPermission, StringComparison.OrdinalIgnoreCase);
        });
    }

    public static IEnumerable<string> GetPermissionClaims(this Role role)
    {
        var claims = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rolePermission in role.RolePermissions.Where(rp => rp.Permission != null))
        {
            var name = rolePermission.Permission!.Name;
            claims.Add(name);

            if (LegacyPermissionMap.TryGetValue(name, out var legacy))
                claims.Add(legacy);
        }

        return claims;
    }
}
