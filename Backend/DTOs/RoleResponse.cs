namespace Backend.DTOs;

public class RoleResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool CanAdd { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanAccessDashboard { get; set; }
}
