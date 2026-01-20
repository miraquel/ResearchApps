CREATE PROCEDURE [dbo].[Pr_SelectById]
@RecId int = 1
AS
BEGIN
  SELECT a.[PrId]
      ,a.[PrName]
      ,a.[PrDate]
      ,CONVERT(varchar,a.[PrDate],106) as PrDateStr
      ,a.[BudgetId]
      ,b.[BudgetName]
      ,a.[RequestDate]
      ,ROUND(a.[Total],0) as [Total]
      ,a.[Notes]
      ,a.[PrStatusId]
	  ,[PrStatusName] = CASE
		  WHEN a.[PrStatusId] = 0 THEN CONCAT('<span class="badge bg-warning">',s.PrStatusName,'</span>')  
		  WHEN a.[PrStatusId] = 1 THEN CONCAT('<span class="badge bg-primary">',s.PrStatusName,'</span>') 
		  WHEN a.[PrStatusId] = 2 THEN CONCAT('<span class="badge bg-info">',s.PrStatusName,'</span>')
	      WHEN a.[PrStatusId] = 3 THEN CONCAT('<span class="badge bg-success">',s.PrStatusName,'</span>') 
          WHEN a.[PrStatusId] = 4 THEN CONCAT('<span class="badge bg-secondary">',s.PrStatusName,'</span>') 
          WHEN a.[PrStatusId] = 5 THEN CONCAT('<span class="badge bg-danger">',s.PrStatusName,'</span>') 
          ELSE s.PrStatusName
		END
      ,wt.[UserId] AS [CurrentApprover]
      ,wt.[Index] AS [CurrentIndex]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Pr] a
  JOIN [Budget] b ON b.BudgetId = a.BudgetId
  JOIN [PrStatus] s ON s.PrStatusId = a.PrStatusId
  LEFT JOIN [dbo].[WfTrans] wt ON a.[WfTransId] = wt.[WfTransId]
  WHERE a.RecId = @RecId
END

GO

