CREATE PROCEDURE [dbo].[ItemGroup01_SelectById]
@ItemGroup01Id int = 1
AS
BEGIN
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

