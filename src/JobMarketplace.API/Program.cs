using JobMarketplace.Application;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Infrastructure;
using JobMarketplace.Infrastructure.Persistence;
using JobMarketplace.Infrastructure.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Register layers ──────────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── Register Infrastructure (EF Core + SQL Server) ───
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddScoped<IDapperQueryService, DapperQueryService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Auto-create database ─────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();
    await StoredProcedureMigrator.DeployStoredProceduresAsync(db); // running stored procedure migrations
    Console.WriteLine("Database 'JobMarketplaceDB' created successfully!");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    // Redirect root "/" to Scalar docs
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
