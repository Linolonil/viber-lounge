using Microsoft.EntityFrameworkCore;
using ViberLounge.Domain.Entities;
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

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var Usuario = await _context.Usuarios.FindAsync(id) ?? throw new KeyNotFoundException($"Usuario with ID {id} not found.");
            return Usuario;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            var Usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email) ?? throw new KeyNotFoundException($"Usuario with email {email} not found.");
            return Usuario;
        }

        public async Task AddAsync(Usuario Usuario)
        {
            await _context.Usuarios.AddAsync(Usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario Usuario)
        {
            _context.Usuarios.Update(Usuario);
            await _context.SaveChangesAsync();
        }

        public Task<bool> CheckPasswordAsync(Usuario user, string senha)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(senha, user.Senha);
            return Task.FromResult(isValid);
        }
    }
}
