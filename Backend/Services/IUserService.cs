using Backend.DTOs;

namespace Backend.Services;
public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(int userId);
    Task<UserResult> CreateUserAsync(UserCreateRequest userCreateRequest);
    Task<UserResult> UpdateUserAsync(int userId, UserUpdateRequest userUpdateRequest);
    Task<bool> DeleteUserAsync(int userId);
}