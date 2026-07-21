using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs.Links;

public class ShortLinkUpdateRequest
{
    [Required]
    [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
    public string OriginalUrl { get; set; } = string.Empty;
}