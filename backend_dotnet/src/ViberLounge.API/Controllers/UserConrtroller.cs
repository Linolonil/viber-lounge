using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ViberLounge.API.Controllers;

[Route("auth")]
public class AuthProfileController : ControllerBase
{
    [HttpGet("profile")]
    [Authorize]
    public IActionResult Profile()
    {
        string? userId = GetClaim("id");
        string? nome = User.Identity?.Name;
        string? email = GetClaim("email");
        string? role = GetClaim("role");

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

    private string? GetClaim(string type)
    {
        return User.FindFirst(type)?.Value ?? User.FindFirst($"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/{type}")?.Value;
    }
}