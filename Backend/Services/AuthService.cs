using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Auth;
using Backend.Data;
using Backend.DTOs;

using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(AppDbContext context, IConfiguration configuration, IMapper mapper)
    {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
    }
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Username),
            new Claim(ClaimTypes.Role, user.Role!.Name)
        };

       if(user.Role.RolePermissions != null)
        {
            foreach (var rp in user.Role.RolePermissions)
            {
                if(rp.Permission != null)
                {
                    claims.Add(new Claim(Permissions.ClaimType, rp.Permission.Name));
                }
            }
        }

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
        var user = await _context.Users.Include(u => u.Role).ThenInclude(r => r!.RolePermissions).ThenInclude(rp => rp.Permission).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return null;

       return _mapper.Map<UserProfileResponse>(user);
    }

    public async Task<AuthResponse> LoginAsync(UserLoginRequest userLogin)
    {
        var user = await _context.Users.Include(u => u.Role).ThenInclude(r => r!.RolePermissions).ThenInclude(rp => rp.Permission).FirstOrDefaultAsync(u => u.Email == userLogin.Email);
        if (user == null) return new AuthResponse { Success = false, ErrorMessage = "Email veya şifre hatalı." };

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash);

        if (!isPasswordValid) return new AuthResponse { Success = false, ErrorMessage = "Email veya şifre hatalı." };

        return new AuthResponse { Success = true, ErrorMessage = null, Token = GenerateJwtToken(user), Role = user.Role!.Name };
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

            var defaultRole = await _context.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).FirstOrDefaultAsync(r => r.Name.ToLower() == "user")
                ?? await _context.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).OrderBy(r => r.Id).FirstOrDefaultAsync();

            if (defaultRole is null)
            {
                return new AuthResponse { Success = false, ErrorMessage = "Sistemde tanımlı bir rol bulunamadı." };
            }

            // Veritabanına kaydetme
            var newUser = new User
            {
                Username = userRegister.Username,
                Email = userRegister.Email,
                PasswordHash = hashedPassword,
                RoleId = defaultRole.Id
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            newUser.Role = defaultRole;

            return new AuthResponse { Success = true, ErrorMessage = null, Token = GenerateJwtToken(newUser), Role = defaultRole.Name };
        }
}
