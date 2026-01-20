CREATE PROCEDURE [dbo].[GrLine_Update]
@GrLineId int, 
@PoLineId int, 
@Qty numeric(32,16) = 0, 
@Notes nvarchar(100)='', 
@WhId int, 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @GrId nvarchar(20), @PoId nvarchar(20), @ItemId int, @ItemName nvarchar(100), @DeliveryDate date, @UnitId int
	DECLARE @Price numeric(32,16), @SubTotal numeric(32,16), @Ppn numeric(32,16), @Total numeric(32,16)

	SELECT @GrId = GrId FROM GrLine WHERE GrLineId = @GrLineId

	SELECT @PoId = PoId, @ItemId = ItemId, @ItemName = ItemName, @DeliveryDate = DeliveryDate, @UnitId = UnitId 
		, @Price = Price, @Ppn = Ppn
	FROM PoLine WHERE PoLineId = @PoLineId

	--* Gr Line *--
	UPDATE [GrLine]
	SET [PoLineId] = @PoLineId
		, [PoId] = @PoId
		, [ItemId] = @ItemId
		, [ItemName] = @ItemName
		, [Qty] = @Qty
		, [UnitId] = @UnitId
		, [Price] = @Price
		, [Ppn] = @Ppn
		, [WhId] = @WhId
		, [Notes] = @Notes
		, [ModifiedDate] = GETDATE()
		, [ModifiedBy] = @ModifiedBy
	WHERE GrLineId = @GrLineId

	--* Gr Header *--
	SELECT @SubTotal = SUM(Qty * Price)
		, @Ppn = SUM(Qty * Ppn)
		, @Total = SUM(Qty * (Price + Ppn))
	FROM GrLine
	WHERE GrId = @GrId

	UPDATE [Gr]
	SET SubTotal = @SubTotal
		,Ppn = @Ppn
		,Total = @Total
	WHERE GrId = @GrId
	
	
	--* InventTrans *--
	UPDATE [InventTrans]
	SET Qty = @Qty
		, Value = @Qty * @Price
		, WhId = @WhId
	WHERE [RefType] = 'Goods Receipt' AND [RefId] = cast(@GrLineId as nvarchar)


	SELECT @GrId
END

GO

