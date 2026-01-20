CREATE PROCEDURE [dbo].[DoLine_Delete]
@DoLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @DoId nvarchar(20), @Qty decimal(18,2), @CostPrice decimal(18,2), @Onhand decimal(18,2)
		, @ItemId int, @WhId int

	--* Init *--
	SELECT @DoId = a.DoId 
		, @Qty = Qty
		, @CostPrice = Price
		, @ItemId = ItemId
		, @WhId = WhId 
	FROM DoLine a JOIN Do b ON b.DoId = a.DoId
	WHERE DoLineId = @DoLineId

	--* cek  stock *--
	IF @Qty < 0 
	BEGIN
		SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND WhId = @WhId
		IF @Onhand < -1*@Qty
		BEGIN
			SELECT '-1:::Transaksi gagal, stock yg tersedia hanya ' + cast(@Onhand as nvarchar)
			RETURN
		END
	END

	--* Do Line *--
	DELETE FROM [DoLine]
	WHERE DoLineId = @DoLineId

	--* InventTrans *--
	DELETE FROM [InventTrans]
	WHERE [RefType] = 'Delivery Order' AND [RefId] = cast(@DoLineId as nvarchar)

	SELECT '1:::' + @DoId
END

GO

