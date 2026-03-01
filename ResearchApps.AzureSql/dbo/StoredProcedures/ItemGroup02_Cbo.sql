CREATE PROCEDURE [dbo].[ItemGroup02_Cbo]
@Id int = null,
@Term nvarchar = null
AS
BEGIN
	SELECT a.[ItemGroup02Id]
      ,a.[ItemGroup02Name]
  FROM [ItemGroup02] a
  WHERE a.StatusId = 1 AND
	1 = CASE WHEN @Id IS NULL THEN 1
		WHEN @Id = a.ItemGroup02Id THEN 1
		ELSE 0 
	END AND
	1 = CASE WHEN @Term IS NULL THEN 1
		WHEN a.ItemGroup02Name LIKE @Term THEN 1
		ELSE 0
	END
END
GO

