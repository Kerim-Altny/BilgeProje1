using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Backend.Models;

[Table("RolePermissions")]
public class RolePermission
{
    public int RoleId { get; set; }
    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }

    public int PermissionId { get; set; }
    [ForeignKey(nameof(PermissionId))]
    public Permission? Permission { get; set; }
}