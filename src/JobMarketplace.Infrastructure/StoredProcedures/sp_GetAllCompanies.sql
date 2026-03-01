CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllCompanies]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        PublicGuid,
        Name,
        Description,
        Website,
        LogoUrl,
        Industry,
        Location,
        EmployeeCount,
        FoundedYear,
        ContactEmail,
        ContactPhone,
        CreatedAt,
        CreatedBy,
        LastModifiedAt,
        LastModifiedBy
    FROM dbo.Companies
    ORDER BY Name;
END
GO