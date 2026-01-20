CREATE PROCEDURE [dbo].[ReportUpdate]
    @ReportId INT,
    @ReportName NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @ReportType INT = 1,
    @SqlQuery NVARCHAR(MAX) = NULL,
    @TemplatePath NVARCHAR(500) = NULL,
    @StatusId INT = 1,
    @PageSize NVARCHAR(20) = 'A4',
    @Orientation INT = 1,
    @ModifiedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Report]
    SET ReportName = @ReportName,
        Description = @Description,
        ReportType = @ReportType,
        SqlQuery = @SqlQuery,
        TemplatePath = @TemplatePath,
        StatusId = @StatusId,
        PageSize = @PageSize,
        Orientation = @Orientation,
        ModifiedDate = GETDATE(),
        ModifiedBy = @ModifiedBy
    WHERE ReportId = @ReportId;
    
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

