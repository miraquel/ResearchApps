CREATE PROCEDURE [dbo].[Top_Insert]
@TopName nvarchar(20),
@TopDay int,
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @TopId int
	
	INSERT INTO [Top]
	([TopName], [TopDay], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@TopName, @TopDay, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @TopId = SCOPE_IDENTITY()

	SELECT a.[TopId]
      ,a.[TopName]
      ,a.[TopDay]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Top] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.TopId = @TopId
END
GO

