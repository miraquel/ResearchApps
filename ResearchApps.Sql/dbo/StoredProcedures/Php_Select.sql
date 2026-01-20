CREATE PROCEDURE [dbo].[Php_Select]
AS
BEGIN
	SELECT a.[PhpId]
      ,a.[PhpDate]
      ,CONVERT(VARCHAR(11),a.[PhpDate],106) as [PhpDateStr]
      ,a.[Descr]
	  ,a.[RefId]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[PhpStatusId]
	  ,[PhpStatusName] = CASE   
		  WHEN a.[PhpStatusId] = 0 THEN CONCAT('<label class="label label-warning">',s.[PhpStatusName],'</label>')  
		  WHEN a.[PhpStatusId] = 1 THEN CONCAT('<label class="label label-success">',s.[PhpStatusName],'</label>')  
		  WHEN a.[PhpStatusId] = 2 THEN CONCAT('<label class="label label-primary">',s.[PhpStatusName],'</label>')  
		  WHEN a.[PhpStatusId] = 3 THEN CONCAT('<label class="label label-danger">',s.[PhpStatusName],'</label>')  
		END   
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Php] a
  JOIN [PhpStatus] s ON s.PhpStatusId = a.PhpStatusId
  ORDER BY PhpId DESC
END

GO

