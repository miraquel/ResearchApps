CREATE PROCEDURE [dbo].[Mc_Rpt]
@StartDate datetime = '1 Jan 2025',
@EndDate datetime = '31 Dec 2025'
AS
BEGIN
	SELECT a.[McId] as [MC ID]
		  ,a.[McDate] as [MC DATE]
		  ,a.[CustomerId] as [CUST ID]
		  ,c.[CustomerName]  as [CUST NAME]
		  ,a.[SjNo] as [SJ]
		  ,a.[RefNo]  as [REF]
		  ,a.[Notes] as [NOTES HD]
		  ,b.[ItemId] as [ITEM ID]
		  ,i.[ItemName] as [ITEM NAME]
		  ,w.WhName as [WH]
		  ,b.[Qty] as [QTY]
		  ,b.[Price] as [PRICE]
		  ,b.[Notes] as [NOTES DT]
		  ,s.McStatusName as [STATUS]
	  FROM [Mc] a
	  JOIN [McLine] b ON b.McId = a.McId
	  JOIN [Customer] c ON c.CustomerId = a.CustomerId
	  JOIN [Item] i ON i.ItemId = b.ItemId
	  JOIN [Wh] w ON w.WhId = b.WhId
	  JOIN [McStatus] s ON s.McStatusId = a.McStatusId
	  WHERE a.[McDate] BETWEEN @StartDate AND @EndDate
	  ORDER BY a.[McDate] ASC
END

GO

