CREATE PROCEDURE [dbo].[ItemDeptSelectById]
@ItemDeptId int = 1
AS
BEGIN
	SELECT a.[ItemDeptId]
      ,a.[ItemDeptName]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [ItemDept] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.ItemDeptId = @ItemDeptId
END

GO

