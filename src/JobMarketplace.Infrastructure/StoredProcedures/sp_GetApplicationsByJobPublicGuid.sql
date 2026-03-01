CREATE OR ALTER PROCEDURE [dbo].[sp_GetApplicationsByJobPublicGuid]
    @JobPublicGuid UNIQUEIDENTIFIER,
    @PageSize INT = 20,
    @Cursor BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@PageSize + 1)
        a.Id,
        a.PublicGuid,
        a.ApplicantName,
        a.ApplicantEmail,
        a.Status,
        a.AppliedAt,
        a.CreatedAt
    FROM dbo.JobApplications a
    INNER JOIN dbo.Jobs j ON a.JobId = j.Id
    WHERE j.PublicGuid = @JobPublicGuid
      AND a.Id > @Cursor
    ORDER BY a.Id;
END
GO