CREATE PROCEDURE [dbo].[BpbLine_Update]
@BpbLineId int, 
@ItemId int, 
@WhId int,
@Qty numeric(32,16) = 0, 
@Notes nvarchar(100)='', 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @BpbId nvarchar(20)
	DECLARE @Price decimal(18,2) 

	SELECT @BpbId = BpbId FROM BpbLine WHERE BpbLineId = @BpbLineId
	SELECT @Price = Price FROM [BpbLine] WHERE BpbLineId = @BpbLineId

	--* Bpb Line *--
	UPDATE [BpbLine]
	SET [ItemId] = @ItemId
		, [WhId] = @WhId
		, [Qty] = @Qty
		, [Price] = @Price
		, [Notes] = @Notes
		, [ModifiedDate] = GETDATE()
		, [ModifiedBy] = @ModifiedBy
	WHERE BpbLineId = @BpbLineId

	--* InventTrans *--
	UPDATE [InventTrans]
	SET Qty = -1*@Qty
		, Value = -1*@Qty * @Price
		, WhId = @WhId
	WHERE [RefType] = 'Pengambilan Barang' AND [RefId] = cast(@BpbLineId as nvarchar)

	SELECT @BpbId
END

GO

