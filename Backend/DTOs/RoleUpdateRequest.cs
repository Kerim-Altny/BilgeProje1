using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RoleUpdateRequest
{
    [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
    public string? Name { get; set; }


}
