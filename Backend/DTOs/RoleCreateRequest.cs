using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RoleCreateRequest
{
    [Required(ErrorMessage = "Rol adı zorunludur.")]
    [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    public bool CanAdd { get; set; } = false;
    public bool CanEdit { get; set; } = false;
    public bool CanDelete { get; set; } = false;
    public bool CanAccessDashboard { get; set; } = false;
}
