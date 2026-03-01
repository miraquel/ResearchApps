CREATE PROCEDURE [dbo].[ItemGroup02_Update]
@ItemGroup02Id int,
@ItemGroup02Name nvarchar(20),
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [ItemGroup02]
	SET ItemGroup02Name = @ItemGroup02Name
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [ItemGroup02Id] = @ItemGroup02Id

	SELECT a.[ItemGroup02Id]
      ,a.[ItemGroup02Name]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [ItemGroup02] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.ItemGroup02Id = @ItemGroup02Id
END
GO

