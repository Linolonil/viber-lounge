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
            Usuario? Usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            return Usuario;
        }
        public async Task AddUserAsync(Usuario Usuario)
        {
            await _context.Usuarios.AddAsync(Usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var Usuario = await _context.Usuarios.FindAsync(id) ?? throw new KeyNotFoundException($"Usuario with ID {id} not found.");
            return Usuario;
        }

        public async Task UpdateAsync(Usuario Usuario)
        {
            _context.Usuarios.Update(Usuario);
            await _context.SaveChangesAsync();
        }

        public Task<bool> CheckPasswordAsync(string senhaHash, string senha)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(senha, senhaHash);
            return Task.FromResult(isValid);
        }
    }
}
