--EXEC [PsLine_Insert_new] 6, 37100, 1, 1, 100, '', admin
CREATE PROCEDURE [dbo].[PsLine_Insert_new]
@RecId int,
@ItemId int,
@WhId int,
@LocationId int,
@Qty numeric(32,16) = 0,
@Notes nvarchar(100)='',
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PsId nvarchar(20), @PsDate datetime;
	DECLARE @PsLineId int, @CostPrice numeric(32,16), @Onhand numeric(32,16);
	DECLARE @Value numeric(32,16)
	DECLARE @InventDimId int

	BEGIN TRY
		--* Init *--
		SELECT @PsId = PsId, @PsDate = PsDate FROM Ps WHERE RecId = @RecId;
		SELECT @CostPrice = CostPrice FROM InventSum WHERE ItemId = @ItemId;
		IF isnull(@CostPrice,0) = 0
			SELECT @CostPrice = CostPrice FROM Item WHERE ItemId = @ItemId;

		SET @Value = @Qty* @CostPrice

		--* cek  stock *--
		IF @Qty < 0
		BEGIN
			SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND WhId = @WhId;
			IF ISNULL(@Onhand,0) < -1*@Qty
			BEGIN
				SELECT '-1:::Transaksi gagal, stock yg tersedia hanya ' + cast(ISNULL(@Onhand,0) as nvarchar);
				RETURN;
			END
		END

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

		--* Ps Line *--
		INSERT INTO [PsLine]
		([PsId], [ItemId], [WhId], [InventDimId], [Qty], [Price], [Notes]
		  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
		VALUES
		(@PsId, @ItemId, @WhId, @InventDimId, @Qty, ISNULL(@CostPrice,0), @Notes
		,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

		SELECT @PsLineId = SCOPE_IDENTITY();

		--* InventTrans *--
		EXEC InventTrans_Insert @ItemId,@InventDimId,@PsDate,'Penyesuaian Stock',@PsLineId,@PsId
			,@Qty
			,@Value
			,@CreatedBy

		--* Update Ps header Amount *--
		UPDATE [Ps]
		SET [Amount] = (SELECT ISNULL(SUM(ABS([Qty]) * [Price]), 0) FROM [PsLine] WHERE [PsId] = @PsId)
		WHERE [RecId] = @RecId;

		SELECT '1:::' + @PsId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END
GO

