namespace Backend.DTOs;
public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
}
