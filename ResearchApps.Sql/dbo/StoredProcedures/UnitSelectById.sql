CREATE PROCEDURE [dbo].[UnitSelectById]
@UnitId int = 1
AS
BEGIN
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

