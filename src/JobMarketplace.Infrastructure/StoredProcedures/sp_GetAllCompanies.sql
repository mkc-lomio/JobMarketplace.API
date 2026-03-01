CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllCompanies]
    @PageSize INT = 20,
    @Cursor BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@PageSize + 1)
        c.Id,
        c.PublicGuid,
        c.Name,
        c.Industry,
        c.Location,
        c.ContactEmail,
        c.FoundedYear,
        c.CreatedAt
    FROM dbo.Companies c
    WHERE c.Id > @Cursor
    ORDER BY c.Id;
END
GO