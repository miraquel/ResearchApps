CREATE PROCEDURE [dbo].[Co_SelectById]
@RecId int=1
AS
BEGIN
	SELECT a.[CoId]
      ,a.[CoDate]
      ,CONVERT(VARCHAR(11),a.[CoDate],106) as [CoDateStr]
      ,a.[CustomerId]
      ,c.[CustomerName]
	  ,c.[Address]
	  ,a.[PoCustomer]
      ,a.[RefNo]
      ,a.[CoTypeId]
      ,t.[CoTypeName]
      ,a.[IsPpn]
      ,ROUND(a.[SubTotal],0) as [SubTotal]
      ,ROUND(a.[Ppn],0) as [Ppn]
      ,ROUND(a.[Total],0) as [Total]
      ,a.[Notes]
      ,a.[CoStatusId]
      ,[CoStatusName] = CASE
        WHEN a.[CoStatusId] = 0 THEN CONCAT('<span class="badge bg-secondary">',s.[CoStatusName],'</span>')
        WHEN a.[CoStatusId] = 1 THEN CONCAT('<span class="badge bg-success">',s.[CoStatusName],'</span>')
        WHEN a.[CoStatusId] = 2 THEN CONCAT('<span class="badge bg-warning">',s.[CoStatusName],'</span>')
        WHEN a.[CoStatusId] = 3 THEN CONCAT('<span class="badge bg-active">',s.[CoStatusName],'</span>')
        WHEN a.[CoStatusId] = 4 THEN CONCAT('<span class="badge bg-primary">',s.[CoStatusName],'</span>')
        WHEN a.[CoStatusId] = 5 THEN CONCAT('<span class="badge bg-danger">',s.[CoStatusName],'</span>')
        ELSE 'NA'
        END
	  ,a.[Revision]
	  ,isnull(a.WfTransId,0) as WfTransId
	  ,w.UserId as CurrentApprover
	  ,w.[Index] as CurrentIndex
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Co] a
  JOIN [CoStatus] s ON s.CoStatusId = a.CoStatusId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  JOIN [CoType] t ON t.CoTypeId = a.CoTypeId
  LEFT JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
  WHERE a.RecId = @RecId
END

GO

