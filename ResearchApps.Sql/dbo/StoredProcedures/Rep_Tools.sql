CREATE PROCEDURE [dbo].[Rep_Tools]
@Year int = 2026,
@Month int = 1
AS
BEGIN
	SELECT 
		@Year as [Year]
		, @Month as [Month]
		, DATENAME(month, max(a.TransDate)) + ' ' + CAST(@Year as varchar) as [MonthName]
		, a.[ItemId]
		, i.ItemName
		, u.UnitName
		, it.ItemTypeName
		, id.ItemDeptName
		, ISNULL(bpb.ProdId,'') as ProdId
		, ISNULL(fg.ItemName,'') as ProductName
		, SUM(-1*a.[Qty]) as Qty
		, MAX(a.CostPrice) as [CostPrice]
		, SUM(-1*a.[Value]) as Value
	FROM [InventTrans] a
	JOIN Item i ON i.ItemId = a.ItemId
	JOIN Unit u ON u.UnitId = i.UnitId
	JOIN ItemType it ON it.ItemTypeId = i.ItemTypeId
	JOIN ItemDept id ON id.ItemDeptId = i.ItemDeptId
	LEFT JOIN BpbLine bpb  ON bpb.BpbLineId = a.RefId AND a.RefType = 'Pengambilan Barang'
	LEFT JOIN Prod p  ON p.ProdId = bpb.ProdId
	LEFT JOIN Item fg  ON fg.ItemId = p.ItemId
	WHERE a.RefType = 'Pengambilan Barang'
		AND YEAR(a.TransDate) = @Year AND MONTH(a.TransDate) = @Month
	GROUP BY a.[ItemId]
		, i.ItemName
		, u.UnitName
		, it.ItemTypeName
		, id.ItemDeptName
		, bpb.ProdId
		, fg.ItemName
	ORDER BY a.ItemId, bpb.ProdId
END
GO

