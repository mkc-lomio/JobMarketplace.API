-- ============================================================
-- Full-Text Index + Performance Indexes for JobMarketplace
-- ============================================================

-- ============================================================
-- PREREQUISITE: SQL Server Full-Text Search must be installed.
-- If you get "Full-Text Search is not installed" error, follow these steps:
--   1. Open "SQL Server Installation Center"
--      - Search "SQL Server 2022 Installation Center" in Windows Start
--      - Or run: C:\SQLServer2022\setup.exe (your install media location)
--   2. Click "New SQL Server stand-alone installation or add features to an existing installation"
--   3. On the "Installation Type" page, select "Add features to an existing instance"
--   4. Select your instance (e.g., MSSQLSERVER)
--   5. On the "Feature Selection" page, check "Full-Text and Semantic Extractions for Search"
--   6. Complete the wizard and restart SQL Server service
-- 
-- Or you may refer here: https://www.youtube.com/watch?v=Cx6HP4ysx1c
--
-- To verify Full-Text Search is installed, run:
--   SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled');
--   -- Returns 1 if installed, 0 if not
-- ============================================================

-- Full-Text Catalog (idempotent)
IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE name = 'JobMarketplaceCatalog')
    CREATE FULLTEXT CATALOG JobMarketplaceCatalog AS DEFAULT;
GO

-- Full-Text Index on Jobs (Title + Description)
IF NOT EXISTS (
    SELECT 1 FROM sys.fulltext_indexes fi
    INNER JOIN sys.tables t ON fi.object_id = t.object_id
    WHERE t.name = 'Jobs'
)
BEGIN
    CREATE FULLTEXT INDEX ON dbo.Jobs (Title, Description)
    KEY INDEX PK_Jobs ON JobMarketplaceCatalog
    WITH CHANGE_TRACKING AUTO;
END
GO

-- Filtered index: Active jobs (90% of queries filter by Active)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Jobs_Active_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Jobs_Active_CompanyId
    ON dbo.Jobs (Id, CompanyId)
    INCLUDE (PublicGuid, Title, Location, IsRemote, SalaryMin, SalaryMax, SalaryCurrency, JobType, ExperienceLevel, CreatedAt)
    WHERE Status = 'Active';
GO

-- Index for application count by JobId
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_JobApplications_JobId')
    CREATE NONCLUSTERED INDEX IX_JobApplications_JobId
    ON dbo.JobApplications (JobId)
    INCLUDE (Id);
GO

-- Index for company lookup in joins
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Companies_Id_Name')
    CREATE NONCLUSTERED INDEX IX_Companies_Id_Name
    ON dbo.Companies (Id)
    INCLUDE (PublicGuid, Name, Industry, Location);
GO