CREATE PROCEDURE [dbo].[Bpb_Rpt]
@StartDate datetime = '1 Jan 2025',
@EndDate datetime = '31 Dec 2025'
AS
BEGIN
	SELECT a.[BpbId] as [BPB ID]
		  ,a.[BpbDate] as [DATE]
		  ,a.[Descr] as [DESCR]
		  ,a.[RefType] as [REF TYPE]
		  ,a.[RefId]  as [REF ID]
		  ,a.[Notes] as [NOTES HD]
		  ,b.[ItemId] as [ITEM ID]
		  ,i.[ItemName] as [ITEM NAME]
		  ,w.WhName as [WH]
		  ,b.[Qty] as [QTY]
		  ,b.[Price] as [PRICE]
		  ,b.[ProdId] as [PRO ID]
		  ,b.[Notes] as [NOTES DT]
		  ,s.BpbStatusName as [STATUS]
	  FROM [Bpb] a
	  JOIN [BpbLine] b ON b.[BpbId] = a.[BpbId]
	  JOIN [Item] i ON i.ItemId = b.ItemId
	  JOIN [Wh] w ON w.WhId = b.WhId
	  JOIN [BpbStatus] s ON s.BpbStatusId = a.[BpbStatusId]
	  WHERE a.[BpbDate] BETWEEN @StartDate AND @EndDate
	  ORDER BY a.[BpbDate] ASC
END

GO

