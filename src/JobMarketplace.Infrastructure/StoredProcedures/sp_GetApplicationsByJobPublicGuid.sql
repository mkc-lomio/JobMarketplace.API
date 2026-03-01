CREATE OR ALTER PROCEDURE [dbo].[sp_GetApplicationsByJobPublicGuid]
    @JobPublicGuid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.Id,
        a.PublicGuid,
        a.ApplicantName,
        a.ApplicantEmail,
        a.ApplicantPhone,
        a.ResumeUrl,
        a.CoverLetter,
        a.Status,
        a.Notes,
        a.AppliedAt,
        a.JobId,
        a.CreatedAt
    FROM dbo.JobApplications a
    INNER JOIN dbo.Jobs j ON a.JobId = j.Id
    WHERE j.PublicGuid = @JobPublicGuid
    ORDER BY a.AppliedAt DESC;
END
GO