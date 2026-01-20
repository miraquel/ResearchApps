CREATE PROCEDURE [dbo].[ItemTypeCbo]
@Id int = null,
@Term nvarchar = null
AS
BEGIN
	SELECT a.[ItemTypeId]
		,a.[ItemTypeName]
	FROM [ItemType] a
	WHERE a.StatusId = 1 AND 
		1 = CASE WHEN @Id IS NULL THEN 1
			WHEN a.ItemTypeId = @Id THEN 1
			ELSE 0
			END AND
		1 = CASE WHEN @Term IS NULL THEN 1
			WHEN a.ItemTypeName LIKE @Term THEN 1
			ELSE 0
			END
END

GO

