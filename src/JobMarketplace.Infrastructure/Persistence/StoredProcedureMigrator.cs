using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence
{
    public static class StoredProcedureMigrator
    {
        public static async Task DeployStoredProceduresAsync(ApplicationDbContext context)
        {
            var assembly = typeof(StoredProcedureMigrator).Assembly;
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(r => r.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r);

            foreach (var resourceName in resourceNames)
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream is null) continue;

                using var reader = new StreamReader(stream);
                var sql = await reader.ReadToEndAsync();

                // Remove GO statements (not valid in ExecuteSqlRawAsync)
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
