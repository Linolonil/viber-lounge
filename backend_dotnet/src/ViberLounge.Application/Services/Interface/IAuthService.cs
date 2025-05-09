using ViberLounge.Domain.Entities;

public interface IAuthService
{
    Task<Usuario> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}