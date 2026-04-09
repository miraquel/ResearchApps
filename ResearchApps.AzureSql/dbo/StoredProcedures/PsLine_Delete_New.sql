--EXEC [PsLine_Delete_New] 7, 'admin'
CREATE PROCEDURE [dbo].[PsLine_Delete_New]
@PsLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PsId nvarchar(20), @Qty numeric(32,16), @Price numeric(32,16), @Onhand numeric(32,16), @ItemId int, @InventDimId int;

	BEGIN TRY
		--* Init *--
		SELECT @PsId = PsId
			, @Qty = Qty
			, @Price = Price
			, @ItemId = ItemId
			, @InventDimId = InventDimId
		FROM PsLine WHERE PsLineId = @PsLineId;

		--* cek  stock *--
		IF @Qty > 0
		BEGIN
			SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND InventDimId = @InventDimId;
			IF @Onhand < @Qty
			BEGIN
				SELECT 'Transaksi gagal, stock yg tersedia hanya ' + cast(@Onhand as nvarchar) AS Result;
				RETURN;
			END
		END

		--* Ps Line *--
		DELETE FROM [PsLine]
		WHERE PsLineId = @PsLineId;

		--* InventTrans *--
		DELETE FROM [InventTrans]
		WHERE [RefType] = 'Penyesuaian Stock' AND [RefId] = cast(@PsLineId as nvarchar);

		UPDATE InventSum 
		SET QTY = Qty - @Qty
			, Value = Value - (@Qty*@Price)
			, ModifiedBy = @ModifiedBy
			, ModifiedDate = GETDATE()
		WHERE ItemId = @ItemId and InventDimId = @InventDimId

		SELECT @PsId AS Result;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END
GO

