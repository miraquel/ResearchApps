CREATE PROCEDURE [dbo].[UnitCbo]
AS
BEGIN
	SELECT a.[UnitId]
      ,a.[UnitName]
  FROM [Unit] a
  WHERE a.StatusId = 1
END

GO

