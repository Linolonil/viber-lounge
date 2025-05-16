using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using ViberLounge.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;

namespace ViberLounge.Tests.TestUtils
{

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IDisposable where TStartup : class
    {
        private IServiceScope? _scope;
        private string _databaseName;
        private readonly string _environmentName;
        private IServiceProvider? _serviceProvider;

        public CustomWebApplicationFactory(string environmentName = "Development")
        {
            _environmentName = environmentName;
            _databaseName = Guid.NewGuid().ToString();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(_environmentName);

            builder.ConfigureServices(services =>
            {
                // Remove configurações anteriores do ApplicationDbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Permite configuração adicional por override/subclasse
                ConfigureAdditionalServices(services);

                // Adiciona ApplicationDbContext usando banco em memória
                services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
                {
                    // Adiciona logging de EF Core para diagnóstico
                    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                    options.UseInMemoryDatabase(_databaseName)
                        .UseLoggerFactory(loggerFactory)
                        .EnableSensitiveDataLogging();
                });

                // Constrói provider apenas ao final, guarda para uso em seed/reset/cleanup
                _serviceProvider = services.BuildServiceProvider();

            });
        }

        protected virtual void ConfigureAdditionalServices(IServiceCollection services)
        {
            // Exemplo: services.AddSingleton<IMyService, MyMockService>();
        }


        // Inicializa o banco de dados e executa seed de dados.
        // Chame esse método no setup dos testes, se precisar.
        // public virtual async Task SeedDatabaseAsync()
        // {
        //     using var scope = _serviceProvider.CreateScope();
        //     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //     await db.Database.EnsureCreatedAsync();
        // }


        // Remove todos os dados do banco de dados. Útil para garantir isolamento.
        public virtual async Task ResetDatabaseAsync()
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service provider is not initialized.");
            }
            
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.RemoveRange(db.ChangeTracker.Entries().Select(e => e.Entity));
            await db.SaveChangesAsync();

            // Alternativa: recriar o banco, dependendo do seu modelo
            // await db.Database.EnsureDeletedAsync();
            // await db.Database.EnsureCreatedAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scope?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
