CREATE PROCEDURE [dbo].[BpbLine_Delete]
@BpbLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @BpbId nvarchar(20), @Qty numeric(32,16), @CostPrice numeric(32,16), @Onhand numeric(32,16)
		, @ItemId int, @WhId int, @ProdId nvarchar(20)

	--* Init *--
	SELECT @BpbId = a.BpbId 
		, @Qty = Qty
		, @CostPrice = Price
		, @ItemId = ItemId
		, @WhId = WhId 
		, @ProdId = RefId
	FROM BpbLine a JOIN Bpb b ON b.BpbId = a.BpbId
	WHERE BpbLineId = @BpbLineId

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

	--* Bpb Line *--
	DELETE FROM [BpbLine]
	WHERE BpbLineId = @BpbLineId

	--* InventTrans *--
	DELETE FROM [InventTrans]
	WHERE [RefType] = 'Pengambilan Barang' AND [RefId] = cast(@BpbLineId as nvarchar)
		
	--* Prod Result *--
	UPDATE Prod
	SET ResultValue = ResultValue - (@Qty * @CostPrice)
	WHERE ProdId = @ProdId

	SELECT '1:::' + @BpbId
END

GO

