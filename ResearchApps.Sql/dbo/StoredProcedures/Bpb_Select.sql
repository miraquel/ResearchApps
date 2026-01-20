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
		  WHEN a.[BpbStatusId] = 0 THEN CONCAT('<label class="label label-warning">',s.[BpbStatusName],'</label>')  
		  WHEN a.[BpbStatusId] = 1 THEN CONCAT('<label class="label label-success">',s.[BpbStatusName],'</label>')  
		  WHEN a.[BpbStatusId] = 2 THEN CONCAT('<label class="label label-primary">',s.[BpbStatusName],'</label>')  
		  WHEN a.[BpbStatusId] = 3 THEN CONCAT('<label class="label label-danger">',s.[BpbStatusName],'</label>')  
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

