using System.Text;
using System.Security.Claims;
using ViberLounge.API.Controllers;
using ViberLounge.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{

    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _userRepository;

        private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Usuario> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var existingUser = await _userRepository.IsEmailExists(request.Email!);
                if (existingUser != null)
                    throw new InvalidOperationException("Já existe um usuário com este email.");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }

            string? senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            Usuario novoUsuario = new()
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = senhaHash
            };

            await _userRepository.AddUserAsync(novoUsuario);

            return novoUsuario;
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            Usuario? user = await _userRepository.IsEmailExists(request.Email!);
            if (user == null || !await _userRepository.CheckPasswordAsync(user.Senha!, request.Senha!))
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos.");
            }

            var token = GenerateToken(user);

            var userDto = new UsuarioDto
            {
                Id = user.Id,
                Email = user.Email,
                Nome = user.Nome,
                Role = user.Role
            };

            return new LoginResponse { User = userDto, Token = token };
        }


        private string GenerateToken(Usuario user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Nome!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role ?? "USER")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}