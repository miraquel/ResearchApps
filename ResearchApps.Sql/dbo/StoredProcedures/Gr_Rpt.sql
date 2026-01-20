CREATE PROCEDURE [dbo].[Gr_Rpt]
@StartDate datetime = '1 Jan 2025',
@EndDate datetime = '31 Jan 2025'
AS
BEGIN
	SELECT a.[GrId] as [GR ID]
      ,a.[GrDate] as [GR DATE]
      ,a.[SupplierId] as [SUPPLIER ID]
      ,v.[SupplierName] as [SUPPLIER NAME]
      ,a.[RefNo] as [REF]
      ,a.[SubTotal] as [SUBTOTAL]
      ,a.[Ppn] as [PPN]
      ,a.[Total] as [TOTAL]
      ,a.[Notes] as [NOTES HD]
      ,s.[GrStatusName] as [STATUS]
      ,b.[PoId] as [PO]
      ,w.[WhName] as [WH]
      ,b.[ItemId] as [ITEM ID]
      ,b.[ItemName] as [ITEM NAME]
      ,b.[Qty] as [QTY]
      ,u.[UnitName] as [UNIT]
	  ,it.ItemTypeName as [KELOMPOK BRG]
	  ,id.ItemDeptName as [SUB TITLE BRG]
	  ,b.[Price] as [PRICE]
      ,b.[Ppn] as [PPN]
      ,b.[Notes] as [NOTES DT]
	FROM [dbo].[Gr] a
	JOIN [GrLine] b ON b.[GrId] = a.[GrId]
	JOIN [Supplier] v ON v.[SupplierId] = a.[SupplierId]
	JOIN [Wh] w ON w.[WhId] = b.[WhId]
	JOIN [Unit] u ON u.[UnitId] = b.[UnitId]
	JOIN [GrStatus] s ON s.[GrStatusId] = a.[GrStatusId]
	JOIN [Item] i ON i.ItemId = b.ItemId
    JOIN ItemType it ON it.ItemTypeId = i.ItemTypeId 
    JOIN ItemDept id ON id.ItemDeptId = i.ItemDeptId
	WHERE a.[GrDate] BETWEEN @StartDate AND @EndDate
	ORDER BY a.[GrDate] ASC

END

GO

