CREATE PROCEDURE [dbo].[ReportSelectById]
    @ReportId INT
AS
BEGIN
    SET NOCOUNT ON;
    
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
    WHERE r.ReportId = @ReportId;
END

GO

