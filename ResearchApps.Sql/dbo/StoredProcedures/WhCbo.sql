CREATE PROCEDURE [dbo].[WhCbo]
AS
BEGIN
	SELECT a.[WhId]
      ,a.[WhName]
  FROM [Wh] a
  WHERE a.StatusId = 1
END

GO

