CREATE PROCEDURE [dbo].[ReportParameterDelete]
    @ParameterId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[ReportParameter] WHERE ParameterId = @ParameterId;
END

GO

