using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.User;

public interface IAuthService
{
    Task<Usuario> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}