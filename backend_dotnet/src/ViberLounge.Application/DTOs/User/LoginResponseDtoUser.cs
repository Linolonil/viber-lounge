using ViberLounge.API.Controllers;

namespace ViberLounge.Application.DTOs.User;

public class LoginResponse
{
    public string? Token { get; set; }
    public UserDto? User { get; set; }
}