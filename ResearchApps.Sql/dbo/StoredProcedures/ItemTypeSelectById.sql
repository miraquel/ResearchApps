CREATE PROCEDURE [dbo].[ItemTypeSelectById]
@ItemTypeId int = 1
AS
BEGIN
	SELECT a.[ItemTypeId]
		,a.[ItemTypeName]
		,a.[StatusId]
		,s.[StatusName]
		,a.[CreatedDate]
		,a.[CreatedBy]
		,a.[ModifiedDate]
		,a.[ModifiedBy]
	FROM [ItemType] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE a.ItemTypeId = @ItemTypeId
END

GO

