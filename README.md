# JobMarketplace API

A .NET 10 Web API built with Clean Architecture, CQRS, MediatR, Repository Pattern, EF Core (writes), and Dapper + Stored Procedures (reads). Secured with JWT authentication, refresh token rotation, and role-based authorization.

---

## Table of Contents

- [Architecture](#architecture)
  - [Clean Architecture — Layer Structure](#clean-architecture--layer-structure)
  - [Request Flow](#request-flow)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [1. Clone the Repository](#1-clone-the-repository)
  - [2. Make Sure SQL Server is Running](#2-make-sure-sql-server-is-running)
  - [3. Check the Connection String](#3-check-the-connection-string)
  - [4. Build the Solution](#4-build-the-solution)
  - [5. Run the API](#5-run-the-api)
  - [6. Open the API Documentation](#6-open-the-api-documentation)
- [Authentication](#authentication)
  - [Default Admin Account](#default-admin-account)
  - [Auth Flow](#auth-flow)
  - [Roles](#roles)
- [Testing the API](#testing-the-api)
  - [Step 1 — Log In](#step-1--log-in)
  - [Step 2 — Create a Company](#step-2--create-a-company)
  - [Step 3 — Create a Job](#step-3--create-a-job)
  - [Step 4 — Get All Jobs](#step-4--get-all-jobs)
  - [Step 5 — Register a Job Seeker and Apply](#step-5--register-a-job-seeker-and-apply)
  - [Step 6 — Refresh an Expired Token](#step-6--refresh-an-expired-token)
  - [Step 7 — Logout (Revoke Token)](#step-7--logout-revoke-token)
- [Using Scalar UI (Browser)](#using-scalar-ui-browser)
- [Project Structure](#project-structure)
- [Useful Commands](#useful-commands)
- [Tech Stack](#tech-stack)

---

## Architecture

Clean Architecture enforces a strict dependency rule: code dependencies can only point inward. The outer layers (API, Infrastructure) depend on the inner layers (Application, Domain) — never the reverse. This means your core business logic has zero knowledge of ASP.NET, Entity Framework, SQL Server, or any other framework or infrastructure concern.

The practical benefits for this project:

- **Testability** — `Application` and `Domain` have no framework dependencies, so handlers and validators can be unit tested without spinning up a database or HTTP server.
- **Replaceability** — swapping SQL Server for PostgreSQL, or EF Core for a different ORM, only touches `Infrastructure`. Nothing in `Application` or `Domain` changes.
- **Separation of concerns** — FluentValidation lives in `Application`, BCrypt lives in `Infrastructure`, controllers live in `API`. Each layer has one job and owns it completely.
- **Explicit contracts** — `Domain` defines the repository interfaces (`IJobRepository`, `IUserRepository`, etc.). `Infrastructure` implements them. `Application` only ever sees the interface — it never imports EF Core directly.

### Clean Architecture — Layer Structure

![Clean Architecture Layer Structure](./assets/jobmarketplace_clean_architecture_structure.svg)

The solution is organized into four concentric layers. Dependencies always point inward — outer layers know about inner ones, never the reverse.

| Layer | Project | Responsibility |
|-------|---------|----------------|
| **Presentation** | `JobMarketplace.API` | Controllers, middleware, JWT config, Scalar docs |
| **Application** | `JobMarketplace.Application` | CQRS commands/queries, FluentValidation, AutoMapper, DTOs |
| **Domain** | `JobMarketplace.Domain` | Entities, enums, repository interfaces — zero dependencies |
| **Infrastructure** | `JobMarketplace.Infrastructure` | EF Core, Dapper, repositories, TokenService, PasswordHasher |

### Request Flow

![Request Flow Diagram](./assets/jobmarketplace_request_flow.svg)

Every HTTP request passes through this pipeline:

```
HTTP Client
  → Middleware (CorrelationId → GlobalExceptionHandler → RequestLogging)
  → JWT Auth + Role check
  → Controller action
  → MediatR dispatcher (_mediator.Send)
  → Pipeline behaviors (LoggingBehavior → ValidationBehavior)
  → Command / Query handler
  → Repository (EF Core writes) or DapperQueryService (reads / streaming)
  → SQL Server — JobMarketplaceDB
```

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
2. Create all tables (Companies, Jobs, JobApplications, Users, RefreshTokens) with proper indexes and constraints
3. Deploy all stored procedures (sp_GetAllCompanies, sp_GetAllJobs, etc.)
4. Seed the default admin user and sample companies from embedded JSON data

You should see in the console:

```
Deployed 5 stored procedure(s) successfully.
Seeded 1 user(s).
Seeded 2 company(ies).
Database 'JobMarketplaceDB' migrated successfully!
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

## Authentication

The API uses **JWT access tokens** (5-minute expiry) and **refresh tokens** (7-day expiry) with rotation. Most endpoints require authentication — you need to log in first and include the access token in every request.

### Default Admin Account

The app seeds a default admin user on first run:

| Field | Value |
|-------|-------|
| Email | `admin@jobmarketplace.com` |
| Password | `Admin@123!` |
| Role | `Admin` |

> **Change these credentials in production.** The seed data lives in `Infrastructure/SeedData/seed-users.json`.

### Auth Flow

```
1. Login (POST /api/auth/login)      → get accessToken + refreshToken
2. Use accessToken in requests       → Authorization: Bearer <token>
3. Token expires (5 min)             → call POST /api/auth/refresh
4. Refresh returns new token pair    → repeat from step 2
5. Done for the day                  → call POST /api/auth/revoke (logout)
```

### Roles

| Role | Can Do |
|------|--------|
| **Admin** | Everything |
| **Employer** | Create/manage companies and jobs, view applications |
| **JobSeeker** | Submit applications |

---

## Testing the API

### Step 1 — Log In

All protected endpoints require a Bearer token. Start by logging in:

```bash
curl -X POST https://localhost:7219/api/auth/login \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "email": "admin@jobmarketplace.com",
    "password": "Admin@123!"
  }'
```

Response:

```json
{
  "isSuccess": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "a1b2c3d4...",
    "accessTokenExpiresAt": "2026-03-01T08:05:00Z",
    "userPublicGuid": "3fa85f64-...",
    "email": "admin@jobmarketplace.com",
    "role": "Admin"
  }
}
```

**Copy the `accessToken`** — you'll include it in every subsequent request as:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Step 2 — Create a Company

```bash
curl -X POST https://localhost:7219/api/companies \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <paste-access-token-here>" \
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

### Step 3 — Create a Job

```bash
curl -X POST https://localhost:7219/api/jobs \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <paste-access-token-here>" \
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

### Step 4 — Get All Jobs

```bash
curl https://localhost:7219/api/jobs \
  -H "Authorization: Bearer <paste-access-token-here>" \
  -k
```

### Step 5 — Register a Job Seeker and Apply

Register a new user with the `JobSeeker` role:

```bash
curl -X POST https://localhost:7219/api/auth/register \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "email": "juan@email.com",
    "password": "Juan@123!",
    "firstName": "Juan",
    "lastName": "Dela Cruz",
    "role": 0
  }'
```

Use the **new access token** from the register response to submit an application:

```bash
curl -X POST https://localhost:7219/api/applications \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <paste-jobseeker-access-token-here>" \
  -k \
  -d '{
    "jobPublicGuid": "<paste-job-guid-here>",
    "applicantName": "Juan Dela Cruz",
    "applicantEmail": "juan@email.com",
    "coverLetter": "I would love to join your team!"
  }'
```

> **Note:** A JobSeeker token cannot create companies or jobs (403 Forbidden). An Employer token cannot submit applications. Only Admin can do everything.

### Step 6 — Refresh an Expired Token

When your access token expires (after 5 minutes), use the refresh token to get a new pair:

```bash
curl -X POST https://localhost:7219/api/auth/refresh \
  -H "Content-Type: application/json" \
  -k \
  -d '{
    "refreshToken": "<paste-refresh-token-here>"
  }'
```

This returns a new `accessToken` and a new `refreshToken`. The old refresh token is revoked (single-use).

### Step 7 — Logout (Revoke Token)

```bash
curl -X POST https://localhost:7219/api/auth/revoke \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <paste-access-token-here>" \
  -k \
  -d '{
    "refreshToken": "<paste-refresh-token-here>"
  }'
```

> **Note:** The `-k` flag tells curl to accept the self-signed development HTTPS certificate.

---

## Using Scalar UI (Browser)

The Scalar UI at `/scalar/v1` lets you send requests directly from the browser. For protected endpoints:

1. Call `POST /api/auth/login` first and copy the `accessToken` from the response
2. Click the **Auth** or **Authorize** button in Scalar
3. Enter: `Bearer <your-access-token>` (include the word "Bearer")
4. Now all requests will include the token automatically

---

## Project Structure

```
assets/
├── jobmarketplace_clean_architecture_structure.svg  ← Layer structure diagram
└── jobmarketplace_request_flow.svg                  ← Request flow diagram

src/
├── JobMarketplace.Domain              ← Entities, Enums, Interfaces (zero dependencies)
├── JobMarketplace.Application         ← CQRS Commands/Queries, Validation, Mapping, Auth
├── JobMarketplace.Infrastructure      ← EF Core, Dapper, Repositories, JWT, BCrypt, Seed Data
└── JobMarketplace.API                 ← Controllers, Middleware, JWT Config, Program.cs
```

---

## Useful Commands

| Command | Description |
|---------|-------------|
| `dotnet build` | Build all projects |
| `dotnet run --project src/JobMarketplace.API` | Run the API from root directory |
| `dotnet clean` | Clean build artifacts |
| `dotnet test` | Run tests (when added) |
| `dotnet ef migrations add <Name> -p src/JobMarketplace.Infrastructure -s src/JobMarketplace.API` | Add a new EF Core migration |

---

## Tech Stack

| Technology | Purpose |
|---|---|
| .NET 10 | Framework |
| Clean Architecture | Project structure |
| CQRS + MediatR | Command/Query separation |
| Repository Pattern | Write abstraction (EF Core) |
| EF Core 10 | ORM for writes + migrations |
| Dapper | Micro-ORM for reads (stored procedures) |
| SQL Server | Database |
| FluentValidation | Input validation |
| AutoMapper | Object mapping |
| Scalar | API documentation |
| BCrypt.Net | Password hashing (adaptive, salted) |
| JWT Bearer | Stateless API authentication |
| Refresh Token Rotation | Long-lived sessions with theft detection |