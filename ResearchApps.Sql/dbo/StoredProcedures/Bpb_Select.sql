CREATE PROCEDURE [dbo].[Bpb_Select]
AS
BEGIN
	SELECT a.[BpbId]
      ,a.[BpbDate]
      ,CONVERT(VARCHAR(11),a.[BpbDate],106) as [BpbDateStr]
      ,a.[Descr]
      ,a.[RefType]
      ,a.[RefId]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[BpbStatusId]
	  ,[BpbStatusName] = CASE   
		  WHEN a.[BpbStatusId] = 0 THEN CONCAT('<span class="badge bg-warning">',s.[BpbStatusName],'</span>')  
		  WHEN a.[BpbStatusId] = 1 THEN CONCAT('<span class="badge bg-success">',s.[BpbStatusName],'</span>')  
		  WHEN a.[BpbStatusId] = 2 THEN CONCAT('<span class="badge bg-primary">',s.[BpbStatusName],'</span>')  
		  WHEN a.[BpbStatusId] = 3 THEN CONCAT('<span class="badge bg-danger">',s.[BpbStatusName],'</span>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Bpb] a
  JOIN [BpbStatus] s ON s.BpbStatusId = a.BpbStatusId
  ORDER BY BpbId DESC
END

GO

