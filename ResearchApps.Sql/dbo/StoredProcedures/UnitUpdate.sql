CREATE PROCEDURE [dbo].[UnitUpdate]
@UnitId int,
@UnitName nvarchar(20),
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [Unit]
	SET UnitName = @UnitName
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [UnitId] = @UnitId

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

