using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RoleCreateRequest
{
    [Required(ErrorMessage = "Rol adı zorunludur.")]
    [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;


}
