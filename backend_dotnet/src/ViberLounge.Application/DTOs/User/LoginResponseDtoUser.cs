using ViberLounge.API.Controllers;

public class LoginResponse
{
    public string? Token { get; set; }
    public UsuarioDto? User { get; set; }
}