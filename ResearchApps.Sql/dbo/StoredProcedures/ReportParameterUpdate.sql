CREATE PROCEDURE [dbo].[ReportParameterUpdate]
    @ParameterId INT,
    @ReportId INT,
    @ParameterName NVARCHAR(100),
    @DisplayLabel NVARCHAR(200),
    @DataType INT = 1,
    @DefaultValue NVARCHAR(500) = NULL,
    @IsRequired BIT = 1,
    @DisplayOrder INT = 0,
    @LookupSource NVARCHAR(1000) = NULL,
    @Placeholder NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[ReportParameter]
    SET ReportId = @ReportId,
        ParameterName = @ParameterName,
        DisplayLabel = @DisplayLabel,
        DataType = @DataType,
        DefaultValue = @DefaultValue,
        IsRequired = @IsRequired,
        DisplayOrder = @DisplayOrder,
        LookupSource = @LookupSource,
        Placeholder = @Placeholder
    WHERE ParameterId = @ParameterId;
    
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

