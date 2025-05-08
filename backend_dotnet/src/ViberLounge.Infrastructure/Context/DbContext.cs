using ViberLounge.Entities;
using ViberLounge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ViberLounge.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<VendaItem> ItensVendas { get; set; }
        public DbSet<VendaCancelada> VendasCanceladas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){ }

        public override int SaveChanges()
        {
            AddTimestamps(); 
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }
        public void BaseEntityTimeStamp(){
            AddTimestamps();
        }
        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Produto ↔ VendaItem (1:N)
            modelBuilder.Entity<Produto>(b =>
            {
                b.ToTable("Produtos");
                b.HasKey(x => x.Id);

                b.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

                b.HasMany(x => x.VendaItens)
                .WithOne(x => x.Produto)
                .HasForeignKey(x => x.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // Usuario ↔ Venda (1:N) e ↔ VendaCancelada (1:N)
            modelBuilder.Entity<Usuario>(b =>
            {
                b.ToTable("Usuarios");
                b.HasKey(x => x.Id);

                b.Property(x => x.Role)
                .HasConversion<string>()
                .IsRequired();

                b.HasMany(x => x.Vendas)
                .WithOne(x => x.Usuario)
                .HasForeignKey(x => x.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(x => x.VendasCanceladas)
                .WithOne(x => x.Usuario)
                .HasForeignKey(x => x.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // Venda ↔ Usuario (N:1), Venda ↔ VendaItem (1:N), Venda ↔ VendaCancelada (1:1)
            modelBuilder.Entity<Venda>(b =>
            {
                b.ToTable("Vendas");
                b.HasKey(x => x.Id);

                b.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

                b.Property(x => x.FormaPagamento)
                .HasConversion<string>()
                .IsRequired();

                b.HasOne(x => x.Usuario)
                .WithMany(x => x.Vendas)
                .HasForeignKey(x => x.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(x => x.Itens)
                .WithOne(x => x.Venda)
                .HasForeignKey(x => x.IdVenda)
                .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.VendaCancelada)
                .WithOne(x => x.Venda)
                .HasForeignKey<VendaCancelada>(x => x.IdVenda)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // VendaItem ↔ Venda (N:1), VendaItem ↔ Produto (N:1), VendaItem ↔ VendaCancelada (1:1 opcional)
            modelBuilder.Entity<VendaItem>(b =>
            {
                b.ToTable("VendaItens");
                b.HasKey(x => x.Id);

                b.HasOne(x => x.Venda)
                .WithMany(x => x.Itens)
                .HasForeignKey(x => x.IdVenda)
                .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Produto)
                .WithMany(x => x.VendaItens)
                .HasForeignKey(x => x.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Cancelamento)
                .WithOne(x => x.VendaItem)
                .HasForeignKey<VendaCancelada>(x => x.IdVendaItem)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // VendaCancelada ↔ Venda, VendaCancelada ↔ VendaItem, VendaCancelada ↔ Usuario
            modelBuilder.Entity<VendaCancelada>(b =>
            {
                b.ToTable("VendasCanceladas");
                b.HasKey(x => x.Id);

                b.Property(x => x.TipoCancelamento)
                .HasConversion<string>()
                .IsRequired();

                // Os relacionamentos com Venda e VendaItem já estão definidos acima,
                // basta garantir a FK com Usuario:
                b.HasOne(x => x.Usuario)
                .WithMany(x => x.VendasCanceladas)
                .HasForeignKey(x => x.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}