CREATE PROCEDURE [dbo].[ItemGroup02_SelectById]
@ItemGroup02Id int = 1
AS
BEGIN
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

