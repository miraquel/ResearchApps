CREATE PROCEDURE [dbo].[ItemGroup01_Cbo]
@Id int = null,
@Term nvarchar = null
AS
BEGIN
	SELECT a.[ItemGroup01Id]
      ,a.[ItemGroup01Name]
  FROM [ItemGroup01] a
  WHERE a.StatusId = 1 AND
	1 = CASE WHEN @Id IS NULL THEN 1
		WHEN @Id = a.ItemGroup01Id THEN 1
		ELSE 0 
	END AND
	1 = CASE WHEN @Term IS NULL THEN 1
		WHEN a.ItemGroup01Name LIKE @Term THEN 1
		ELSE 0
	END
END
GO

