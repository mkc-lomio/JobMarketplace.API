using JobMarketplace.API.Middleware;
using JobMarketplace.Application;
using JobMarketplace.Infrastructure;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register all services from each layer (assembly scanning — no manual registration)
builder.Services.AddApplicationServices();                          // MediatR, FluentValidation, AutoMapper
builder.Services.AddInfrastructureServices(builder.Configuration);  // EF Core, Repositories, Dapper

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply pending migrations + deploy stored procedures on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();                               // Creates DB if missing, applies new migrations if exists
    await StoredProcedureMigrator.DeployStoredProceduresAsync(db);  // CREATE OR ALTER — idempotent, safe every startup
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
app.UseAuthorization();
app.MapControllers();

app.Run();