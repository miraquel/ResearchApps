CREATE PROCEDURE [dbo].[ItemGroup01_Update]
@ItemGroup01Id int,
@ItemGroup01Name nvarchar(20),
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [ItemGroup01]
	SET ItemGroup01Name = @ItemGroup01Name
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [ItemGroup01Id] = @ItemGroup01Id

	SELECT a.[ItemGroup01Id]
      ,a.[ItemGroup01Name]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [ItemGroup01] a
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.ItemGroup01Id = @ItemGroup01Id
END
GO

