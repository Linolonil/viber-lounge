using Microsoft.AspNetCore.Mvc;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;

namespace ViberLounge.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Realiza o login do usuário
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <returns>Token de autenticação</returns>
    /// <response code="200">Retorna o token de autenticação</response>
    /// <response code="401">Se as credenciais forem inválidas</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="request">Dados do novo usuário</param>
    /// <returns>Confirmação de registro</returns>
    /// <response code="201">Usuário registrado com sucesso</response>
    /// <response code="400">Se os dados do usuário forem inválidos</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            Usuario? newUser = await _authService.RegisterAsync(request);
            return Created(string.Empty, new { message = "Usuário cadastrado com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}