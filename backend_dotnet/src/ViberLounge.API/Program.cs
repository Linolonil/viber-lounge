using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using ViberLounge.Infrastructure.Context;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
string ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
string connectionsStrings = ENVIRONMENT == "Development" ? "ConnectionStrings:DefaultConnection" : "ConnectionStrings:ProductionConnection";

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

configDataBase(builder);
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
// Configure the HTTP request pipeline.
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