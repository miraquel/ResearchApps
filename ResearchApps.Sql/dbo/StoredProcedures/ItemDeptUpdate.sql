CREATE PROCEDURE [dbo].[ItemDeptUpdate]
@ItemDeptId int,
@ItemDeptName nvarchar(20),
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [ItemDept]
	SET ItemDeptName = @ItemDeptName
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [ItemDeptId] = @ItemDeptId

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

