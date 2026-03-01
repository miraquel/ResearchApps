CREATE PROCEDURE [dbo].[Rep_InventTrans_ByItem]
@ItemId int = 1001,
@StartDate datetime = null,
@EndDate datetime = null
AS
BEGIN
	SELECT a.[ItemId]
		, i.ItemName
		, a.[WhId]
		, w.WhName
		, a.[TransDate]
        ,CONVERT(varchar,a.[TransDate],106) as [TransDateStr]
		, a.[RefType]
		, a.[RefNo]
		, a.[RefId]
		, a.[Qty]
		, u.UnitName
		, a.[Value]
	FROM [InventTrans] a
	JOIN Item i ON i.ItemId = a.ItemId
	JOIN Wh w ON w.WhId = a.WhId
	JOIN Unit u ON u.UnitId = i.UnitId
	WHERE a.ItemId = @ItemId
		AND (@StartDate IS NULL OR a.[TransDate] >= @StartDate)
		AND (@EndDate IS NULL OR a.[TransDate] <= @EndDate)
	ORDER BY a.TransDate desc, a.RecId desc
END
GO

