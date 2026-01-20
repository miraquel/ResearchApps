CREATE PROCEDURE [dbo].[Php_Rpt]
@StartDate datetime = '1 Jan 2025',
@EndDate datetime = '31 Dec 2025'
AS
BEGIN
	SELECT a.[PhpId] as [PHP ID]
		  ,a.[PhpDate] as [DATE]
		  ,a.[Descr] as [DESCR]
		  ,a.[RefId] as [REF ID]
		  ,a.[Notes] as [NOTES HD]
		  ,b.[ItemId] as [ITEM ID]
		  ,i.[ItemName] as [ITEM NAME]
		  ,w.WhName as [WH]
		  ,b.[Qty] as [QTY]
		  ,b.[Price] as [PRICE]
		  ,b.[ProdId] as [PRO ID]
		  ,b.[Notes] as [NOTES DT]
		  ,s.PhpStatusName as [STATUS]
	  FROM [Php] a
	  JOIN [PhpLine] b ON b.[PhpId] = a.[PhpId]
	  JOIN [Item] i ON i.ItemId = b.ItemId
	  JOIN [Wh] w ON w.WhId = b.WhId
	  JOIN [PhpStatus] s ON s.PhpStatusId = a.[PhpStatusId]
	  WHERE a.[PhpDate] BETWEEN @StartDate AND @EndDate
	  ORDER BY a.[PhpDate] ASC
END

GO

