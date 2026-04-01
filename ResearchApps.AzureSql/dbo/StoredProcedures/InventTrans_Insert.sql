CREATE PROCEDURE [dbo].[InventTrans_Insert]
@ItemId int,
@InventDimId int,
@TransDate datetime,
@RefType nvarchar(20),
@RefId int,
@RefNo nvarchar(20),
@Qty numeric(32,16),
@CostPrice numeric(32,16),
@CreatedBy nvarchar(20)

AS 
BEGIN
	
	--* InventTrans *--
	INSERT INTO [InventTrans]
	([ItemId],[InventDimId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
	VALUES
	(@ItemId, @InventDimId, @TransDate, @RefType, @RefId, @RefNo, @Qty, @Qty*ISNULL(@CostPrice,0)
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

	--* InventSum *--
	IF EXISTS (SELECT 1 FROM InventSum WHERE ItemId = @ItemId AND InventDimId = @InventDimId)
	BEGIN --InventSum sudah ada 
		UPDATE InventSum 
		SET Qty = Qty + @Qty
			, Value = Value + (@Qty * @CostPrice)
			, ModifiedBy = @CreatedBy
			, ModifiedDate = GETDATE()
		WHERE ItemId = @ItemId AND InventDimId = @InventDimId
	END
	ELSE
	BEGIN --InventSum belum ada 
		INSERT INTO InventSum ([ItemId],[InventDimId],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
		VALUES (@ItemId, @InventDimId, @Qty, (@Qty * @CostPrice), GETDATE(),@CreatedBy,GETDATE(),@CreatedBy)
	END
END
GO

