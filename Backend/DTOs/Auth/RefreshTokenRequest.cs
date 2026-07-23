namespace Backend.DTOs;

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
