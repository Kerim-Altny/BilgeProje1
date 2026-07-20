namespace Backend.DTOs;

public class RolePermissionUpdateRequest
{
    public List<string> Permissions { get; set; } = new List<string>();
}