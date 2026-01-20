CREATE PROCEDURE [dbo].[PoLine_Insert]
/****** PPN 11% ******/
@RecId int, 
@PrLineId int,
@ItemId int, 
@DeliveryDate date,
@Qty numeric(32,16), 
@Price numeric(32,16), 
@Notes nvarchar(100), 
@CreatedBy nvarchar(20)
AS
BEGIN
	DECLARE @IsPpn bit, @PoId nvarchar(20), @ItemName nvarchar(100), @UnitId int
	DECLARE @SubTotal decimal(18,2), @Ppn decimal(18,2), @Total decimal(18,2)

	SELECT @PoId = PoId, @IsPpn = IsPpn FROM Po WHERE RecId = @RecId
	SELECT @ItemName = ItemName, @UnitId = UnitId FROM Item WHERE ItemId = @ItemId

	--* Po Line *--
	INSERT INTO [PoLine]
	([PoId], [PrLineId], [ItemId], [ItemName], [DeliveryDate], [Qty], [UnitId], [Price], [Ppn], [Notes]
	  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@PoId, @PrLineId, @ItemId, @ItemName, CAST(@DeliveryDate as DATE), @Qty, @UnitId, @Price, 0, @Notes
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	--* Po Header *--
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

	SELECT @PoId
END

GO

