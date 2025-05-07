using ViberLounge.Domain.Entities;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<Usuario> RegisterAsync(RegisterRequest request);
}