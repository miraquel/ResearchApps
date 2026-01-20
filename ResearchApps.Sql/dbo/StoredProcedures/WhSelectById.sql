CREATE PROCEDURE [dbo].[WhSelectById]
@WhId int = 1
AS
BEGIN
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

