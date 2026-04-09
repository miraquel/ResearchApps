CREATE PROCEDURE [dbo].[PsLine_Delete]
@PsLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PsId nvarchar(20), @Qty numeric(32,16), @Onhand numeric(32,16), @ItemId int, @WhId int;
	DECLARE @InsertErrMsg nvarchar(2048);

	BEGIN TRY
		--* Init *--
		SELECT @PsId = PsId
			, @Qty = Qty
			, @ItemId = ItemId
			, @WhId = WhId
		FROM PsLine WHERE PsLineId = @PsLineId;

		--* cek  stock *--
		IF @Qty > 0
		BEGIN
			SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND WhId = @WhId;
			IF @Onhand < @Qty
			BEGIN
				SET @InsertErrMsg = 'Stock tidak mencukupi untuk melakukan transaksi ini,' + ' stock yang tersedia hanya ' + cast(@Onhand as nvarchar);
			    THROW 50000, @InsertErrMsg, 1;
			END
		END

		--* Ps Line *--
		DELETE FROM [PsLine]
		WHERE PsLineId = @PsLineId;

		--* InventTrans *--
		DELETE FROM [InventTrans]
		WHERE [RefType] = 'Penyesuaian Stock' AND [RefId] = cast(@PsLineId as nvarchar);

		SELECT @PsId AS Result;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END
GO
