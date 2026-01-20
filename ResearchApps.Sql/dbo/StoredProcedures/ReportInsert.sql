CREATE PROCEDURE [dbo].[ReportInsert]
    @ReportName NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @ReportType INT = 1,
    @SqlQuery NVARCHAR(MAX) = NULL,
    @TemplatePath NVARCHAR(500) = NULL,
    @StatusId INT = 1,
    @PageSize NVARCHAR(20) = 'A4',
    @Orientation INT = 1,
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Report] (ReportName, Description, ReportType, SqlQuery, TemplatePath, StatusId, PageSize, Orientation, CreatedBy, ModifiedBy)
    VALUES (@ReportName, @Description, @ReportType, @SqlQuery, @TemplatePath, @StatusId, @PageSize, @Orientation, @CreatedBy, @CreatedBy);
    
    DECLARE @NewId INT = SCOPE_IDENTITY();
    
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
    WHERE r.ReportId = @NewId;
END

GO

