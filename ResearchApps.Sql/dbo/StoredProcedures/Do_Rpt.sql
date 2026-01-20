CREATE PROCEDURE [dbo].[Do_Rpt]
@StartDate datetime = '1 Jan 2025',
@EndDate datetime = '31 Jan 2025'
AS
BEGIN
	SELECT a.[DoId] as [DO ID]
      ,a.[DoDate] as [DO DATE]
      ,a.[Descr] as [DESCR]
      ,a.[RefId] as [REF]
      ,a.[Notes] as [NOTES HD]
      ,s.[DoStatusName] as [STATUS]
      ,a.[CustomerId] as [CUST ID]
      ,c.[CustomerName] as [CUST NAME]
      ,a.[Dn] as [DN]
      ,b.[ItemId] as [ITEM ID]
      ,i.[ItemName] as [ITEM NAME]
      ,w.[WhName] as [WH]
      ,b.[Qty] as [QTY]
	  ,u.UnitName as [UNIT]
      ,b.[Notes] as [NOTES DT]
	FROM [Do] a
	JOIN [DoLine] b ON b.[DoId] = a.[DoId]
	JOIN [DoStatus] s ON s.DoStatusId = a.DoStatusId
	JOIN [Customer] c ON c.[CustomerId] = a.[CustomerId]
	JOIN [Item] i ON i.[ItemId] = b.[ItemId]
	JOIN [Wh] w ON w.[WhId] = b.[WhId]
	JOIN [Unit] u ON u.[UnitId] = u.[UnitId]
	WHERE a.[DoDate] BETWEEN @StartDate AND @EndDate
	ORDER BY a.[DoDate] ASC
END

GO

