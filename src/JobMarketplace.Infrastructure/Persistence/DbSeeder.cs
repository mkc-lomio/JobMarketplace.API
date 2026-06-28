using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            await SeedJobsAsync(context);
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

        private static async Task SeedJobsAsync(ApplicationDbContext context)
        {
            var data = await ReadSeedDataAsync<SeedJob>("seed-jobs.json");
            if (data is null || data.Count == 0) return;

            // Cache resolved/created companies within this seed run to avoid
            // redundant DB queries when multiple jobs share the same company
            var companyCache = new Dictionary<string, Company>();

            var seededCount = 0;
            foreach (var job in data)
            {
                var exists = await context.Jobs.AnyAsync(j => j.Title == job.Title && j.Company.Name == job.Company.Name);
                if (exists) continue;

                // Try cache first, then DB, then create new — no SaveChangesAsync needed mid-loop
                if (!companyCache.TryGetValue(job.Company.Name, out var company))
                {
                    company = await context.Companies.FirstOrDefaultAsync(c => c.Name == job.Company.Name)
                              ?? new Company
                              {
                                  Name = job.Company.Name,
                                  Description = job.Company.Description,
                                  Website = job.Company.Website,
                                  Industry = job.Company.Industry,
                                  Location = job.Company.Location,
                                  EmployeeCount = job.Company.EmployeeCount,
                                  FoundedYear = job.Company.FoundedYear,
                                  ContactEmail = job.Company.ContactEmail.ToLowerInvariant(),
                                  ContactPhone = job.Company.ContactPhone
                              };

                    companyCache[job.Company.Name] = company;
                }

                await context.Jobs.AddAsync(new Job
                {
                    Title = job.Title,
                    Description = job.Description,
                    Requirements = job.Requirements,
                    Location = job.Location,
                    IsRemote = job.IsRemote,
                    SalaryMin = job.SalaryMin,
                    SalaryMax = job.SalaryMax,
                    SalaryCurrency = job.SalaryCurrency,
                    JobType = job.JobType,
                    Status = job.Status,
                    ExpiresAt = job.ExpiresAt,
                    Tags = job.Tags,
                    Company = company  // EF Core resolves CompanyId and saves both in one shot
                });
                seededCount++;
            }

            if (seededCount > 0)
            {
                await context.SaveChangesAsync(); // Inserts companies first, then jobs — all in one transaction
                Console.WriteLine($"Seeded {seededCount} job(s).");
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

        private record SeedJob
        {
            public string Title { get; init; } = default!;
            public string Description { get; init; } = default!;
            public string? Requirements { get; init; }
            public string Location { get; init; } = default!;
            public bool IsRemote { get; init; }
            public decimal? SalaryMin { get; init; }
            public decimal? SalaryMax { get; init; }
            public string? SalaryCurrency { get; init; }
            public JobType JobType { get; init; } = default!;
            public JobStatus Status { get; init; } = JobStatus.Draft;
            public DateTime? ExpiresAt { get; init; }
            public string? Tags { get; init; }
            // CompanyId is intentionally omitted — company is resolved or created by name at seed time
            public SeedCompany Company { get; init; } = default!;
        }

        private record SeedSkill
        {
            public string Name { get; init; } = default!;
            public string Description { get; init; } = default!;
        }
    }
}