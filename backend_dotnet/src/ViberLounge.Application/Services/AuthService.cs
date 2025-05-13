using System.Text;
using System.Security.Claims;
using ViberLounge.API.Controllers;
using ViberLounge.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ViberLounge.Application.DTOs.User;
using Microsoft.Extensions.Configuration;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{

    public class AuthService : IAuthService
    {
        private readonly ILoggerService _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _userRepository;

        public AuthService(ILoggerService logger, IUsuarioRepository userRepository, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<Usuario> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("Iniciando o registro do usuário.");
            try
            {
                var existingUser = await _userRepository.IsEmailExists(request.Email!);
                if (existingUser != null)
                    throw new Exception("Já existe um usuário com este email.");

                string? senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
                if (senhaHash == null)
                    throw new Exception("Erro ao gerar o hash da senha.");

                Usuario novoUsuario = new()
                {
                    Nome = request.Nome,
                    Email = request.Email,
                    Senha = senhaHash
                };
                
                await _userRepository.AddUserAsync(novoUsuario);

                return novoUsuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(new Exception("Erro ao registrar o usuário: {Message}") , "Usuário não encontrado.");
                throw new Exception(ex.Message);
            }
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Iniciando o login do usuário.");
            try
            {
                Usuario? user = await _userRepository.IsEmailExists(request.Email!);
                if (user == null)
                    throw new Exception("Usuário não encontrado.");

                if (!await _userRepository.CheckPasswordAsync(user.Senha!, request.Senha!))
                    throw new Exception("Email ou senha inválidos.");

                var token = GenerateToken(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nome = user.Nome,
                    Role = user.Role
                };

                return new LoginResponse { User = userDto, Token = token };      
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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