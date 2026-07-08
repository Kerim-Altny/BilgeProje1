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
            new Claim(ClaimTypes.Name,user.Username)
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

    public async Task<AuthResult> LoginAsync(UserLogin userLogin)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);
        if (user == null) return new AuthResult { Success = false, ErrorMessage = "Kullanıcı adı veya şifre hatalı." };
        
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash);
        
        if (!isPasswordValid) return new AuthResult { Success = false, ErrorMessage = "Kullanıcı adı veya şifre hatalı." };
        
        return new AuthResult { Success = true, ErrorMessage = null, Token = GenerateJwtToken(user) };
    }

    public async Task<AuthResult> RegisterAsync(UserRegister userRegister)
    {
       throw new NotImplementedException();
    }
}
