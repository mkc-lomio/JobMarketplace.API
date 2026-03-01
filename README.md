# JobMarketplace API

A .NET 10 Web API built with Clean Architecture, CQRS, MediatR, Repository Pattern, EF Core (writes), and Dapper + Stored Procedures (reads).

---

## Prerequisites

Before you begin, make sure you have the following installed on your machine:

| Tool | Version | Download |
|------|---------|----------|
| **.NET 10 SDK** | 10.0 or later | [https://dotnet.microsoft.com/download/dotnet/10.0](https://dotnet.microsoft.com/download/dotnet/10.0) |
| **SQL Server** | 2019 or later (Developer Edition is free) | [https://www.microsoft.com/en-us/sql-server/sql-server-downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| **SQL Server Management Studio (SSMS)** | Latest (optional, for viewing data) | [https://learn.microsoft.com/en-us/ssms/download](https://learn.microsoft.com/en-us/ssms/download) |
| **Visual Studio 2022** | 17.12+ with ASP.NET workload | [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/) |
| **Git** | Any recent version | [https://git-scm.com/downloads](https://git-scm.com/downloads) |

> **Alternatives:** You can use **Visual Studio Code** with the C# Dev Kit extension or **JetBrains Rider** instead of Visual Studio. You can also use **SQL Server Express** or **LocalDB** instead of Developer Edition.

### Verify .NET SDK

```bash
dotnet --version
```

Should output `10.0.x`. If not, install the .NET 10 SDK from the link above.

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/<your-username>/JobMarketplace.git
cd JobMarketplace
```

### 2. Make Sure SQL Server is Running

Open **SQL Server Configuration Manager** (or Services) and verify that **SQL Server (MSSQLSERVER)** is running. The app connects to `localhost` with Windows Authentication by default.

### 3. Check the Connection String

The connection string is in `src/JobMarketplace.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=JobMarketplaceDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

**If you need to change it:**

| Scenario | Change `Server=` to |
|----------|---------------------|
| Default SQL Server instance | `localhost` |
| Named instance | `localhost\SQLEXPRESS` |
| LocalDB | `(localdb)\MSSQLLocalDB` |
| Custom port | `localhost,1434` |
| SQL Authentication | Add `User Id=sa;Password=YourPassword;` and remove `Trusted_Connection=true;` |

### 4. Build the Solution

```bash
dotnet build
```

All four projects should build with **0 errors**. Warnings are fine.

### 5. Run the API

```bash
cd src/JobMarketplace.API
dotnet run
```

On first run, the app will automatically:
1. Create the `JobMarketplaceDB` database
2. Create all tables (Companies, Jobs, JobApplications) with proper indexes and constraints
3. Deploy all stored procedures (sp_GetAllCompanies, sp_GetAllJobs, etc.)

You should see in the console:

```
Database 'JobMarketplaceDB' created successfully!
Deployed 5 stored procedure(s) successfully.
```

### 6. Open the API Documentation

Once the app is running, open your browser and go to:

```
https://localhost:7219/scalar/v1
```

or

```
http://localhost:5158/scalar/v1
```

This opens the **Scalar** API documentation UI where you can explore and test all endpoints.

---

## Testing the API

### Using Scalar (Browser)

The Scalar UI at `/scalar/v1` lets you send requests directly from the browser. Expand any endpoint, fill in the JSON body, and click **Send**.

### Using curl

**1. Create a company:**

```bash
curl -X POST https://localhost:7219/api/companies \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "name": "Acme Corp",
    "description": "Leading tech company",
    "industry": "Technology",
    "location": "Manila, PH",
    "foundedYear": 2020,
    "contactEmail": "hr@acmecorp.com"
  }'
```

Copy the `publicGuid` from the response — you'll need it for the next step.

**2. Create a job (use the company's publicGuid):**

```bash
curl -X POST https://localhost:7219/api/jobs \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "title": "Senior .NET Developer",
    "description": "Build awesome APIs with Clean Architecture",
    "location": "Remote",
    "isRemote": true,
    "salaryMin": 120000,
    "salaryMax": 180000,
    "salaryCurrency": "USD",
    "jobType": 0,
    "experienceLevel": 3,
    "companyPublicGuid": "<paste-company-guid-here>"
  }'
```

**3. Get all active jobs:**

```bash
curl https://localhost:7219/api/jobs -k
```

**4. Submit an application (use the job's publicGuid):**

```bash
curl -X POST https://localhost:7219/api/applications \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "jobPublicGuid": "<paste-job-guid-here>",
    "applicantName": "Juan Dela Cruz",
    "applicantEmail": "juan@email.com",
    "coverLetter": "I would love to join your team!"
  }'
```

> **Note:** The `-k` flag tells curl to accept the self-signed development HTTPS certificate.

---

## Project Structure

```
src/
├── JobMarketplace.Domain              ← Entities, Enums, Interfaces (zero dependencies)
├── JobMarketplace.Application         ← CQRS Commands/Queries, Validation, Mapping
├── JobMarketplace.Infrastructure      ← EF Core, Dapper, Repositories, Stored Procedures
└── JobMarketplace.API                 ← Controllers, Middleware, Program.cs
```

For a detailed walkthrough of the architecture, see the [Enterprise Architecture Article](./Enterprise-Architecture-NET10-Article.md).

---

## Useful Commands

| Command | Description |
|---------|-------------|
| `dotnet build` | Build all projects |
| `dotnet run --project src/JobMarketplace.API` | Run the API from root directory |
| `dotnet clean` | Clean build artifacts |
| `dotnet test` | Run tests (when added) |

---

## Troubleshooting

**"Cannot connect to SQL Server"**
- Make sure SQL Server is running (check Windows Services → SQL Server)
- Verify the connection string matches your SQL Server instance name
- If using SQL Server Express, change `Server=localhost` to `Server=localhost\SQLEXPRESS`

**"Database already exists but tables are missing"**
- Drop the database in SSMS: `DROP DATABASE JobMarketplaceDB;`
- Re-run the app — it will recreate everything

**"SSL connection error" or "certificate not trusted"**
- This is normal in development. The `-k` flag in curl and `TrustServerCertificate=true` in the connection string handle this
- In the browser, click "Advanced" → "Proceed" when you see the certificate warning

**"Stored procedures not found"**
- Make sure the `.sql` files are marked as `EmbeddedResource` in `JobMarketplace.Infrastructure.csproj`
- Check the console output — it should say "Deployed 5 stored procedure(s) successfully."

---

## Tech Stack

| Technology | Purpose |
|---|---|
| .NET 10 | Framework |
| Clean Architecture | Project structure |
| CQRS + MediatR | Command/Query separation |
| Repository Pattern | Write abstraction (EF Core) |
| EF Core 10 | ORM for writes |
| Dapper | Micro-ORM for reads (stored procedures) |
| SQL Server | Database |
| FluentValidation | Input validation |
| AutoMapper | Object mapping |
| Scalar | API documentation |