using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs.Links;

public class ShortLinkCreateRequest
{
    [Required]
    [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
    public string OriginalUrl { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 3, ErrorMessage = "Özel kod 3-20 karakter olmalıdır.")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Sadece harf, rakam, tire ve alt çizgi kullanılabilir.")]
    public string? CustomCode { get; set; }
}