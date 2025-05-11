using ViberLounge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Usuario?> IsEmailExists(string email)
        {
            Usuario? usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            return usuario;
        }
        public async Task AddUserAsync(Usuario Usuario)
        {
            await _context.Usuarios.AddAsync(Usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, Usuario input)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return;

            usuario.Nome = input.Nome;
            usuario.Email = input.Email;
            usuario.Senha = input.Senha;

            await _context.SaveChangesAsync();
        }

        public Task<bool> CheckPasswordAsync(string senhaHash, string senha)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(senha, senhaHash);
            return Task.FromResult(isValid);
        }

        public Task<bool> UserExistsAsync(int id)
        {
            bool exists = _context.Usuarios.AsNoTracking().Any(u => u.Id == id);
            return Task.FromResult(exists);
        }
    }
}
