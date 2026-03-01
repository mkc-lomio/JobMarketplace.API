CREATE OR ALTER PROCEDURE [dbo].[sp_SearchJobs]
    @SearchTerm NVARCHAR(200) = NULL,
    @Location NVARCHAR(200) = NULL,
    @JobType NVARCHAR(50) = NULL,
    @ExperienceLevel NVARCHAR(50) = NULL,
    @PageSize INT = 20,
    @Cursor BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Treat empty string as NULL
    IF @SearchTerm = '' SET @SearchTerm = NULL;

    -- Format for CONTAINS: split words, quote each, join with AND
    -- e.g. 'Job Title 160' → '"Job*" AND "Title*" AND "160*"'
    -- This allows plain text input to work with full-text search
    IF @SearchTerm IS NOT NULL
    BEGIN
        DECLARE @FormattedTerm NVARCHAR(500) = '';
        DECLARE @Word NVARCHAR(200);
        DECLARE @Pos INT;

        SET @SearchTerm = LTRIM(RTRIM(@SearchTerm));

        WHILE LEN(@SearchTerm) > 0
        BEGIN
            SET @Pos = CHARINDEX(' ', @SearchTerm);
            IF @Pos = 0
            BEGIN
                SET @Word = @SearchTerm;
                SET @SearchTerm = '';
            END
            ELSE
            BEGIN
                SET @Word = LEFT(@SearchTerm, @Pos - 1);
                SET @SearchTerm = LTRIM(SUBSTRING(@SearchTerm, @Pos + 1, LEN(@SearchTerm)));
            END

            IF LEN(@Word) > 0
            BEGIN
                IF LEN(@FormattedTerm) > 0
                    SET @FormattedTerm = @FormattedTerm + ' AND ';
                SET @FormattedTerm = @FormattedTerm + '"' + @Word + '*"';
            END
        END

        SET @SearchTerm = @FormattedTerm;
    END

    IF @SearchTerm IS NOT NULL
    BEGIN
        -- With full-text search
        SELECT TOP (@PageSize + 1)
            j.Id,
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
            c.PublicGuid AS CompanyPublicGuid,
            c.Name AS CompanyName,
            c.Industry AS CompanyIndustry,
            COUNT(a.Id) AS ApplicationCount
        FROM dbo.Jobs j
        INNER JOIN dbo.Companies c ON j.CompanyId = c.Id
        LEFT JOIN dbo.JobApplications a ON a.JobId = j.Id
        WHERE j.Status = 'Active'
          AND j.Id > @Cursor
          AND CONTAINS((j.Title, j.Description), @SearchTerm)
          AND (@Location IS NULL OR j.Location = @Location)
          AND (@JobType IS NULL OR j.JobType = @JobType)
          AND (@ExperienceLevel IS NULL OR j.ExperienceLevel = @ExperienceLevel)
        GROUP BY
            j.Id, j.PublicGuid, j.Title, j.Location, j.IsRemote,
            j.SalaryMin, j.SalaryMax, j.SalaryCurrency, j.JobType,
            j.ExperienceLevel, j.CreatedAt,
            c.PublicGuid, c.Name, c.Industry
        ORDER BY j.Id;
    END
    ELSE
    BEGIN
        -- Without full-text search (filters only)
        SELECT TOP (@PageSize + 1) -- Fetch one extra row to check if more pages exist (trimmed in C# by PagedResult)
        -- e.g. PageSize = 20 → fetches 21. Got 21? HasMore = true. Got 20 or less? That's everything.
            j.Id,
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
            c.PublicGuid AS CompanyPublicGuid,
            c.Name AS CompanyName,
            c.Industry AS CompanyIndustry,
            COUNT(a.Id) AS ApplicationCount
        FROM dbo.Jobs j
        INNER JOIN dbo.Companies c ON j.CompanyId = c.Id
        LEFT JOIN dbo.JobApplications a ON a.JobId = j.Id
        WHERE j.Status = 'Active'
          AND j.Id > @Cursor
          AND (@Location IS NULL OR j.Location = @Location)
          AND (@JobType IS NULL OR j.JobType = @JobType)
          AND (@ExperienceLevel IS NULL OR j.ExperienceLevel = @ExperienceLevel)
        GROUP BY
            j.Id, j.PublicGuid, j.Title, j.Location, j.IsRemote,
            j.SalaryMin, j.SalaryMax, j.SalaryCurrency, j.JobType,
            j.ExperienceLevel, j.CreatedAt,
            c.PublicGuid, c.Name, c.Industry
        ORDER BY j.Id;
    END
END
GO