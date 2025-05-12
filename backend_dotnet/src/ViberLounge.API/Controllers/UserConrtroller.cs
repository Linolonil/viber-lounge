using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;

namespace ViberLounge.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AuthProfileController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Profile()
    {
        string? userId = User.FindFirst("id")?.Value;;
        string? nome = User.Identity?.Name;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userId == null || nome == null || email == null || role == null)
            return Unauthorized(new { message = "Token inválido ou informações do usuário ausentes." });

        ProfileDto profile = new()
        {
            Id = userId,
            Nome = nome,
            Email = email,
            Role = role
        };

        return Ok(profile);
    }
}