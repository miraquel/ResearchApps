CREATE PROCEDURE [dbo].[PoLine_Update]
/****** PPN 11% ******/
@PoLineId int,
@ItemId int, 
@DeliveryDate date,
@Qty numeric(32,16), 
@Price numeric(32,16), 
@Notes nvarchar(100), 
@ModifiedBy nvarchar(20)
AS
BEGIN
	DECLARE @IsPpn bit, @PoId nvarchar(20), @ItemName nvarchar(100), @UnitId int
	DECLARE @SubTotal decimal(18,2), @Ppn decimal(18,2), @Total decimal(18,2)

	-- Get PoId and IsPpn from the line's PO
	SELECT @PoId = PoId FROM PoLine WHERE PoLineId = @PoLineId
	SELECT @IsPpn = IsPpn FROM Po WHERE PoId = @PoId
	SELECT @ItemName = ItemName, @UnitId = UnitId FROM Item WHERE ItemId = @ItemId

	--* Update Po Line *--
	UPDATE [PoLine]
	SET [ItemId] = @ItemId
		,[ItemName] = @ItemName
		,[DeliveryDate] = CAST(@DeliveryDate as DATE)
		,[Qty] = @Qty
		,[UnitId] = @UnitId
		,[Price] = @Price
		,[Notes] = @Notes
		,[ModifiedDate] = GETDATE()
		,[ModifiedBy] = @ModifiedBy
	WHERE [PoLineId] = @PoLineId

	--* Recalculate Po Header *--
	IF @IsPpn = 1
	BEGIN
		UPDATE [PoLine]
		SET Ppn = Price * 0.11
		WHERE PoId = @PoId

		SELECT @SubTotal = ISNULL(SUM(Qty * Price), 0)
			, @Ppn = ISNULL(SUM(Qty * Price * 0.11), 0)
			, @Total = ISNULL(SUM(Qty * Price * 1.11), 0)
		FROM PoLine
		WHERE PoId = @PoId

		UPDATE [Po]
		SET SubTotal = @SubTotal
			,Ppn = @Ppn
			,Total = @Total
		WHERE PoId = @PoId
	END
	ELSE 
	BEGIN
		UPDATE [PoLine]
		SET Ppn = 0
		WHERE PoId = @PoId

		SELECT @SubTotal = ISNULL(SUM(Qty * Price), 0)
			, @Ppn = 0
			, @Total = ISNULL(SUM(Qty * Price), 0)
		FROM PoLine
		WHERE PoId = @PoId

		UPDATE [Po]
		SET SubTotal = @SubTotal
			,Ppn = @Ppn
			,Total = @Total
		WHERE PoId = @PoId
	END
END

GO

