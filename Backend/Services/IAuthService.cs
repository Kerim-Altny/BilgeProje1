using Backend.DTOs;

namespace Backend.Services;
public interface IAuthService
{
    Task<AuthResult> RegisterAsync(UserRegister userRegister);
    Task<AuthResult> LoginAsync(UserLogin userLogin);
}