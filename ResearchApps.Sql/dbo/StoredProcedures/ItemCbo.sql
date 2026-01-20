CREATE PROCEDURE [dbo].[ItemCbo]
    @Id INT = NULL,
    @Term NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [ItemId],
        CONCAT([ItemId], ' ', [ItemName]) AS [ItemName]
    FROM [dbo].[Item]
    WHERE 
        1 = CASE
            WHEN @Id IS NULL THEN 1
            WHEN ItemId = @Id THEN 1
            ELSE 0
        END AND
        1 = CASE
            WHEN @Term IS NULL THEN 1
            WHEN ItemName LIKE @Term THEN 1
            ELSE 0
        END
    ORDER BY [ItemName]
END

GO

