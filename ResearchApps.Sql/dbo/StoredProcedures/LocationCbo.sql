CREATE PROCEDURE [dbo].[LocationCbo]
AS
BEGIN
	SELECT a.[LocationId]
      ,a.[LocationName]
  FROM [Location] a
  WHERE a.StatusId = 1
END

GO

