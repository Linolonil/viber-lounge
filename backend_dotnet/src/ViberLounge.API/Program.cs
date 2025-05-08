using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ViberLounge.Infrastructure.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
string ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
string connectionsStrings = ENVIRONMENT == "Development" ? "ConnectionStrings:DefaultConnection" : "ConnectionStrings:ProductionConnection";

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<IAuthService, AuthService>();
configDataBase(builder);
configDependencyInjection(builder);
var app = builder.Build();

// await using (var scope = app.Services.CreateAsyncScope())
// {
//     await scope.ServiceProvider.InitializeDatabaseAsync();
// }
void configDataBase(WebApplicationBuilder serviceProvider)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
        builder.Configuration[connectionsStrings],
        options => options.SetPostgresVersion(new Version(15, 0, 0))
    ));
}

void configDependencyInjection(WebApplicationBuilder builder)
{
    configDependencyService(builder.Services);
    configDependencyRepository(builder.Services);
}

void configDependencyService(IServiceCollection services){
    builder.Services.AddScoped<IAuthService, AuthService>();
}

void configDependencyRepository(IServiceCollection services)
{
    builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions{ Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse});

app.Run();