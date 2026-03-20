using JobMarketplace.API.Middleware;
using JobMarketplace.Application;
using JobMarketplace.Application.Common;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Infrastructure;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json;

// ─── Serilog Bootstrap ───────────────────────────────────────
// Configure Serilog BEFORE building the host so startup errors are captured too.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()          // Enables LogContext.PushProperty (used by CorrelationIdMiddleware)
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();

const string AngularCorsPolicy = "AngularLocalPolicy";

try
{
    Log.Information("Starting JobMarketplace API...");

    var builder = WebApplication.CreateBuilder(args);

    // Replace default logging with Serilog
    builder.Host.UseSerilog();

    // Register all services from each layer
    builder.Services.AddApplicationServices();                          // MediatR, FluentValidation, AutoMapper
    builder.Services.AddInfrastructureServices(builder.Configuration);  // EF Core, Repositories, Dapper, Health Checks

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

    // ─── CORS ────────────────────────────────────────────────────
    // Whitelists the Angular dev server. In production, replace with your deployed URL.
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AngularCorsPolicy, policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    var app = builder.Build();

    // Apply pending migrations + deploy stored procedures on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        await StoredProcedureMigrator.DeployStoredProceduresAsync(db);
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await DbSeeder.SeedAsync(db, passwordHasher);
        Log.Information("Database 'JobMarketplaceDB' migrated successfully!");
    }

    // ─── Middleware pipeline — order matters! ────────────────────
    // 1. Correlation ID first — so every middleware below has access to it
    app.UseMiddleware<CorrelationIdMiddleware>();

    // 2. Exception handler — catches everything from middleware below
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // 3. Request logging — logs method, path, status code, duration
    app.UseMiddleware<RequestLoggingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
    }

    app.UseHttpsRedirection();
    app.UseCors(AngularCorsPolicy);   // must be before UseAuthentication + UseAuthorization
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // ─── Health check endpoint ───────────────────────────────────
    // GET /health — returns { "status": "Healthy", "checks": { "sqlserver": "Healthy" } }
    // Used by load balancers, Docker, Kubernetes to check if the API is ready
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds + "ms",
                checks = report.Entries.ToDictionary(
                    e => e.Key,
                    e => new
                    {
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration.TotalMilliseconds + "ms",
                        description = e.Value.Description
                    })
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                new JsonSerializerOptions { WriteIndented = true }));
        }
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}