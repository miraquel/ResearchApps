CREATE PROCEDURE [dbo].[ReportCbo]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ReportId,
        ReportName,
        ReportType,
        StatusId
    FROM [dbo].[Report]
    WHERE StatusId = 1
    ORDER BY ReportName;
END

GO

