using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViberLounge.Domain.Entities;

namespace ViberLounge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Nome) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Senha))
            {
                throw new ArgumentException("Nome, email e senha são obrigatórios.");
            }

            Usuario? newUser = await _authService.RegisterAsync(request);
            
            return StatusCode(201, newUser);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout realizado com sucesso" });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult Profile()
    {
        var userId = User.FindFirst("id")?.Value;
        var nome = User.Identity?.Name;
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("role")?.Value;

        if (userId == null)
            return Unauthorized(new { message = "Token não fornecido ou inválido" });

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Role = "ADMIN"
        };

        return Ok(usuario);
    }
}