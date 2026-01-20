CREATE PROCEDURE [dbo].[UnitInsert]
@UnitName nvarchar(20),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @UnitId int
	
	INSERT INTO [Unit]
	([UnitName], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@UnitName, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @UnitId = SCOPE_IDENTITY()

	SELECT a.[UnitId]
      ,a.[UnitName]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Unit] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.UnitId = @UnitId
END

GO

