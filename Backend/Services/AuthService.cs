using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Auth;
using Backend.Data;
using Backend.DTOs;

using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using System.Security.Cryptography;

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

        if (user.Role.RolePermissions != null)
        {
            foreach (var rp in user.Role.RolePermissions)
            {
                if (rp.Permission != null)
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
    private string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
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

        user.RefreshToken = GenerateRefreshToken();
        var expiryDays = Convert.ToDouble(_configuration["JWT:RefreshTokenExpiryInDays"] ?? "7");
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(expiryDays);
        await _context.SaveChangesAsync();

        return new AuthResponse { Success = true, ErrorMessage = null, Token = GenerateJwtToken(user), RefreshToken = user.RefreshToken, Role = user.Role!.Name };
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

        newUser.RefreshToken = GenerateRefreshToken();

        var expiryDays = Convert.ToDouble(_configuration["JWT:RefreshTokenExpiryInDays"] ?? "7");
        newUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(expiryDays);

        await _context.SaveChangesAsync();

        return new AuthResponse { Success = true, ErrorMessage = null, Token = GenerateJwtToken(newUser), RefreshToken = newUser.RefreshToken, Role = defaultRole.Name };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
        {
            return new AuthResponse { Success = false, ErrorMessage = "Geçersiz token." };
        }

        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return new AuthResponse { Success = false, ErrorMessage = "Token içerisinde kullanıcı ID bulunamadı." };
        }

        var user = await _context.Users.Include(u => u.Role).ThenInclude(r => r!.RolePermissions).ThenInclude(rp => rp.Permission).FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            return new AuthResponse { Success = false, ErrorMessage = "Geçersiz veya süresi dolmuş refresh token." };
        }

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        var expiryDays = Convert.ToDouble(_configuration["JWT:RefreshTokenExpiryInDays"] ?? "7");
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(expiryDays);

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            Role = user.Role!.Name
        };
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _configuration["JWT:Issuer"],
            ValidAudience = _configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
