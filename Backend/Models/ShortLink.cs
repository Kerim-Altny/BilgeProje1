using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("ShortLinks")]
public class ShortLink
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    [Required]
    [MaxLength(2048)]
    public string OriginalUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ShortCode { get; set; } = string.Empty;

    [Required]
    public int CreatedByUserId { get; set; }

    [ForeignKey("CreatedByUserId")]
    public User? CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpirationDate { get; set; }

    public long ClickCount { get; set; } = 0;
}