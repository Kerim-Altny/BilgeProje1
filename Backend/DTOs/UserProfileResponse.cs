namespace Backend.DTOs;

// [Authorize] korumalı endpoint'in döndürdüğü, giriş yapmış kullanıcı bilgisi.
public class UserProfileResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
