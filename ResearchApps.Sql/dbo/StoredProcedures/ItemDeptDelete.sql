CREATE PROCEDURE [dbo].[ItemDeptDelete]
@ItemDeptId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [ItemDept]
	WHERE ItemDeptId = @ItemDeptId
END

GO

