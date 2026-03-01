using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;

// ─── Load Configuration ──────────────────────────────────────
var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

var companyCount = configuration.GetValue("BulkSeeder:Companies", 1_000);
var userCount = configuration.GetValue("BulkSeeder:Users", 500_000);
var jobCount = configuration.GetValue("BulkSeeder:Jobs", 100_000);
var appCount = configuration.GetValue("BulkSeeder:Applications", 10_000_000);
var batchSize = configuration.GetValue("BulkSeeder:BatchSize", 100_000);
var totalCount = companyCount + userCount + jobCount + appCount;

// ─── Safety check ────────────────────────────────────────────
Console.WriteLine("JobMarketplace Bulk Data Seeder");
Console.WriteLine($"  Environment: {environment}");
Console.WriteLine($"  Database:    {ExtractDbName(connectionString)}");
Console.WriteLine($"  Records:     {totalCount:N0}");
Console.WriteLine($"    Companies:    {companyCount:N0}");
Console.WriteLine($"    Users:        {userCount:N0}");
Console.WriteLine($"    Jobs:         {jobCount:N0}");
Console.WriteLine($"    Applications: {appCount:N0}");
Console.WriteLine($"  Batch size:  {batchSize:N0}");
Console.WriteLine();

if (!ConfirmExistingData())
    return;

Console.Write("Continue? (y/n): ");
if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }

// ─── Seed ────────────────────────────────────────────────────
var totalSw = Stopwatch.StartNew();
Console.WriteLine();
Console.WriteLine("=== Seeding Started ===");

var companyIds = await InsertCompanies();
var userIds = await InsertUsers();
var jobIds = await InsertJobs(companyIds);
await InsertApplications(jobIds);

totalSw.Stop();
Console.WriteLine();
Console.WriteLine($"=== Done — {totalSw.Elapsed.TotalSeconds:F1}s total ===");

// ─── Companies ───────────────────────────────────────────────
async Task<long[]> InsertCompanies()
{
    var sw = Stopwatch.StartNew();
    var dt = new DataTable();
    dt.Columns.Add("PublicGuid", typeof(Guid));
    dt.Columns.Add("Name", typeof(string));
    dt.Columns.Add("Description", typeof(string));
    dt.Columns.Add("Industry", typeof(string));
    dt.Columns.Add("Location", typeof(string));
    dt.Columns.Add("ContactEmail", typeof(string));
    dt.Columns.Add("FoundedYear", typeof(int));
    dt.Columns.Add("CreatedAt", typeof(DateTime));

    var rng = new Random(42);
    for (int i = 0; i < companyCount; i++)
    {
        dt.Rows.Add(Guid.NewGuid(), $"Company-{i + 1}", $"Description for company {i + 1}",
            Pick(rng, "Technology", "Finance", "Healthcare", "Education", "Retail"),
            Pick(rng, "Manila, PH", "Makati, PH", "Cebu, PH", "Remote", "BGC, PH"),
            $"contact{i + 1}@company.com", rng.Next(1990, 2025), DateTime.UtcNow);
    }

    await BulkCopy("Companies", dt);
    Console.WriteLine($"  Companies: {companyCount:N0} — {sw.Elapsed.TotalSeconds:F1}s");
    return await GetIds("Companies");
}

// ─── Users ───────────────────────────────────────────────────
async Task<long[]> InsertUsers()
{
    var sw = Stopwatch.StartNew();
    var hash = BCrypt.Net.BCrypt.HashPassword("Bulk@123!", 11);

    var dt = NewUserTable();
    var rng = new Random(42);
    var inserted = 0;

    for (int i = 0; i < userCount; i++)
    {
        var role = i < 10 ? "Admin" : (i % 5 == 0 ? "Employer" : "JobSeeker");
        dt.Rows.Add(Guid.NewGuid(), $"user{i + 1}@test.com", hash,
            $"First{i + 1}", $"Last{i + 1}", role, true, 0, DateTime.UtcNow);

        if (dt.Rows.Count >= batchSize)
        {
            await BulkCopy("Users", dt);
            inserted += dt.Rows.Count;
            Console.WriteLine($"  Users: {inserted:N0}/{userCount:N0}");
            dt.Clear();
        }
    }
    if (dt.Rows.Count > 0) { await BulkCopy("Users", dt); inserted += dt.Rows.Count; }

    Console.WriteLine($"  Users: {inserted:N0} — {sw.Elapsed.TotalSeconds:F1}s");
    return await GetIds("Users");
}

// ─── Jobs ────────────────────────────────────────────────────
async Task<long[]> InsertJobs(long[] companyIds)
{
    var sw = Stopwatch.StartNew();
    var dt = NewJobTable();
    var rng = new Random(42);
    var inserted = 0;

    for (int i = 0; i < jobCount; i++)
    {
        var salaryMin = rng.Next(30, 120) * 1000m;
        dt.Rows.Add(Guid.NewGuid(), $"Job Title {i + 1}", $"Job description {i + 1}",
            Pick(rng, "Manila, PH", "Makati, PH", "Cebu, PH", "Remote"),
            rng.Next(2) == 1, salaryMin, salaryMin + rng.Next(10, 60) * 1000m, "PHP",
            Pick(rng, "FullTime", "PartTime", "Contract", "Freelance"),
            Pick(rng, "Entry", "Junior", "Mid", "Senior", "Lead"),
            i % 10 == 0 ? "Closed" : "Active",
            companyIds[rng.Next(companyIds.Length)], DateTime.UtcNow);

        if (dt.Rows.Count >= batchSize)
        {
            await BulkCopy("Jobs", dt);
            inserted += dt.Rows.Count;
            Console.WriteLine($"  Jobs: {inserted:N0}/{jobCount:N0}");
            dt.Clear();
        }
    }
    if (dt.Rows.Count > 0) { await BulkCopy("Jobs", dt); inserted += dt.Rows.Count; }

    Console.WriteLine($"  Jobs: {inserted:N0} — {sw.Elapsed.TotalSeconds:F1}s");
    return await GetIds("Jobs");
}

