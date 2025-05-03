using Microsoft.EntityFrameworkCore;
using ViberLounge.Domain.Entities;
using ViberLounge.Infrastructure.Data;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ViberLoungeDbContext _context;

        public UsuarioRepository(ViberLoungeDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id) ?? throw new KeyNotFoundException($"Usuario with ID {id} not found.");
            return usuario;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email) ?? throw new KeyNotFoundException($"Usuario with email {email} not found.");
            return usuario;
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
