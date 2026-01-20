CREATE PROCEDURE [dbo].[ReportDelete]
    @ReportId INT,
    @ModifiedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Parameters are deleted by cascade
    DELETE FROM [dbo].[Report] WHERE ReportId = @ReportId;
    
    SELECT @@ROWCOUNT AS AffectedRows;
END

GO

