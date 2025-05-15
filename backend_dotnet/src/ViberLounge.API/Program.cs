using Serilog;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Infrastructure.Logging;
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
configCors(builder);
configSwagger(builder);
configDataBase(builder);
configJwtAuthentication(builder);
configModelError(builder.Services);
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
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "ViberLounge API",
            Version = "v1",
            Description = "API para gerenciamento de produtos e vendas",
            Contact = new OpenApiContact
            {
                Name = "ViberLounge Support",
                Email = "",
                Url = new Uri("https://ViberLounge.com/support")
            }
        });
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
    builder.Services.AddSingleton<ILoggerService, LoggerService>();
    builder.Services.AddScoped<ValidateModelAttribute>();
    configDependencyService(builder.Services);
    configDependencyRepository(builder.Services);
}
// Configuração dos serviços
void configDependencyService(IServiceCollection services){
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ISaleService, SaleService>();
}

// Configuração dos repositórios
void configDependencyRepository(IServiceCollection services)
{
    builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
    builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
    builder.Services.AddScoped<IVendaRepository, VendaRepository>();
}

void configModelError(IServiceCollection services)
{
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .SelectMany(ms => ms.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .Distinct()
                .ToList();

            return new BadRequestObjectResult(new { message = string.Join(" ", errors) });
        };
    });
}

void configCors(WebApplicationBuilder serviceProvider)
{
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowedOriginsPolicy", policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()   
                .AllowAnyHeader()
                .AllowCredentials(); 
        });
    });
}
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ViberLounge API V1");
        c.RoutePrefix = string.Empty; // Define a URL base para o Swagger
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowedOriginsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions{ Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse});

app.Run();