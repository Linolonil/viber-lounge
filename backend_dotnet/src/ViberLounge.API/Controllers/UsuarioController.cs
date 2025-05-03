using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UsuarioDto>>> GetAll()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetById(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        if (usuario == null) return NotFound();
        return Ok(usuario);
    }

    // [HttpPost]
    // [Authorize(Roles = "Admin")]
    // public async Task<ActionResult<UsuarioDto>> Create(CriarUsuarioCommand command)
    // {
    //     var usuario = await _usuarioService.CreateAsync(command);
    //     return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    // }

    // [HttpPost("login")]
    // [AllowAnonymous]
    // public async Task<ActionResult<LoginResponseDto>> Login(LoginCommand command)
    // {
    //     var response = await _usuarioService.LoginAsync(command);
    //     return Ok(response);
    // }
}