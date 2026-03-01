CREATE PROCEDURE [dbo].[Prod_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = 'ProdId',
    @SortOrder NVARCHAR(4) = 'DESC',
    @RecId INT = NULL,
    @ProdId NVARCHAR(20) = NULL,
    @ProdDate DATETIME = NULL,
    @ProdDateFrom DATETIME = NULL,
    @ProdDateTo DATETIME = NULL,
    @CustomerId INT = NULL,
    @CustomerName NVARCHAR(50) = NULL,
    @ItemId INT = NULL,
    @ItemName NVARCHAR(100) = NULL,
    @PlanQty DECIMAL(18,2) = NULL,
    @PlanQtyOperator NVARCHAR(2) = '=',
    @ResultQty DECIMAL(18,2) = NULL,
    @ResultQtyOperator NVARCHAR(2) = '=',
    @ResultValue DECIMAL(18,2) = NULL,
    @ResultValueOperator NVARCHAR(2) = '=',
    @CostPrice DECIMAL(18,2) = NULL,
    @CostPriceOperator NVARCHAR(2) = '=',
    @Notes NVARCHAR(100) = NULL,
    @ProdStatusId INT = NULL,
    @ProdStatusName NVARCHAR(50) = NULL,
    @CreatedDate DATETIME = NULL,
    @CreatedBy NVARCHAR(20) = NULL,
    @ModifiedDate DATETIME = NULL,
    @ModifiedBy NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table eliminates duplicate WHERE clause logic
    SELECT
        a.[RecId],
        a.[ProdId],
        a.[ProdDate],
        a.[CustomerId],
        c.[CustomerName],
        a.[ItemId],
        i.[ItemName],
        u.[UnitName],
        a.[PlanQty],
        a.[ResultQty],
        a.[ResultValue],
        CAST(IIF(a.[ResultQty] > 0, a.[ResultValue] / a.[ResultQty], 0) as decimal(18,2)) as [CostPrice],
        a.[Notes],
        a.[ProdStatusId],
        s.[ProdStatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy]
    INTO #FilteredData
    FROM [Prod] a
    JOIN [ProdStatus] s ON s.ProdStatusId = a.ProdStatusId
    JOIN [Item] i ON i.ItemId = a.ItemId
    JOIN [Unit] u ON u.UnitId = i.UnitId
    JOIN [Customer] c ON c.CustomerId = a.CustomerId
    WHERE (@RecId IS NULL OR a.RecId = @RecId)
      AND (@ProdId IS NULL OR a.ProdId LIKE '%' + @ProdId + '%')
      AND (@ProdDate IS NULL OR a.ProdDate = @ProdDate)
      AND (@ProdDateFrom IS NULL OR a.ProdDate >= @ProdDateFrom)
      AND (@ProdDateTo IS NULL OR a.ProdDate <= @ProdDateTo)
      AND (@CustomerId IS NULL OR a.CustomerId = @CustomerId)
      AND (@CustomerName IS NULL OR c.CustomerName LIKE '%' + @CustomerName + '%')
      AND (@ItemId IS NULL OR a.ItemId = @ItemId)
      AND (@ItemName IS NULL OR i.ItemName LIKE '%' + @ItemName + '%')
      AND (@PlanQty IS NULL OR
           (@PlanQtyOperator = '=' AND a.PlanQty = @PlanQty) OR
           (@PlanQtyOperator = '>' AND a.PlanQty > @PlanQty) OR
           (@PlanQtyOperator = '>=' AND a.PlanQty >= @PlanQty) OR
           (@PlanQtyOperator = '<' AND a.PlanQty < @PlanQty) OR
           (@PlanQtyOperator = '<=' AND a.PlanQty <= @PlanQty))
      AND (@ResultQty IS NULL OR
           (@ResultQtyOperator = '=' AND a.ResultQty = @ResultQty) OR
           (@ResultQtyOperator = '>' AND a.ResultQty > @ResultQty) OR
           (@ResultQtyOperator = '>=' AND a.ResultQty >= @ResultQty) OR
           (@ResultQtyOperator = '<' AND a.ResultQty < @ResultQty) OR
           (@ResultQtyOperator = '<=' AND a.ResultQty <= @ResultQty))
      AND (@ResultValue IS NULL OR
           (@ResultValueOperator = '=' AND a.ResultValue = @ResultValue) OR
           (@ResultValueOperator = '>' AND a.ResultValue > @ResultValue) OR
           (@ResultValueOperator = '>=' AND a.ResultValue >= @ResultValue) OR
           (@ResultValueOperator = '<' AND a.ResultValue < @ResultValue) OR
           (@ResultValueOperator = '<=' AND a.ResultValue <= @ResultValue))
      AND (@Notes IS NULL OR a.Notes LIKE '%' + @Notes + '%')
      AND (@ProdStatusId IS NULL OR a.ProdStatusId = @ProdStatusId)
      AND (@ProdStatusName IS NULL OR s.ProdStatusName LIKE '%' + @ProdStatusName + '%')
      AND (@CreatedDate IS NULL OR CAST(a.CreatedDate AS DATE) = CAST(@CreatedDate AS DATE))
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@ModifiedDate IS NULL OR CAST(a.ModifiedDate AS DATE) = CAST(@ModifiedDate AS DATE))
      AND (@ModifiedBy IS NULL OR a.ModifiedBy LIKE '%' + @ModifiedBy + '%')
      AND (@CostPrice IS NULL OR
           (@CostPriceOperator = '=' AND CAST(IIF(a.[ResultQty] > 0, a.[ResultValue] / a.[ResultQty], 0) as decimal(18,2)) = @CostPrice) OR
           (@CostPriceOperator = '>' AND CAST(IIF(a.[ResultQty] > 0, a.[ResultValue] / a.[ResultQty], 0) as decimal(18,2)) > @CostPrice) OR
           (@CostPriceOperator = '>=' AND CAST(IIF(a.[ResultQty] > 0, a.[ResultValue] / a.[ResultQty], 0) as decimal(18,2)) >= @CostPrice) OR
           (@CostPriceOperator = '<' AND CAST(IIF(a.[ResultQty] > 0, a.[ResultValue] / a.[ResultQty], 0) as decimal(18,2)) < @CostPrice) OR
           (@CostPriceOperator = '<=' AND CAST(IIF(a.[ResultQty] > 0, a.[ResultValue] / a.[ResultQty], 0) as decimal(18,2)) <= @CostPrice));

    -- Main result set with formatting
    SELECT
        [RecId],
        [ProdId],
        [ProdDate],
        CONVERT(VARCHAR(11), [ProdDate], 106) as [ProdDateStr],
        [CustomerId],
        [CustomerName],
        [ItemId],
        [ItemName],
        [UnitName],
        ROUND([PlanQty], 2) as [PlanQty],
        ROUND([ResultQty], 2) as [ResultQty],
        ROUND([ResultValue], 2) as [ResultValue],
        ROUND([CostPrice], 2) as [CostPrice],
        [Notes],
        [ProdStatusId],
        [ProdStatusName] = CASE [ProdStatusId]
            WHEN 0 THEN CONCAT('<span class="badge bg-warning">', [ProdStatusName], '</span>')
            WHEN 1 THEN CONCAT('<span class="badge bg-success">', [ProdStatusName], '</span>')
            WHEN 2 THEN CONCAT('<span class="badge bg-primary">', [ProdStatusName], '</span>')
            WHEN 3 THEN CONCAT('<span class="badge bg-danger">', [ProdStatusName], '</span>')
            ELSE [ProdStatusName]
            END,
        [CreatedDate],
        [CreatedBy],
        [ModifiedDate],
        [ModifiedBy]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'ASC' THEN [RecId] END ASC,
        CASE WHEN @SortColumn = 'RecId' AND @SortOrder = 'DESC' THEN [RecId] END DESC,
        CASE WHEN @SortColumn = 'ProdId' AND @SortOrder = 'ASC' THEN [ProdId] END ASC,
        CASE WHEN @SortColumn = 'ProdId' AND @SortOrder = 'DESC' THEN [ProdId] END DESC,
        CASE WHEN @SortColumn = 'ProdDate' AND @SortOrder = 'ASC' THEN [ProdDate] END ASC,
        CASE WHEN @SortColumn = 'ProdDate' AND @SortOrder = 'DESC' THEN [ProdDate] END DESC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'ASC' THEN [CustomerName] END ASC,
        CASE WHEN @SortColumn = 'CustomerName' AND @SortOrder = 'DESC' THEN [CustomerName] END DESC,
        CASE WHEN @SortColumn = 'ItemName' AND @SortOrder = 'ASC' THEN [ItemName] END ASC,
        CASE WHEN @SortColumn = 'ItemName' AND @SortOrder = 'DESC' THEN [ItemName] END DESC,
        CASE WHEN @SortColumn = 'PlanQty' AND @SortOrder = 'ASC' THEN [PlanQty] END ASC,
        CASE WHEN @SortColumn = 'PlanQty' AND @SortOrder = 'DESC' THEN [PlanQty] END DESC,
        CASE WHEN @SortColumn = 'ResultQty' AND @SortOrder = 'ASC' THEN [ResultQty] END ASC,
        CASE WHEN @SortColumn = 'ResultQty' AND @SortOrder = 'DESC' THEN [ResultQty] END DESC,
        CASE WHEN @SortColumn = 'ResultValue' AND @SortOrder = 'ASC' THEN [ResultValue] END ASC,
        CASE WHEN @SortColumn = 'ResultValue' AND @SortOrder = 'DESC' THEN [ResultValue] END DESC,
        CASE WHEN @SortColumn = 'CostPrice' AND @SortOrder = 'ASC' THEN [CostPrice] END ASC,
        CASE WHEN @SortColumn = 'CostPrice' AND @SortOrder = 'DESC' THEN [CostPrice] END DESC,
        CASE WHEN @SortColumn = 'ProdStatusId' AND @SortOrder = 'ASC' THEN [ProdStatusId] END ASC,
        CASE WHEN @SortColumn = 'ProdStatusId' AND @SortOrder = 'DESC' THEN [ProdStatusId] END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'ASC' THEN [CreatedDate] END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortOrder = 'DESC' THEN [CreatedDate] END DESC,
        CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'ASC' THEN [ModifiedDate] END ASC,
        CASE WHEN @SortColumn = 'ModifiedDate' AND @SortOrder = 'DESC' THEN [ModifiedDate] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    -- Total count from same filtered dataset
    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

