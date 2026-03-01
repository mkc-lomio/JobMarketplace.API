CREATE OR ALTER PROCEDURE [dbo].[sp_ExportJobs]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        j.PublicGuid,
        j.Title,
        j.Location,
        j.IsRemote,
        j.SalaryMin,
        j.SalaryMax,
        j.SalaryCurrency,
        j.JobType,
        j.ExperienceLevel,
        j.CreatedAt,
        c.Name AS CompanyName,
        c.Industry AS CompanyIndustry
    FROM dbo.Jobs j
    INNER JOIN dbo.Companies c ON j.CompanyId = c.Id
    WHERE j.Status = 'Active'
    ORDER BY j.Id;
END
GO