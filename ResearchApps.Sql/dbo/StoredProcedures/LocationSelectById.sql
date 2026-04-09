CREATE PROCEDURE [dbo].[LocationSelectById]
@LocationId int = 1
AS
BEGIN
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

