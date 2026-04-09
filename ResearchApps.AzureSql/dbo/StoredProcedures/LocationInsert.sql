CREATE PROCEDURE [dbo].[LocationInsert]
@LocationName nvarchar(20),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @LocationId int

	INSERT INTO [Location]
	([LocationName], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@LocationName, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @LocationId = SCOPE_IDENTITY()

	SELECT a.[LocationId]
      ,a.[LocationName]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Location] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.LocationId = @LocationId
END

GO

