CREATE PROCEDURE [dbo].[PhpLine_Insert]
@RecId int, 
@ItemId int, 
@WhId int,
@Qty numeric(32,16) = 0,
@ProdId nvarchar(20),
@Notes nvarchar(100)='', 
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	--validasi Warehouse
	IF @WhId = 1 
	BEGIN
		SELECT '-1:::Silahkan pilih warehouse selain TL.'
		RETURN		
	END

	--validasi Item
	IF EXISTS(SELECT 1 FROM Item WHERE ItemId=@ItemId AND StatusId<>1)
	BEGIN
		SELECT '-1:::Transaksi gagal, barang yang dipilih sudah Non Aktif.'
		RETURN		
	END

	DECLARE @PhpId nvarchar(20), @PhpDate datetime
	DECLARE @PhpLineId int, @CostPrice decimal(18,2) 

	--* Init *--
	SELECT @PhpId = PhpId, @PhpDate = PhpDate FROM Php WHERE RecId = @RecId
	SET @CostPrice = 0

	--* cek qty tidak boleh minus, jika salah input maka dihapus lalu input lagi *--
	IF @Qty <= 0 
	BEGIN
		SELECT '-1:::Qty tidak boleh minus atau nol.'
		RETURN		
	END

	--* Php Line *--
	INSERT INTO [PhpLine]
	([PhpId], [ItemId], [WhId], [Qty], [Price], [ProdId], [Notes]
	  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@PhpId, @ItemId, @WhId, @Qty, @CostPrice, @ProdId, @Notes
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT @PhpLineId = SCOPE_IDENTITY()

	--* InventTrans *--
	INSERT INTO [InventTrans]
	([ItemId],[WhId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
	VALUES
	(@ItemId, @WhId, @PhpDate, 'Hasil Produksi', @PhpLineId, @PhpId, @Qty, (@Qty*@CostPrice)
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	--* Prod Result *--
	UPDATE Prod
	SET ResultQty = ResultQty + @Qty
	WHERE ProdId = @ProdId
		
	SELECT @PhpId
END

GO

