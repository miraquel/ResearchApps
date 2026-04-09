CREATE PROCEDURE [dbo].[LocationUpdate]
@LocationId int,
@LocationName nvarchar(20),
@StatusId int,
@ModifiedBy nvarchar(20)
AS
BEGIN
	UPDATE [Location]
	SET LocationName = @LocationName
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [LocationId] = @LocationId

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

