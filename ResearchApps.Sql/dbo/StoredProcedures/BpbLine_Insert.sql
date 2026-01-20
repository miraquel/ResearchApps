CREATE PROCEDURE [dbo].[BpbLine_Insert]
@RecId int, 
@ItemId int, 
@WhId int,
@Qty numeric(32,16) = 0,
@ProdId nvarchar(20),
@Notes nvarchar(100)='', 
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @BpbId nvarchar(20), @BpbDate datetime
	DECLARE @BpbLineId int, @CostPrice decimal(18,2), @Onhand decimal(18,2), @BufferStock decimal(18,2)

	--* Init *--
	SELECT @BpbId = BpbId, @BpbDate = BpbDate FROM Bpb WHERE RecId = @RecId
	SELECT @CostPrice = CostPrice FROM InventSum WHERE ItemId = @ItemId

	--* cek  stock *--
	SELECT @Onhand = a.Qty, @BufferStock = i.BufferStock 
	FROM InventSum a
	JOIN Item i ON i.ItemId = a.ItemId
	WHERE a.ItemId = @ItemId AND a.WhId = @WhId
	IF ISNULL(@Onhand,0) < @Qty
	BEGIN
		SELECT '-1:::Transaksi gagal, stock yg tersedia hanya ' + cast(ISNULL(@Onhand,0) as nvarchar)
		RETURN
	END	

	--* Bpb Line *--
	INSERT INTO [BpbLine]
	([BpbId], [ItemId], [WhId], [Qty], [Price], ProdId, [Notes]
	  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@BpbId, @ItemId, @WhId, @Qty, @CostPrice, @ProdId, @Notes
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT @BpbLineId = SCOPE_IDENTITY()

	--* InventTrans *--
	INSERT INTO [InventTrans]
	([ItemId],[WhId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
	VALUES
	(@ItemId, @WhId, @BpbDate, 'Pengambilan Barang', @BpbLineId, @BpbId, -1*@Qty, -1*@Qty*@CostPrice
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	--* Prod Result *--
	UPDATE Prod
	SET ResultValue = ResultValue + (@Qty * @CostPrice)
	WHERE ProdId = @ProdId

	IF ISNULL(@Onhand,0)-@Qty < @BufferStock 
		SELECT '-1:::Transaksi Berhasil. Informasi stock saat ini ' + cast(ISNULL(@Onhand-@Qty,0) as nvarchar) + ', kurang dari minimal stock!' 
	ELSE
		SELECT '1:::' + @BpbId
END

GO

