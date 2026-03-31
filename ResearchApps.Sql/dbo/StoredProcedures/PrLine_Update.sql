CREATE PROCEDURE [dbo].[PrLine_Update]
@PrLineId int,
@ItemId int,
@RequestDate date,
@Qty numeric(32,16) = 0,
@Price numeric(32,16) = 0,
@Notes nvarchar(100)='',
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PrId nvarchar(20), @ItemName nvarchar(100), @UnitId int, @Total decimal(18,2);

	BEGIN TRY
		SELECT @PrId = PrId FROM PrLine WHERE PrLineId = @PrLineId;
		SELECT @ItemName = ItemName, @UnitId = UnitId FROM Item WHERE ItemId = @ItemId;

		--* Pr Line *--
		UPDATE [PrLine]
		SET [ItemId] = @ItemId
			, [ItemName] = @ItemName
			, [RequestDate] = @RequestDate
			, [Qty] = @Qty
			, [UnitId] = @UnitId
			, [Price] = @Price
			, [Notes] = @Notes
			, [ModifiedDate] = GETDATE()
			, [ModifiedBy] = @ModifiedBy
		WHERE PrLineId = @PrLineId;

		--* Pr Header *--
		SELECT @Total = SUM(Qty * Price)
		FROM PrLine
		WHERE PrId = @PrId;

		UPDATE [Pr]
		SET Total = @Total
		WHERE PrId = @PrId;

		SELECT @PrId AS PrId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
