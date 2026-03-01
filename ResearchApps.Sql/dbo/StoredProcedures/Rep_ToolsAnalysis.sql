CREATE PROCEDURE [dbo].[Rep_ToolsAnalysis]
@Year int = 2026,
@Month int = 1
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @Result TABLE(
		[Year]			int
		,[Month]		int
		,[MonthName]	nvarchar(20)
		,CustomerName	nvarchar(100)
		,ProdId			nvarchar(20)
		,ProductName	nvarchar(100)
		,ItemId			int
		,ItemName		nvarchar(100)
		,UnitName		nvarchar(20)
		,Qty			decimal(18,2)
		,[CostPrice]	decimal(18,2)
		,[Value]		decimal(18,2)
		,[QtyDo]		decimal(18,2)
		,[ToolLife]		decimal(18,2)
		,[ToolCostPerPcs]			decimal(18,2)
		,[SalesPrice]	decimal(18,2)
		,ProductId		int
	)
	INSERT INTO @Result
	SELECT 
		@Year as [Year]
		, @Month as [Month]
		, LEFT(DATENAME(month, max(a.TransDate)),3) + ' ' + CAST(@Year as varchar) as [MonthName]
		, c.CustomerName
		, bpb.ProdId as ProdId
		, fg.ItemName as ProductName
		, a.[ItemId]
		, i.ItemName
		, u.UnitName
		, SUM(-1*a.[Qty]) as Qty
		, MAX(a.CostPrice) as [CostPrice]
		, ROUND( SUM(-1*a.[Value]) ,0) as Value
		, ISNULL(MAX(x.Qty),0) as [QtyDo]
		, ROUND( ISNULL(MAX(x.Qty),0) / SUM(-1*a.[Qty]) ,0) as [ToolLife]
		, ROUND( IIF(ISNULL(MAX(x.Qty),0) = 0, 0 , SUM(-1*a.[Value]) / ISNULL(MAX(x.Qty),0) ) ,2) as [ToolCostPerPcs]
		, MAX(fg.SalesPrice) as [SalesPrice]
		, p.ItemId
	FROM [InventTrans] a
	JOIN Item i ON i.ItemId = a.ItemId
	JOIN Unit u ON u.UnitId = i.UnitId
	JOIN BpbLine bpb  ON bpb.BpbLineId = a.RefId AND a.RefType = 'Pengambilan Barang'
	JOIN Prod p  ON p.ProdId = bpb.ProdId
	JOIN Item fg  ON fg.ItemId = p.ItemId
	JOIN Customer c ON c.CustomerId = p.CustomerId
	LEFT JOIN 
	(
		SELECT b.ItemId, SUM(b.Qty) as Qty 
		FROM Do a
		JOIN DoLine b ON b.DoId = a.DoId
		WHERE YEAR(a.DoDate) = @Year AND MONTH(a.DoDate) = @Month
		GROUP BY b.ItemId
	) x ON x.ItemId = p.ItemId
	WHERE a.RefType = 'Pengambilan Barang'
		AND YEAR(a.TransDate) = @Year AND MONTH(a.TransDate) = @Month
		AND i.ItemTypeId = 2 --khusus Tools saja
	GROUP BY c.CustomerName
		, bpb.ProdId 
		, fg.ItemName 
		, a.[ItemId]
		, i.ItemName
		, u.UnitName
		,p.ItemId
	ORDER BY c.CustomerName

--output
	DECLARE @Result2 TABLE(
		[Year]			int
		,[Month]		int
		,[MonthName]	nvarchar(20)
		,CustomerName	nvarchar(100)
		,ProdId			nvarchar(200)
		,ProductName	nvarchar(100)
		,ItemId			int
		,ItemName		nvarchar(100)
		,UnitName		nvarchar(20)
		,Qty			decimal(18,2)
		,[CostPrice]	decimal(18,2)
		,[Value]		decimal(18,2)
		,[QtyDo]		decimal(18,2)
		,[ToolLife]		decimal(18,2)
		,[ToolCostPerPcs]			decimal(18,2)
		,[SalesPrice]	decimal(18,2)
	)
	INSERT INTO @Result2
	SELECT [Year]
		,[Month]	
		,[MonthName]
		,CustomerName
		,SUBSTRING(
			(
				SELECT ','+p.ProdId  AS [text()]
				FROM (SELECT DISTINCT ProductId, ProdId FROM @Result) p
				WHERE p.ProductId = a.ProductId
				ORDER BY p.ProductId
				FOR XML PATH ('')
			), 2, 200) as [ProdId]
		,ProductName
		,ItemId	
		,ItemName
		,UnitName
		,Qty
		,[CostPrice]
		,[Value]
		,[QtyDo]
		,[ToolLife]	
		,[ToolCostPerPcs]
		,[SalesPrice]
	FROM @Result a

	SELECT [Year]
		,[Month]	
		,[MonthName]
		,CustomerName
		,[ProdId]
		,ProductName
		,ItemId	
		,ItemName
		,UnitName
		,Qty
		,[CostPrice]
		,[Value]
		,[QtyDo]
		,[ToolLife]	
		,[ToolCostPerPcs]
		,[SalesPrice] 
	FROM @Result2
	ORDER BY CustomerName, ProdId
END
GO

