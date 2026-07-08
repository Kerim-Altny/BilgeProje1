using Backend.DTOs;

namespace Backend.Services;
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(UserRegisterRequest userRegister);
    Task<AuthResponse> LoginAsync(UserLoginRequest userLogin);
    Task<UserProfileResponse?> GetProfileAsync(int userId);
}
