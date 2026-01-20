CREATE PROCEDURE [dbo].[WhInsert]
@WhName nvarchar(20),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @WhId int
	
	INSERT INTO [Wh]
	([WhName], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@WhName, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @WhId = SCOPE_IDENTITY()

	SELECT a.[WhId]
      ,a.[WhName]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Wh] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.WhId = @WhId
END

GO

