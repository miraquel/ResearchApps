CREATE PROCEDURE [dbo].[Ps_Select]
AS
BEGIN
	SELECT a.[PsId]
      ,a.[PsDate]
      ,CONVERT(VARCHAR(11),a.[PsDate],106) as [PsDateStr]
      ,a.[Descr]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[PsStatusId]
	  ,[PsStatusName] = CASE   
		  WHEN a.[PsStatusId] = 0 THEN CONCAT('<label class="label label-warning">',s.[PsStatusName],'</label>')  
		  WHEN a.[PsStatusId] = 1 THEN CONCAT('<label class="label label-success">',s.[PsStatusName],'</label>')  
		  WHEN a.[PsStatusId] = 2 THEN CONCAT('<label class="label label-primary">',s.[PsStatusName],'</label>')  
		  WHEN a.[PsStatusId] = 3 THEN CONCAT('<label class="label label-danger">',s.[PsStatusName],'</label>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Ps] a
  JOIN [PsStatus] s ON s.PsStatusId = a.PsStatusId
  ORDER BY PsId DESC
END

GO

