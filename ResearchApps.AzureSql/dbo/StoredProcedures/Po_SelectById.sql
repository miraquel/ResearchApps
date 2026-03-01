CREATE PROCEDURE [dbo].[Po_SelectById]
@RecId int = 4
AS
BEGIN
	SELECT a.[PoId]
      ,a.[PoDate]
      ,CONVERT(VARCHAR(11),a.[PoDate],106) as [PoDateStr]
      ,a.[SupplierId]
      ,b.[SupplierName] + ' (' + a.Pic + ')' as [SupplierName]
      ,a.[Pic]
      ,b.[Address] + ' ' + b.[City] as [Address]
      ,b.[Telp]
      ,b.[Fax]
      ,b.[Email]
      ,a.[RefNo]
      ,a.[IsPpn]
      ,ROUND(a.[SubTotal],0) as [SubTotal]
      ,ROUND(a.[Ppn],0) as [Ppn]
      ,ROUND(a.[Total],0) as [Total]
      ,a.[Notes]
      ,a.[PoStatusId]
      ,[PoStatusName] = CASE a.[PoStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', s.[PoStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', s.[PoStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-info">', s.[PoStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-info">', s.[PoStatusName], '</span>')
            WHEN 4 THEN CONCAT('<span class="badge bg-secondary">', s.[PoStatusName], '</span>')
            WHEN 5 THEN CONCAT('<span class="badge bg-danger">', s.[PoStatusName], '</span>')
            ELSE 'NA'
        END
      ,wt.[UserId] AS [CurrentApprover]
      ,wt.[Index] AS [CurrentIndex]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Po] a
  JOIN [Supplier] b ON b.SupplierId = a.SupplierId
  JOIN [PoStatus] s ON s.PoStatusId = a.PoStatusId
  LEFT JOIN [dbo].[WfTrans] wt ON a.[WfTransId] = wt.[WfTransId]
  WHERE a.RecId = @RecId
END
GO

