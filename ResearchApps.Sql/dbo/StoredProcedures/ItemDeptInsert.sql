CREATE PROCEDURE [dbo].[ItemDeptInsert]
@ItemDeptName nvarchar(20),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @ItemDeptId int
	
	INSERT INTO [ItemDept]
	([ItemDeptName], [StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@ItemDeptName, @StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @ItemDeptId = SCOPE_IDENTITY()

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

