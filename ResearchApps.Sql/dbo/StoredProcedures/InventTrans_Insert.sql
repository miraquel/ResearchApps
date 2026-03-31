CREATE PROCEDURE InventTrans_Insert
@ItemId int,
@WhId int,
@LocationId int,
@TransDate datetime,
@RefType nvarchar(20),
@RefId int,
@RefNo nvarchar(20),
@Qty numeric(32,16),
@CostPrice numeric(32,16),
@CreatedBy nvarchar(20)

AS 
BEGIN
	DECLARE @InventDimId int

	--* InventDim *--
	IF EXISTS (SELECT InventDimId FROM InventDim WHERE WhId = @WhId AND LocationId = @LocationId)
	BEGIN --jika sudah ada, ambil InventDimId nya
		SELECT @InventDimId = InventDimId
		FROM InventDim WHERE WhId = @WhId AND LocationId = @LocationId
	END
	ELSE
	BEGIN --jika belum ada, buat InventDimId baru
		INSERT INTO InventDim (WhId, LocationId, CreatedDate, CreatedBy)
			VALUES (@WhId,@LocationId,GETDATE(),@CreatedBy) 

		SET @InventDimId = SCOPE_IDENTITY()
	END

	--* InventTrans *--
	INSERT INTO [InventTrans]
	([ItemId],[WhId],[InventDimId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
	VALUES
	(@ItemId, @WhId, @InventDimId, @TransDate, @RefType, @RefId, @RefNo, @Qty, @Qty*ISNULL(@CostPrice,0)
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

		---- update CostPrice
		--SELECT @QtySum = SUM(Qty) FROM InventSum WHERE ItemId = @ItemId
		--IF @QtySum > 0
		--BEGIN
		--	SELECT @CostPrice = SUM(Value) / SUM(Qty) FROM InventSum WHERE ItemId = @ItemId
		--	UPDATE InventSum SET CostPrice = @CostPrice WHERE ItemId = @ItemId
		--END
		--ELSE
		--	UPDATE InventSum SET CostPrice = 0 WHERE ItemId = @ItemId
		---- /.update CostPrice
	END
	ELSE
	BEGIN --InventSum belum ada 
		INSERT INTO InventSum ([ItemId],[WhId], [InventDimId],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
		VALUES (@ItemId, @WhId, @InventDimId, @Qty, (@Qty * @CostPrice), GETDATE(),@CreatedBy,GETDATE(),@CreatedBy)

		---- update CostPrice
		--SELECT @QtySum = SUM(Qty) FROM InventSum WHERE ItemId = @ItemId
		--IF @QtySum > 0
		--BEGIN
		--	SELECT @CostPrice = SUM(Value) / SUM(Qty) FROM InventSum WHERE ItemId = @ItemId
		--	UPDATE InventSum SET CostPrice = @CostPrice WHERE ItemId = @ItemId
		--END
		--ELSE
		--	UPDATE InventSum SET CostPrice = 0 WHERE ItemId = @ItemId
		---- /.update CostPrice
	END
END
GO

