CREATE PROCEDURE [dbo].[ReportSelect]
    @ReportName NVARCHAR(200) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        r.ReportId,
        r.ReportName,
        r.Description,
        r.ReportType,
        r.SqlQuery,
        r.TemplatePath,
        r.StatusId,
        s.StatusName,
        r.PageSize,
        r.Orientation,
        r.CreatedDate,
        r.CreatedBy,
        r.ModifiedDate,
        r.ModifiedBy
    FROM [dbo].[Report] r
    LEFT JOIN [dbo].[Status] s ON r.StatusId = s.StatusId
    WHERE (@ReportName IS NULL OR r.ReportName LIKE @ReportName)
    ORDER BY r.ReportId DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Report] r
    WHERE (@ReportName IS NULL OR r.ReportName LIKE @ReportName);
END

GO

