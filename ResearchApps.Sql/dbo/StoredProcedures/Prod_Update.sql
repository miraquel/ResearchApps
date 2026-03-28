CREATE PROCEDURE [dbo].[Prod_Update]
@RecId int,
@ProdDate datetime,
@CustomerId int,
@ItemId int,
@PlanQty numeric(32,16),
@Notes nvarchar(100)='',
@ProdStatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		UPDATE [Prod]
		SET ProdDate = @ProdDate
			, CustomerId = @CustomerId
			, ItemId = @ItemId
			, PlanQty = @PlanQty
			, Notes = @Notes
			, ProdStatusId = @ProdStatusId
			, ModifiedBy = @ModifiedBy
			, ModifiedDate = GETDATE()
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
