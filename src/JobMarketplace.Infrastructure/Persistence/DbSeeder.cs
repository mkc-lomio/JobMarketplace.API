using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JobMarketplace.Infrastructure.Persistence
{
    /// <summary>
    /// Seeds data from embedded JSON files in SeedData/ folder on startup.
    /// Convention: seed-{entity}.json → seeds that entity. Skips existing records by unique key.
    /// Add a new JSON file + a private method to seed any new entity.
    /// Called from Program.cs after MigrateAsync().
    /// </summary>
    public static class DbSeeder
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            await SeedUsersAsync(context, passwordHasher);
            await SeedCompaniesAsync(context);
            // Add more: await SeedJobsAsync(context);
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            var data = await ReadSeedDataAsync<SeedUser>("seed-users.json");
            if (data is null || data.Count == 0) return;

            var seededCount = 0;
            foreach (var item in data)
            {
                var exists = await context.Users.AnyAsync(u => u.Email == item.Email.ToLowerInvariant());
                if (exists) continue;

                await context.Users.AddAsync(new User
                {
                    Email = item.Email.ToLowerInvariant(),
                    PasswordHash = passwordHasher.Hash(item.Password),
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Role = Enum.Parse<UserRole>(item.Role, ignoreCase: true),
                    IsActive = true
                });
                seededCount++;
            }

            if (seededCount > 0)
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"Seeded {seededCount} user(s).");
            }
        }

        private static async Task SeedCompaniesAsync(ApplicationDbContext context)
        {
            var data = await ReadSeedDataAsync<SeedCompany>("seed-companies.json");
            if (data is null || data.Count == 0) return;

            var seededCount = 0;
            foreach (var item in data)
            {
                var exists = await context.Companies.AnyAsync(c => c.ContactEmail == item.ContactEmail.ToLowerInvariant());
                if (exists) continue;

                await context.Companies.AddAsync(new Company
                {
                    Name = item.Name,
                    Description = item.Description,
                    Website = item.Website,
                    Industry = item.Industry,
                    Location = item.Location,
                    EmployeeCount = item.EmployeeCount,
                    FoundedYear = item.FoundedYear,
                    ContactEmail = item.ContactEmail.ToLowerInvariant(),
                    ContactPhone = item.ContactPhone
                });
                seededCount++;
            }

            if (seededCount > 0)
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"Seeded {seededCount} company(ies).");
            }
        }

        /// <summary>
        /// Reads an embedded JSON file from the SeedData folder.
        /// Returns null if the file doesn't exist — seeding is optional per entity.
        /// </summary>
        private static async Task<List<T>?> ReadSeedDataAsync<T>(string fileName)
        {
            var assembly = typeof(DbSeeder).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName is null) return null;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null) return null;

            return await JsonSerializer.DeserializeAsync<List<T>>(stream, _jsonOptions);
        }

        // Seed DTOs — shape matches the JSON files
        private record SeedUser
        {
            public string Email { get; init; } = default!;
            public string Password { get; init; } = default!;
            public string FirstName { get; init; } = default!;
            public string LastName { get; init; } = default!;
            public string Role { get; init; } = default!;
        }

        private record SeedCompany
        {
            public string Name { get; init; } = default!;
            public string Description { get; init; } = default!;
            public string? Website { get; init; }
            public string Industry { get; init; } = default!;
            public string Location { get; init; } = default!;
            public int? EmployeeCount { get; init; }
            public int FoundedYear { get; init; }
            public string ContactEmail { get; init; } = default!;
            public string? ContactPhone { get; init; }
        }
    }
}