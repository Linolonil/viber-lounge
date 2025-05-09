using System.Text;
using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ViberLounge.Infrastructure.Repositories.Interfaces;
using ViberLounge.Application.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);
string ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
string connectionsStrings = ENVIRONMENT == "Development" ? "ConnectionStrings:DefaultConnection" : "ConnectionStrings:ProductionConnection";

// Add services to the container.
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
configDataBase(builder);
configDependencyInjection(builder);

// Autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.")))
        };
    }
);

void configDataBase(WebApplicationBuilder serviceProvider)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
        builder.Configuration[connectionsStrings],
        options => options.SetPostgresVersion(new Version(15, 0, 0))
    ));
}


void configDependencyInjection(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ValidateModelAttribute>();
    configDependencyService(builder.Services);
    configDependencyRepository(builder.Services);
}

void configDependencyService(IServiceCollection services){
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IProdutoService, ProdutoService>();
}

void configDependencyRepository(IServiceCollection services)
{
    builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
    builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
}

var app = builder.Build();

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