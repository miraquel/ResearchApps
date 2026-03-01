CREATE PROCEDURE [dbo].[Top_Cbo]
@Id int = null,
@Term nvarchar = null
AS
BEGIN
	SELECT a.[TopId]
      ,a.[TopName]
      ,a.[TopDay]
  FROM [Top] a
  WHERE a.StatusId = 1 AND
	1 = CASE WHEN @Id IS NULL THEN 1
		WHEN @Id = a.TopId THEN 1
		ELSE 0 
	END AND
	1 = CASE WHEN @Term IS NULL THEN 1
		WHEN a.TopName LIKE @Term THEN 1
		ELSE 0
	END
END
GO

