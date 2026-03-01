CREATE PROCEDURE [dbo].[ItemSelectById]
@ItemId int = 1001
AS
BEGIN
	SELECT a.[ItemId]
      ,a.[ItemName]
      ,a.[ItemTypeId]
      ,b.[ItemTypeName]
      ,a.[ItemDeptId]
      ,c.[ItemDeptName]
      ,a.[ItemGroup01Id]
      ,g1.[ItemGroup01Name]
      ,a.[ItemGroup02Id]
      ,g2.[ItemGroup02Name]
      ,a.[BufferStock]
      ,a.[UnitId]
      ,u.[UnitName]
      ,a.[WhId]
      ,w.[WhName]
      ,a.[PurchasePrice]
      ,a.[SalesPrice]
      ,a.[CostPrice]
      ,a.[Image]
      ,a.[Notes]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Item] a
  JOIN [ItemType] b ON b.ItemTypeId = a.ItemTypeId
  JOIN [ItemDept] c ON c.[ItemDeptId] = a.[ItemDeptId]
  LEFT JOIN [ItemGroup01] g1 ON g1.[ItemGroup01Id] = a.[ItemGroup01Id]
  LEFT JOIN [ItemGroup02] g2 ON g2.[ItemGroup02Id] = a.[ItemGroup02Id]
  JOIN [Unit] u ON u.UnitId = a.UnitId
  JOIN [Wh] w ON w.WhId = a.WhId
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.ItemId = @ItemId
END

GO

