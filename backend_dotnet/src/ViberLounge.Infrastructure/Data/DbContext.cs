using Microsoft.EntityFrameworkCore;
using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Data
{
    public class ViberLoungeDbContext : DbContext
    {
        public ViberLoungeDbContext(DbContextOptions<ViberLoungeDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do Produto
            modelBuilder.Entity<Produto>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasPrecision(18, 2);

            // Configuração da Venda
            modelBuilder.Entity<Venda>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Venda>()
                .HasMany(v => v.Itens)
                .WithOne()
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração do ItemVenda
            modelBuilder.Entity<ItemVenda>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.PrecoUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.Subtotal)
                .HasPrecision(18, 2);

            // Configuração do Usuário
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Senha)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}