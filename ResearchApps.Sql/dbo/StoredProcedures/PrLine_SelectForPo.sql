CREATE PROCEDURE [dbo].[PrLine_SelectForPo]
@PoRecId int,
@PageNumber int = 1,
@PageSize int = 10,
@PrId nvarchar(20) = NULL,
@ItemName nvarchar(100) = NULL,
@DateFrom date = NULL
AS
BEGIN
	DECLARE @Offset int = (@PageNumber - 1) * @PageSize
	DECLARE @CurrentPoId nvarchar(20) = (SELECT PoId FROM Po WHERE RecId = @PoRecId)
	
	-- Get approved PR lines with outstanding quantities for this supplier
	-- Supports partial fulfillment with filters and pagination
	SELECT 
		pl.[PrLineId],
		pl.[PrId] + ' - ' + pl.[ItemName] + ' (Req: ' + CAST(pl.Qty AS VARCHAR) + ', Outstanding: ' + CAST(pl.Qty - ISNULL(po_qty.QtyPo, 0) AS VARCHAR) + ')' as [PrLineName],
		pl.[PrId],
		pl.[ItemId],
		pl.[ItemName],
		pl.[Qty] as [RequestedQty],
		ISNULL(po_qty.QtyPo, 0) as [OrderedQty],
		pl.[Qty] - ISNULL(po_qty.QtyPo, 0) as [OutstandingQty],
		pl.[Price],
		pl.[UnitId],
        ISNULL(u.UnitName, '') as [UnitName],
		pl.[RequestDate],
		CONVERT(VARCHAR(11), pl.[RequestDate], 106) as [RequestDateStr],
		COUNT(*) OVER() as TotalCount
	FROM [PrLine] pl
	INNER JOIN [Pr] pr ON pr.PrId = pl.PrId
    LEFT JOIN [Unit] u ON u.UnitId = pl.UnitId
	LEFT JOIN (
		-- Calculate total quantity already ordered for each PR line
		SELECT PrLineId, SUM(Qty) as QtyPo
		FROM PoLine
		GROUP BY PrLineId
	) po_qty ON po_qty.PrLineId = pl.PrLineId
	-- Exclude lines already in current PO (optimized with LEFT JOIN instead of NOT IN)
	LEFT JOIN PoLine current_po ON current_po.PrLineId = pl.PrLineId AND current_po.PoId = @CurrentPoId
	WHERE pr.PrStatusId = 1 -- Approved PRs only (status 1 = Approved)
		AND pl.Qty - ISNULL(po_qty.QtyPo, 0) > 0 -- Only show lines with outstanding quantity
		AND (@PrId IS NULL OR pl.PrId LIKE '%' + @PrId + '%') -- Filter by PR ID
		AND (@ItemName IS NULL OR pl.ItemName LIKE '%' + @ItemName + '%') -- Filter by Item Name
		AND (@DateFrom IS NULL OR pl.RequestDate >= @DateFrom) -- Filter by Request Date
		AND current_po.PrLineId IS NULL -- Exclude lines already in current PO
	ORDER BY pl.PrId, pl.PrLineId
	OFFSET @Offset ROWS
	FETCH NEXT @PageSize ROWS ONLY
END

GO
