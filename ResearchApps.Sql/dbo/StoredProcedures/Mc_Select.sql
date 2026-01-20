CREATE PROCEDURE [dbo].[Mc_Select]
AS
BEGIN
	SELECT a.[McId]
      ,a.[McDate]
      ,CONVERT(VARCHAR(11),a.[McDate],106) as [McDateStr]
      ,a.[CustomerId]
      ,c.[CustomerName]
	  ,a.[SjNo]
      ,a.[RefNo]
      ,a.[Notes]
      ,a.[McStatusId]
	  ,[McStatusName] = CASE   
		  WHEN a.[McStatusId] = 0 THEN CONCAT('<label class="label label-warning">',s.[McStatusName],'</label>')  
		  WHEN a.[McStatusId] = 1 THEN CONCAT('<label class="label label-success">',s.[McStatusName],'</label>')  
		  WHEN a.[McStatusId] = 2 THEN CONCAT('<label class="label label-primary">',s.[McStatusName],'</label>')  
		  WHEN a.[McStatusId] = 3 THEN CONCAT('<label class="label label-danger">',s.[McStatusName],'</label>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Mc] a
  JOIN [McStatus] s ON s.McStatusId = a.McStatusId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  ORDER BY McId DESC
END

GO

