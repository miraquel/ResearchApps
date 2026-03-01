CREATE PROCEDURE [dbo].[Ps_SelectById]
@RecId int = 1
AS
BEGIN
	SELECT a.[PsId]
      ,a.[PsDate]
      ,a.[Descr]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[PsStatusId]
      ,[PsStatusName] = CASE   
		  WHEN a.[PsStatusId] = 0 THEN CONCAT('<span class="badge bg-warning">',s.[PsStatusName],'</span>')  
		  WHEN a.[PsStatusId] = 1 THEN CONCAT('<span class="badge bg-success">',s.[PsStatusName],'</span>')  
		  WHEN a.[PsStatusId] = 2 THEN CONCAT('<span class="badge bg-primary">',s.[PsStatusName],'</span>')  
		  WHEN a.[PsStatusId] = 3 THEN CONCAT('<span class="badge bg-danger">',s.[PsStatusName],'</span>')  
		END
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Ps] a
  JOIN [PsStatus] s ON s.PsStatusId = a.PsStatusId
  WHERE a.RecId = @RecId
END
GO

