using Microsoft.AspNetCore.Authorization;
namespace Backend.Auth;
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        bool hasPermission = context.User.HasClaim(c => c.Type == Permissions.ClaimType && c.Value == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}