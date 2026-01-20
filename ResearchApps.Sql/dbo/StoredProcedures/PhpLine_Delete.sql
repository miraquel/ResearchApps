CREATE PROCEDURE [dbo].[PhpLine_Delete]
@PhpLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PhpId nvarchar(20), @Qty decimal(18,2), @Onhand decimal(18,2), @ItemId int, @WhId int, @ProdId nvarchar(20)

	--* Init *--
	SELECT @PhpId = a.PhpId
		, @Qty = Qty
		, @ItemId = ItemId
		, @WhId = WhId 
		, @ProdId = ProdId
	FROM PhpLine a JOIN Php b ON b.PhpId = a.PhpId
	WHERE PhpLineId = @PhpLineId

	--* cek  stock *--
	IF @Qty > 0 
	BEGIN
		SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND WhId = @WhId
		IF @Onhand < @Qty
		BEGIN
			SELECT '-1:::Transaksi gagal, stock yg tersedia hanya ' + cast(@Onhand as nvarchar)
			RETURN
		END
	END

	--* Php Line *--
	DELETE FROM [PhpLine]
	WHERE PhpLineId = @PhpLineId

	--* InventTrans *--
	DELETE FROM [InventTrans]
	WHERE [RefType] = 'Hasil Produksi' AND [RefId] = cast(@PhpLineId as nvarchar)

	--* Prod Result *--
	UPDATE Prod
	SET ResultQty = ResultQty - @Qty
	WHERE ProdId = @ProdId


	SELECT @PhpId
END

GO

