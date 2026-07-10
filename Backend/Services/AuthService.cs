using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiryInMinutes"])),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserProfileResponse?> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return null;

        return new UserProfileResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponse> LoginAsync(UserLoginRequest userLogin)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);
        if (user == null) return new AuthResponse { Success = false, ErrorMessage = "Email veya şifre hatalı." };

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash);

        if (!isPasswordValid) return new AuthResponse { Success = false, ErrorMessage = "Email veya şifre hatalı." };

        return new AuthResponse { Success = true, ErrorMessage = null, Token = GenerateJwtToken(user) };
    }

    public async Task<AuthResponse> RegisterAsync(UserRegisterRequest userRegister)
        {
            // E-posta kontrolü
            if (await _context.Users.AnyAsync(u => u.Email == userRegister.Email || u.Username == userRegister.Username))
            {
                return new AuthResponse { Success = false, ErrorMessage = "Kullanıcı adı veya email adresi zaten kullanılıyor." };
            }

            // Şifre BCrypt ile hash'leme
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);

            // Veritabanına kaydetme
            var newUser = new User
            {
                Username = userRegister.Username,
                Email = userRegister.Email,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, ErrorMessage = null, Token = GenerateJwtToken(newUser) };
        }
}
