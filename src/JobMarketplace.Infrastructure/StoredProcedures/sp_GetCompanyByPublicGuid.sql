CREATE OR ALTER PROCEDURE [dbo].[sp_GetCompanyByPublicGuid]
    @PublicGuid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.Id,
        c.PublicGuid,
        c.Name,
        c.Description,
        c.Website,
        c.LogoUrl,
        c.Industry,
        c.Location,
        c.EmployeeCount,
        c.FoundedYear,
        c.ContactEmail,
        c.ContactPhone,
        c.CreatedAt,
        c.CreatedBy,
        c.LastModifiedAt,
        c.LastModifiedBy
    FROM dbo.Companies c
    WHERE c.PublicGuid = @PublicGuid;
END
GO