// ─── Applications ────────────────────────────────────────────
async Task InsertApplications(long[] jobIds)
{
    var sw = Stopwatch.StartNew();
    var dt = NewApplicationTable();
    var rng = new Random(42);
    var inserted = 0;

    for (int i = 0; i < appCount; i++)
    {
        dt.Rows.Add(Guid.NewGuid(), $"Applicant {i + 1}", $"applicant{i + 1}@test.com",
            Pick(rng, "Submitted", "UnderReview", "Shortlisted", "Rejected", "Accepted"),
            jobIds[rng.Next(jobIds.Length)],
            DateTime.UtcNow.AddDays(-rng.Next(0, 365)), DateTime.UtcNow);

        if (dt.Rows.Count >= batchSize)
        {
            await BulkCopy("JobApplications", dt);
            inserted += dt.Rows.Count;
            if (inserted % 1_000_000 == 0)
                Console.WriteLine($"  Applications: {inserted:N0}/{appCount:N0} — {sw.Elapsed.TotalSeconds:F0}s");
            dt.Clear();
        }
    }
    if (dt.Rows.Count > 0) { await BulkCopy("JobApplications", dt); inserted += dt.Rows.Count; }

    Console.WriteLine($"  Applications: {inserted:N0} — {sw.Elapsed.TotalSeconds:F1}s");
}

// ─── Shared helpers ──────────────────────────────────────────
async Task BulkCopy(string table, DataTable dt)
{
    using var conn = new SqlConnection(connectionString);
    await conn.OpenAsync();
    using var bulk = new SqlBulkCopy(conn) { DestinationTableName = $"dbo.{table}", BatchSize = batchSize, BulkCopyTimeout = 600 };
    foreach (DataColumn col in dt.Columns) bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
    await bulk.WriteToServerAsync(dt);
}

async Task<long[]> GetIds(string table)
{
    using var conn = new SqlConnection(connectionString);
    await conn.OpenAsync();
    using var cmd = new SqlCommand($"SELECT Id FROM dbo.{table}", conn);
    using var reader = await cmd.ExecuteReaderAsync();
    var ids = new List<long>();
    while (await reader.ReadAsync()) ids.Add(reader.GetInt64(0));
    return ids.ToArray();
}

string Pick(Random rng, params string[] opts) => opts[rng.Next(opts.Length)];

string ExtractDbName(string cs)
    => cs.Split(';').FirstOrDefault(s => s.Trim().StartsWith("Database", StringComparison.OrdinalIgnoreCase))?.Split('=')[1]?.Trim() ?? "unknown";

bool ConfirmExistingData()
{
    try
    {
        using var conn = new SqlConnection(connectionString);
        conn.Open();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Companies", conn);
        var count = (int)cmd.ExecuteScalar();
        if (count > 100)
        {
            Console.WriteLine($"Companies table already has {count:N0} rows — looks like bulk data exists.");
            Console.Write("Seed anyway? (y/n): ");
            return Console.ReadLine()?.Trim().ToLower() == "y";
        }
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Cannot connect to database: {ex.Message}");
        Console.WriteLine("Make sure the API has been run at least once (to create tables).");
        return false;
    }
}

// ─── DataTable factories ─────────────────────────────────────
DataTable NewUserTable()
{
    var dt = new DataTable();
    dt.Columns.Add("PublicGuid", typeof(Guid));
    dt.Columns.Add("Email", typeof(string));
    dt.Columns.Add("PasswordHash", typeof(string));
    dt.Columns.Add("FirstName", typeof(string));
    dt.Columns.Add("LastName", typeof(string));
    dt.Columns.Add("Role", typeof(string));
    dt.Columns.Add("IsActive", typeof(bool));
    dt.Columns.Add("FailedLoginAttempts", typeof(int));
    dt.Columns.Add("CreatedAt", typeof(DateTime));
    return dt;
}

DataTable NewJobTable()
{
    var dt = new DataTable();
    dt.Columns.Add("PublicGuid", typeof(Guid));
    dt.Columns.Add("Title", typeof(string));
    dt.Columns.Add("Description", typeof(string));
    dt.Columns.Add("Location", typeof(string));
    dt.Columns.Add("IsRemote", typeof(bool));
    dt.Columns.Add("SalaryMin", typeof(decimal));
    dt.Columns.Add("SalaryMax", typeof(decimal));
    dt.Columns.Add("SalaryCurrency", typeof(string));
    dt.Columns.Add("JobType", typeof(string));
    dt.Columns.Add("ExperienceLevel", typeof(string));
    dt.Columns.Add("Status", typeof(string));
    dt.Columns.Add("CompanyId", typeof(long));
    dt.Columns.Add("CreatedAt", typeof(DateTime));
    return dt;
}

DataTable NewApplicationTable()
{
    var dt = new DataTable();
    dt.Columns.Add("PublicGuid", typeof(Guid));
    dt.Columns.Add("ApplicantName", typeof(string));
    dt.Columns.Add("ApplicantEmail", typeof(string));
    dt.Columns.Add("Status", typeof(string));
    dt.Columns.Add("JobId", typeof(long));
    dt.Columns.Add("AppliedAt", typeof(DateTime));
    dt.Columns.Add("CreatedAt", typeof(DateTime));
    return dt;
}