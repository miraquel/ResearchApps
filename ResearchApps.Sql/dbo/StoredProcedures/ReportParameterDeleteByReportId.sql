CREATE PROCEDURE [dbo].[ReportParameterDeleteByReportId]
    @ReportId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[ReportParameter] WHERE ReportId = @ReportId;
END

GO

