CREATE PROCEDURE [dbo].[Ps_Rpt]
@StartDate datetime = '1 Jan 2025',
@EndDate datetime = '31 Dec 2025'
AS
BEGIN
	SELECT a.[PsId]
		  ,a.[PsDate]
		  ,a.[Descr]
		  ,a.[Notes] as [NotesHd]
		  ,b.[ItemId] 
		  ,i.[ItemName] 
		  ,w.WhName 
		  ,b.[Qty] 
		  ,b.[Price] 
		  ,b.[Notes] as [NotesDt]
		  ,s.PsStatusName as [Status]
	  FROM [Ps] a
	  JOIN [PsLine] b ON b.[PsId] = a.[PsId]
	  JOIN [Item] i ON i.ItemId = b.ItemId
	  JOIN [Wh] w ON w.WhId = b.WhId
	  JOIN [PsStatus] s ON s.PsStatusId = a.[PsStatusId]
	  WHERE a.[PsDate] BETWEEN @StartDate AND @EndDate
	  ORDER BY a.[PsDate] ASC
END

GO

