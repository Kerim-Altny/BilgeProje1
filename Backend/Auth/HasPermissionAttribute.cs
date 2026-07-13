using Microsoft.AspNetCore.Authorization;

namespace Backend.Auth;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(permission)
    {
    }
}