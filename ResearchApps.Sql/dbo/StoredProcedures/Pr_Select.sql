CREATE PROCEDURE [dbo].[Pr_Select]
    @PageNumber int = 1,
    @PageSize int = 10,
    @SortOrder nvarchar(max) = 'DESC',
    @SortColumn nvarchar(max) = 'PrId',
    @PrId nvarchar(255) = NULL,
    @PrName nvarchar(255) = NULL,
    @PrDateFrom datetime = NULL,
    @PrDateTo datetime = NULL,
    @BudgetId int = NULL,
    @PrStatusId int = NULL,
    @Total decimal(18,2) = NULL,
    @TotalOperator nvarchar(2) = '=',
    @CreatedDateFrom datetime = NULL,
    @CreatedDateTo datetime = NULL,
    @CreatedBy nvarchar(255) = NULL,
    @ModifiedDateFrom datetime = NULL,
    @ModifiedDateTo datetime = NULL,
    @ModifiedBy nvarchar(255) = NULL
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
  WHERE 1 = CASE
            WHEN @PrId IS NULL THEN 1
            WHEN a.PrId = @PrId THEN 1
            ELSE 0 END 
        AND
        1 = CASE
            WHEN @PrName IS NULL THEN 1
            WHEN a.PrName LIKE '%' + @PrName + '%' THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @PrDateFrom IS NULL THEN 1
            WHEN a.PrDate >= @PrDateFrom THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @PrDateTo IS NULL THEN 1
            WHEN a.PrDate <= @PrDateTo THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @BudgetId IS NULL THEN 1
            WHEN a.BudgetId = @BudgetId THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @PrStatusId IS NULL THEN 1
            WHEN a.PrStatusId = @PrStatusId THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @Total IS NULL THEN 1
            WHEN @TotalOperator = '=' AND a.Total = @Total THEN 1
            WHEN @TotalOperator = '>' AND a.Total > @Total THEN 1
            WHEN @TotalOperator = '>=' AND a.Total >= @Total THEN 1
            WHEN @TotalOperator = '<' AND a.Total < @Total THEN 1
            WHEN @TotalOperator = '<=' AND a.Total <= @Total THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @CreatedDateFrom IS NULL THEN 1
            WHEN a.CreatedDate >= @CreatedDateFrom THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @CreatedDateTo IS NULL THEN 1
            WHEN a.CreatedDate <= @CreatedDateTo THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @CreatedBy IS NULL THEN 1
            WHEN a.CreatedBy LIKE '%' + @CreatedBy + '%' THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @ModifiedDateFrom IS NULL THEN 1
            WHEN a.ModifiedDate >= @ModifiedDateFrom THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @ModifiedDateTo IS NULL THEN 1
            WHEN a.ModifiedDate <= @ModifiedDateTo THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @ModifiedBy IS NULL THEN 1
            WHEN a.ModifiedBy LIKE '%' + @ModifiedBy + '%' THEN 1
            ELSE 0 END
    ORDER BY 
      CASE WHEN @SortColumn = 'PrId' AND @SortOrder = 'ASC' THEN a.PrId END ASC,
      CASE WHEN @SortColumn = 'PrId' AND @SortOrder = 'DESC' THEN a.PrId END DESC,
      CASE WHEN @SortColumn = 'PrName' AND @SortOrder = 'ASC' THEN a.PrName END ASC,
      CASE WHEN @SortColumn = 'PrName' AND @SortOrder = 'DESC' THEN a.PrName END DESC,
      CASE WHEN @SortColumn = 'PrDate' AND @SortOrder = 'ASC' THEN a.PrDate END ASC,
      CASE WHEN @SortColumn = 'PrDate' AND @SortOrder = 'DESC' THEN a.PrDate END DESC,
      CASE WHEN @SortColumn = 'BudgetId' AND @SortOrder = 'ASC' THEN a.BudgetId END ASC,
      CASE WHEN @SortColumn = 'BudgetId' AND @SortOrder = 'DESC' THEN a.BudgetId END DESC,
      CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'ASC' THEN a.Total END ASC,
      CASE WHEN @SortColumn = 'Total' AND @SortOrder = 'DESC' THEN a.Total END DESC,
      CASE WHEN @SortColumn = 'PrStatusId' AND @SortOrder = 'ASC' THEN a.PrStatusId END ASC,
      CASE WHEN @SortColumn = 'PrStatusId' AND @SortOrder = 'DESC' THEN a.PrStatusId END DESC,
      CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN a.CreatedDate END ASC,
      CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN a.CreatedDate END DESC,
      CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'ASC' THEN a.ModifiedDate END ASC,
      CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'DESC' THEN a.ModifiedDate END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
  
  SELECT COUNT(*) AS TotalRecords
  FROM [Pr] a
  WHERE 1 = CASE
            WHEN @PrId IS NULL THEN 1
            WHEN a.PrId = @PrId THEN 1
            ELSE 0 END
        AND 
        1 = CASE
            WHEN @PrName IS NULL THEN 1
            WHEN a.PrName LIKE '%' + @PrName + '%' THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @PrDateFrom IS NULL THEN 1
            WHEN a.PrDate >= @PrDateFrom THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @PrDateTo IS NULL THEN 1
            WHEN a.PrDate <= @PrDateTo THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @BudgetId IS NULL THEN 1
            WHEN a.BudgetId = @BudgetId THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @PrStatusId IS NULL THEN 1
            WHEN a.PrStatusId = @PrStatusId THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @Total IS NULL THEN 1
            WHEN @TotalOperator = '=' AND a.Total = @Total THEN 1
            WHEN @TotalOperator = '>' AND a.Total > @Total THEN 1
            WHEN @TotalOperator = '>=' AND a.Total >= @Total THEN 1
            WHEN @TotalOperator = '<' AND a.Total < @Total THEN 1
            WHEN @TotalOperator = '<=' AND a.Total <= @Total THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @CreatedDateFrom IS NULL THEN 1
            WHEN a.CreatedDate >= @CreatedDateFrom THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @CreatedDateTo IS NULL THEN 1
            WHEN a.CreatedDate <= @CreatedDateTo THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @CreatedBy IS NULL THEN 1
            WHEN a.CreatedBy LIKE '%' + @CreatedBy + '%' THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @ModifiedDateFrom IS NULL THEN 1
            WHEN a.ModifiedDate >= @ModifiedDateFrom THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @ModifiedDateTo IS NULL THEN 1
            WHEN a.ModifiedDate <= @ModifiedDateTo THEN 1
            ELSE 0 END
        AND
        1 = CASE
            WHEN @ModifiedBy IS NULL THEN 1
            WHEN a.ModifiedBy LIKE '%' + @ModifiedBy + '%' THEN 1
            ELSE 0 END;
END

GO

