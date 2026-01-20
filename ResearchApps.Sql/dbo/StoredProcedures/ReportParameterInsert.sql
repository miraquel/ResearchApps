CREATE PROCEDURE [dbo].[ReportParameterInsert]
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
    
    INSERT INTO [dbo].[ReportParameter] (ReportId, ParameterName, DisplayLabel, DataType, DefaultValue, IsRequired, DisplayOrder, LookupSource, Placeholder)
    VALUES (@ReportId, @ParameterName, @DisplayLabel, @DataType, @DefaultValue, @IsRequired, @DisplayOrder, @LookupSource, @Placeholder);
    
    DECLARE @NewId INT = SCOPE_IDENTITY();
    
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
    WHERE ParameterId = @NewId;
END

GO

