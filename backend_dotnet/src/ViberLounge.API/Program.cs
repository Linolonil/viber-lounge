using System.Text;
using HealthChecks.UI.Client;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Infrastructure.Repositories;
using ViberLounge.Application.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ViberLounge.Infrastructure.Repositories.Interfaces;


var builder = WebApplication.CreateBuilder(args);
string ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
string connectionsStrings = ENVIRONMENT == "Development" ? "ConnectionStrings:DefaultConnection" : "ConnectionStrings:ProductionConnection";

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
configSwagger(builder);
configDataBase(builder);
configJwtAuthentication(builder);
configDependencyInjection(builder);

// Configuração da conexão com o banco de dados PostgreSQL
void configDataBase(WebApplicationBuilder serviceProvider)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
        builder.Configuration[connectionsStrings],
        options => options.SetPostgresVersion(new Version(15, 0, 0))
    ));
}

// Configuração do HealthCheck com JWT
void configJwtAuthentication(WebApplicationBuilder serviceProvider)
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // options.RequireHttpsMetadata = false;
        // options.SaveToken = true;
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
    });
}

// Configuração do Swagger com autenticação JWT
void configSwagger(WebApplicationBuilder serviceProvider)
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ViberLounge API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Adicione o TOKEN gerado pela rota auth/login",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

// Configuração da injeção de dependência
void configDependencyInjection(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ValidateModelAttribute>();
    configDependencyService(builder.Services);
    configDependencyRepository(builder.Services);
}

// Configuração dos serviços
void configDependencyService(IServiceCollection services){
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IProdutoService, ProdutoService>();
}

// Configuração dos repositórios
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