CREATE PROCEDURE [dbo].[ReportParameterSelectByReportId]
    @ReportId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ParameterId,
        ReportId,
        ParameterName,
        DisplayLabel,
        DataType,
        DefaultValue,
        IsRequired,
        DisplayOrder,
        LookupSource,
        Placeholder
    FROM [dbo].[ReportParameter]
    WHERE ReportId = @ReportId
    ORDER BY DisplayOrder;
END

GO

