CREATE PROCEDURE [dbo].[ItemGroup01_Insert]
@ItemGroup01Name nvarchar(20),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @ItemGroup01Id int
	
	INSERT INTO [ItemGroup01]
	([ItemGroup01Name], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@ItemGroup01Name, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @ItemGroup01Id = SCOPE_IDENTITY()

	SELECT a.[ItemGroup01Id]
      ,a.[ItemGroup01Name]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [ItemGroup01] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.ItemGroup01Id = @ItemGroup01Id
END
GO

