CREATE PROCEDURE [dbo].[PrStatusCbo]
    @Id INT = NULL,
    @Term NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [PrStatusId],
        [PrStatusName]
    FROM [dbo].[PrStatus]
    WHERE 
        1 = CASE
            WHEN @Id IS NULL THEN 1
            WHEN PrStatusId = @Id THEN 1
            ELSE 0
        END 
        AND
        1 = CASE
            WHEN @Term IS NULL THEN 1
            WHEN PrStatusName LIKE @Term THEN 1
            ELSE 0
        END
    ORDER BY [PrStatusId]
END

GO

