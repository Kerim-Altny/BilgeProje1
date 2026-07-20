using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllUsersAsync()
    {
        var users = await _context.Users.Include(u => u.Role).ToListAsync();
        return _mapper.Map<List<UserResponse>>(users);
    }

    public async Task<UserResponse?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return null;

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<Result<UserResponse>> CreateUserAsync(UserCreateRequest userCreateRequest)
    {
        if (await _context.Users.AnyAsync(u => u.Email.ToLower() == userCreateRequest.Email.ToLower() || u.Username.ToLower() == userCreateRequest.Username.ToLower()))
        {
            return Result<UserResponse>.Conflict("Kullanıcı adı veya e-posta zaten kullanılıyor.");
        }

        var newUser = new User
        {
            Username = userCreateRequest.Username,
            Email = userCreateRequest.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateRequest.Password),

            RoleId = userCreateRequest.RoleId
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Result<UserResponse>.Ok(_mapper.Map<UserResponse>(newUser));
    }

    public async Task<Result<UserResponse>> UpdateUserAsync(int userId, UserUpdateRequest userUpdateRequest)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Result<UserResponse>.NotFound();

        if (await _context.Users.AnyAsync(u => u.Id != userId &&
                (u.Email.ToLower() == userUpdateRequest.Email.ToLower() || u.Username.ToLower() == userUpdateRequest.Username.ToLower())))
        {
            return Result<UserResponse>.Conflict("Kullanıcı adı veya e-posta zaten kullanılıyor.");
        }

        if (!string.IsNullOrEmpty(userUpdateRequest.Username))
            user.Username = userUpdateRequest.Username;

        if (!string.IsNullOrEmpty(userUpdateRequest.Email))
            user.Email = userUpdateRequest.Email;

        if (!string.IsNullOrEmpty(userUpdateRequest.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userUpdateRequest.Password);

        if (userUpdateRequest.RoleId.HasValue)
            user.RoleId = userUpdateRequest.RoleId.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<UserResponse>.Ok(_mapper.Map<UserResponse>(user));
    }

    public async Task<Result<bool>> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Result<bool>.NotFound();

        if (user.Role?.Name == "SuperAdmin")
        {
            return Result<bool>.Conflict("Sistem yöneticisi (SuperAdmin) silinemez.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }
}