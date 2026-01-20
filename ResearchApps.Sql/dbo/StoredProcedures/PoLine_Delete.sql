CREATE PROCEDURE [dbo].[PoLine_Delete]
/****** PPN 11% ******/
@PoLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @IsPpn bit, @PoId nvarchar(20), @ItemName nvarchar(100), @UnitId int
	DECLARE @SubTotal numeric(32,16), @Ppn numeric(32,16), @Total numeric(32,16)

	SELECT @PoId = PoId FROM PoLine WHERE PoLineId = @PoLineId
	SELECT @IsPpn = IsPpn FROM Po WHERE PoId = @PoId

	--* Po Line *--
	DELETE FROM [PoLine]
	WHERE PoLineId = @PoLineId

	--* Po Header *--
	IF @IsPpn = 1
	BEGIN
		UPDATE [PoLine]
		SET Ppn = Price * 0.11
		WHERE PoId = @PoId

		SELECT @SubTotal = SUM(Qty * Price)
			, @Ppn = SUM(Qty * Price * 0.11)
			, @Total = SUM(Qty * Price * 1.11)
		FROM PoLine
		WHERE PoId = @PoId

		UPDATE [Po]
		SET SubTotal = isnull(@SubTotal,0)
			,Ppn = isnull(@Ppn,0)
			,Total = isnull(@Total,0)
		WHERE PoId = @PoId
	END
	ELSE 
	BEGIN
		UPDATE [PoLine]
		SET Ppn = 0
		WHERE PoId = @PoId

		SELECT @SubTotal = SUM(Qty * Price)
			, @Ppn = 0
			, @Total = SUM(Qty * Price)
		FROM PoLine
		WHERE PoId = @PoId

		UPDATE [Po]
		SET SubTotal = @SubTotal
			,Ppn = @Ppn
			,Total = @Total
		WHERE PoId = @PoId
	END

	SELECT @PoId
END

GO

