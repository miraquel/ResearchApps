CREATE PROCEDURE [dbo].[PsLine_Update]
@PsLineId int, 
@ItemId int, 
@WhId int,
@Qty numeric(32,16), 
@Notes nvarchar(100), 
@ModifiedBy nvarchar(20)
AS
BEGIN
	DECLARE @PsId nvarchar(20)
	DECLARE @Price numeric(32,16) 

	SELECT @PsId = PsId FROM PsLine WHERE PsLineId = @PsLineId
	SELECT @Price = Price FROM [PsLine] WHERE PsLineId = @PsLineId

	--* Ps Line *--
	UPDATE [PsLine]
	SET [ItemId] = @ItemId
		, [WhId] = @WhId
		, [Qty] = @Qty
		, [Price] = @Price
		, [Notes] = @Notes
		, [ModifiedDate] = GETDATE()
		, [ModifiedBy] = @ModifiedBy
	WHERE PsLineId = @PsLineId

	--* InventTrans *--
	UPDATE [InventTrans]
	SET Qty = -1*@Qty
		, Value = -1*@Qty * @Price
		, WhId = @WhId
	WHERE [RefType] = 'Penyesuaian Stock' AND [RefId] = cast(@PsLineId as nvarchar)

	SELECT @PsId
END

GO

