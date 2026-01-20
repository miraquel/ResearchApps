CREATE PROCEDURE [dbo].[ItemTypeUpdate]
@ItemTypeId int,
@ItemTypeName nvarchar(20),
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [ItemType]
	SET ItemTypeName = @ItemTypeName
		,StatusId = @StatusId
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE [ItemTypeId] = @ItemTypeId

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

