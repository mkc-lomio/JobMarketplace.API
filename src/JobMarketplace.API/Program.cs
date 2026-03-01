using JobMarketplace.API.Middleware;
using JobMarketplace.Application;
using JobMarketplace.Application.Common;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Infrastructure;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register all services from each layer (assembly scanning — no manual registration)
builder.Services.AddApplicationServices();                          // MediatR, FluentValidation, AutoMapper
builder.Services.AddInfrastructureServices(builder.Configuration);  // EF Core, Repositories, Dapper

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply pending migrations + deploy stored procedures on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();                               // Creates DB if missing, applies new migrations if exists
    await StoredProcedureMigrator.DeployStoredProceduresAsync(db);  // CREATE OR ALTER — idempotent, safe every startup
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbSeeder.SeedAsync(db, passwordHasher);
    Console.WriteLine("Database 'JobMarketplaceDB' migrated successfully!");
}

// Middleware pipeline — order matters (top-to-bottom on request, bottom-to-top on response)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();  // Catches all exceptions → JSON error responses (400/404/500)

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();
app.UseAuthentication();  // Must come BEFORE UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();