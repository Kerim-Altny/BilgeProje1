using Backend.DTOs;

namespace Backend.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(int userId);
    Task<Result<UserResponse>> CreateUserAsync(UserCreateRequest userCreateRequest);
    Task<Result<UserResponse>> UpdateUserAsync(int userId, UserUpdateRequest userUpdateRequest);
    Task<Result<bool>> DeleteUserAsync(int userId);
    Task<Result<bool>> DeleteUsersAsync(IEnumerable<int> userIds);
}