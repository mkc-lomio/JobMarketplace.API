using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence
{
    /// <summary>
    /// Reads .sql files embedded in this assembly and executes them on startup.
    /// All SPs use CREATE OR ALTER — safe to run every time (idempotent).
    /// </summary>
    public static class StoredProcedureMigrator
    {
        public static async Task DeployStoredProceduresAsync(ApplicationDbContext context)
        {
            var assembly = typeof(StoredProcedureMigrator).Assembly;

            // Find all embedded .sql resources (configured in .csproj as EmbeddedResource)
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(r => r.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r);

            foreach (var resourceName in resourceNames)
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream is null) continue;

                using var reader = new StreamReader(stream);
                var sql = await reader.ReadToEndAsync();

                // Split on GO — SSMS understands it, but ExecuteSqlRawAsync does not
                var batches = sql.Split(
                    new[] { "\nGO", "\r\nGO" },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var batch in batches)
                {
                    var trimmed = batch.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        await context.Database.ExecuteSqlRawAsync(trimmed);
                    }
                }
            }

            Console.WriteLine($"Deployed {resourceNames.Count()} stored procedure(s) successfully.");
        }
    }
}