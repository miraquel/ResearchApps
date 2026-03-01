CREATE PROCEDURE [dbo].[Top_SelectById]
@TopId int = 1
AS
BEGIN
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

