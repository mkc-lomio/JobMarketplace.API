CREATE OR ALTER PROCEDURE [dbo].[sp_GetJobByPublicGuid]
    @PublicGuid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        j.Id,
        j.PublicGuid,
        j.Title,
        j.Description,
        j.Requirements,
        j.Responsibilities,
        j.Location,
        j.IsRemote,
        j.SalaryMin,
        j.SalaryMax,
        j.SalaryCurrency,
        j.JobType,
        j.ExperienceLevel,
        j.Status,
        j.ExpiresAt,
        j.Tags,
        j.CompanyId,
        j.CreatedAt,
        c.PublicGuid AS CompanyPublicGuid,
        c.Name AS CompanyName
    FROM dbo.Jobs j
    INNER JOIN dbo.Companies c ON j.CompanyId = c.Id
    WHERE j.PublicGuid = @PublicGuid;
END
GO