CREATE PROCEDURE [dbo].[PhpLine_Update]
@PhpLineId int, 
@ItemId int, 
@WhId int,
@Qty numeric(32,16) = 0, 
@Notes nvarchar(100)='', 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PhpId nvarchar(20)
	DECLARE @Price numeric(32,16) 

	SELECT @PhpId = PhpId FROM PhpLine WHERE PhpLineId = @PhpLineId
	SELECT @Price = Price FROM [PhpLine] WHERE PhpLineId = @PhpLineId

	--* Php Line *--
	UPDATE [PhpLine]
	SET [ItemId] = @ItemId
		, [WhId] = @WhId
		, [Qty] = @Qty
		, [Price] = @Price
		, [Notes] = @Notes
		, [ModifiedDate] = GETDATE()
		, [ModifiedBy] = @ModifiedBy
	WHERE PhpLineId = @PhpLineId

	--* InventTrans *--
	UPDATE [InventTrans]
	SET Qty = -1*@Qty
		, Value = -1*@Qty * @Price
		, WhId = @WhId
	WHERE [RefType] = 'Penyesuaian Stock' AND [RefId] = cast(@PhpLineId as nvarchar)

	SELECT @PhpId
END

GO

