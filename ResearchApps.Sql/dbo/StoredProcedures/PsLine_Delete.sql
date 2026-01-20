CREATE PROCEDURE [dbo].[PsLine_Delete]
@PsLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PsId nvarchar(20), @Qty numeric(32,16), @Onhand numeric(32,16), @ItemId int, @WhId int

	--* Init *--
	SELECT @PsId = PsId
		, @Qty = Qty
		, @ItemId = ItemId
		, @WhId = WhId 
	FROM PsLine WHERE PsLineId = @PsLineId

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

	--* Ps Line *--
	DELETE FROM [PsLine]
	WHERE PsLineId = @PsLineId

	--* InventTrans *--
	DELETE FROM [InventTrans]
	WHERE [RefType] = 'Penyesuaian Stock' AND [RefId] = cast(@PsLineId as nvarchar)

	SELECT '1:::' + @PsId
END

GO

