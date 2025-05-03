using Microsoft.EntityFrameworkCore;
using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Aqui você adicionará os DbSet para suas entidades
    // Exemplo:
    // public DbSet<Usuario> Usuarios { get; set; }
    // public DbSet<Produto> Produtos { get; set; }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aqui você configurará as entidades usando Fluent API
        // Exemplo:
        // modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
        // modelBuilder.Entity<Usuario>().Property(u => u.Nome).IsRequired();
    }
} 