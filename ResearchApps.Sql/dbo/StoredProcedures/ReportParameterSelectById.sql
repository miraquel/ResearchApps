CREATE PROCEDURE [dbo].[ReportParameterSelectById]
    @ParameterId INT
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
    WHERE ParameterId = @ParameterId;
END

GO

