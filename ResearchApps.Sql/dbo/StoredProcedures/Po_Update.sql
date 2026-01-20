CREATE PROCEDURE [dbo].[Po_Update]
/****** PPN 11% ******/
@RecId int, 
@SupplierId int, 
@Pic nvarchar(20)='', 
@PoDate date, 
@IsPpn bit,
@Notes nvarchar(100)='', 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PoId nvarchar(20), @SubTotal numeric(32,16), @Ppn numeric(32,16), @Total numeric(32,16)
	
	SELECT @PoId = PoId FROM Po WHERE RecId = @RecId

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
	END	

	UPDATE [Po]
	SET SupplierId = @SupplierId	
        , Pic = @Pic
		, PoDate = @PoDate
		, IsPpn = @IsPpn
		, Notes = @Notes
		, SubTotal = @SubTotal
		, Ppn = @Ppn
		, Total = @Total
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE PoId = @PoId
END

GO

