CREATE PROCEDURE [dbo].[ItemDeptCbo]
@Id int = null,
@Term nvarchar = null
AS
BEGIN
	SELECT a.[ItemDeptId]
      ,a.[ItemDeptName]
  FROM [ItemDept] a
  WHERE a.StatusId = 1 AND
	1 = CASE WHEN @Id IS NULL THEN 1
		WHEN @Id = a.ItemDeptId THEN 1
		ELSE 0 
	END AND
	1 = CASE WHEN @Term IS NULL THEN 1
		WHEN a.ItemDeptName LIKE @Term THEN 1
		ELSE 0
	END
END

GO

