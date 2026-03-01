CREATE PROCEDURE [dbo].[ItemGroup02_Insert]
@ItemGroup02Name nvarchar(20),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @ItemGroup02Id int
	
	INSERT INTO [ItemGroup02]
	([ItemGroup02Name], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@ItemGroup02Name, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @ItemGroup02Id = SCOPE_IDENTITY()

	SELECT a.[ItemGroup02Id]
      ,a.[ItemGroup02Name]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [ItemGroup02] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.ItemGroup02Id = @ItemGroup02Id
END
GO

