using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<UserResponse?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return null;

        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserResult> CreateUserAsync(UserCreateRequest userCreateRequest)
    {
        if (await _context.Users.AnyAsync(u => u.Email.ToLower() == userCreateRequest.Email.ToLower() || u.Username.ToLower() == userCreateRequest.Username.ToLower()))
        {
            return new UserResult { Status = UserResultStatus.Conflict, Message = "Kullanıcı adı veya e-posta zaten kullanılıyor." };
        }

        var newUser = new User
        {
            Username = userCreateRequest.Username,
            Email = userCreateRequest.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateRequest.Password),
            
            Role = userCreateRequest.Role 
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return new UserResult
        {
            Status = UserResultStatus.Success,
            Data = new UserResponse
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                Role = newUser.Role,
                CreatedAt = newUser.CreatedAt,
                UpdatedAt = newUser.UpdatedAt
            }
        };
    }

    public async Task<UserResult> UpdateUserAsync(int userId, UserUpdateRequest userUpdateRequest)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return new UserResult { Status = UserResultStatus.NotFound };

        if (await _context.Users.AnyAsync(u => u.Id != userId &&
                (u.Email.ToLower() == userUpdateRequest.Email.ToLower() || u.Username.ToLower() == userUpdateRequest.Username.ToLower())))
        {
            return new UserResult { Status = UserResultStatus.Conflict, Message = "Kullanıcı adı veya e-posta zaten kullanılıyor." };
        }

        if (!string.IsNullOrEmpty(userUpdateRequest.Username))
            user.Username = userUpdateRequest.Username;

        if (!string.IsNullOrEmpty(userUpdateRequest.Email))
            user.Email = userUpdateRequest.Email;

        if (!string.IsNullOrEmpty(userUpdateRequest.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userUpdateRequest.Password);

        if (!string.IsNullOrEmpty(userUpdateRequest.Role))
            user.Role = userUpdateRequest.Role;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new UserResult
        {
            Status = UserResultStatus.Success,
            Data = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}