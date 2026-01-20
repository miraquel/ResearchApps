CREATE PROCEDURE [dbo].[SupplierCbo]
AS
BEGIN
	SELECT a.[SupplierId]
      ,a.[SupplierName]
	  ,a.City
  FROM [Supplier] a
  WHERE a.StatusId = 1
END

GO

