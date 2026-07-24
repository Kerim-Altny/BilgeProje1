using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("ShortLinkClicks")]
public class ShortLinkClick
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long ShortLinkId { get; set; }

    [ForeignKey("ShortLinkId")]
    public ShortLink? ShortLink { get; set; }

    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
}
