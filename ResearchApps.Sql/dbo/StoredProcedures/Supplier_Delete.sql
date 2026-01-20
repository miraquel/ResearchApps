CREATE PROCEDURE [dbo].[Supplier_Delete]
@SupplierId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [Supplier]
	WHERE SupplierId = @SupplierId
END

GO

