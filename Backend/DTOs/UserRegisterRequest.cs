using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;
public class UserRegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3,ErrorMessage="Kullanıcı adı 3-50 karakter olmalıdır.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 6,ErrorMessage="Şifre 6-50 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;
}
