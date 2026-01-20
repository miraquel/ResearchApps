CREATE PROCEDURE [dbo].[ItemTypeInsert]
@ItemTypeName nvarchar(20),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'admin'
AS
BEGIN
	DECLARE @ItemTypeId int
	
	INSERT INTO [ItemType]
	([ItemTypeName], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@ItemTypeName, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @ItemTypeId = SCOPE_IDENTITY()

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

