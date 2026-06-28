CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllSkills]
    @PageSize INT = 20,
    @Cursor BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@PageSize + 1)
        s.Id,
        s.PublicGuid,
        s.Name,
        s.Description,
        s.CreatedAt,
        s.LastModifiedAt 
    FROM dbo.Skill s
    WHERE s.Id > @Cursor
    ORDER BY s.Id;
END