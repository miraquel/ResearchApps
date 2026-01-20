CREATE PROCEDURE [dbo].[GrLine_Delete]
@GrLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @IsPpn bit, @GrId nvarchar(20), @ItemName nvarchar(100), @UnitId int
	DECLARE @SubTotal numeric(32,16), @Ppn numeric(32,16), @Total numeric(32,16)
	DECLARE @Qty numeric(32,16), @Onhand numeric(32,16), @ItemId int, @WhId int

	--* Init *--
	SELECT @GrId = GrId 
		, @Qty = Qty
		, @ItemId = ItemId
		, @WhId = WhId 	
	FROM GrLine WHERE GrLineId = @GrLineId

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

	--* Gr Line *--
	DELETE FROM [GrLine]
	WHERE GrLineId = @GrLineId


	--* Gr Header *--
	SELECT @SubTotal = SUM(Qty * Price)
		, @Ppn = SUM(Qty * Ppn)
		, @Total = SUM(Qty * (Price+Ppn))
	FROM GrLine
	WHERE GrId = @GrId

	UPDATE [Gr]
	SET SubTotal = isnull(@SubTotal,0)
		,Ppn = isnull(@Ppn,0)
		,Total = isnull(@Total,0)
	WHERE GrId = @GrId

	--* InventTrans *--
	DELETE FROM [InventTrans]
	WHERE [RefType] = 'Goods Receipt' AND [RefId] = cast(@GrLineId as nvarchar)


	SELECT @GrId
END

GO

