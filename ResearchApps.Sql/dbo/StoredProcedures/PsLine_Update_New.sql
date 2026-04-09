--EXEC [PsLine_Update_New] 8, 37100, 1, 1, 1, -5, 0, '', admin
CREATE PROCEDURE [dbo].[PsLine_Update_New]
@PsLineId int,
@ItemId int,
@InventDimId int,
@WhId int,
@LocationId int,
@Qty numeric(32,16),
@Price numeric(32,16),
@Notes nvarchar(100),
@ModifiedBy nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PsId nvarchar(20), @ItemId_Old int, @InventDimId_Old int	
	, @WhId_Old int, @LocationId_Old int, @Qty_Old numeric(32,16), @Price_Old numeric(32,16)
	, @Onhand numeric(32,16), @PsDate datetime, @CostPrice numeric(32,16), @Value numeric(32,16), @RecId int
	
	BEGIN TRY	
		--* Init *--
		SELECT @PsId = a.PsId 
			,@ItemId_Old = a.ItemId
			,@InventDimId_Old = a.InventDimId
			,@WhId_Old = b.WhId
			,@LocationId_Old = b.LocationId
			,@Qty_Old = a.Qty
			,@Price_Old = a.Price
		FROM PsLine a 
		LEFT JOIN InventDim b ON b.InventDimId = a.InventDimId
		WHERE a.PsLineId = @PsLineId;
		
		SELECT @RecId=RecId, @PsDate = PsDate 
		FROM Ps 
		WHERE PsId = @PsId;
		
		SELECT @CostPrice = CostPrice 
		FROM InventSum 
		WHERE ItemId = @ItemId;


		IF isnull(@CostPrice,0) = 0
			SELECT @CostPrice = CostPrice FROM Item WHERE ItemId = @ItemId;

		SET @Value = @Qty* @CostPrice

		--* InventDim *--
		IF EXISTS (SELECT InventDimId FROM InventDim WHERE WhId = @WhId AND LocationId = @LocationId)
		BEGIN --jika sudah ada, ambil InventDimId nya
			SELECT @InventDimId = InventDimId
			FROM InventDim WHERE WhId = @WhId AND LocationId = @LocationId
		END
		ELSE
		BEGIN --jika belum ada, buat InventDimId baru
			INSERT INTO InventDim (WhId, LocationId, CreatedDate, CreatedBy)
				VALUES (@WhId,@LocationId,GETDATE(),@ModifiedBy) 

			SET @InventDimId = SCOPE_IDENTITY()
		END

		--* cek  stock *--
		IF @Qty < 0
		BEGIN
			SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND InventDimId = @InventDimId;
			IF ISNULL(@Onhand,0) < -1*@Qty
			BEGIN
				SELECT '-1:::Transaksi gagal, stock yg tersedia hanya ' + cast(ISNULL(@Onhand,0) as nvarchar);
				RETURN;
			END
		END

		--* Ps Line *--
		UPDATE [PsLine]
		SET [ItemId] = @ItemId
			, [WhId] = @WhId
			, [Qty] = @Qty
			, [Price] = @Price
			, [Notes] = @Notes
			, [ModifiedDate] = GETDATE()
			, [ModifiedBy] = @ModifiedBy
		WHERE PsLineId = @PsLineId;

		--* InventDim *--
		IF EXISTS (SELECT InventDimId FROM InventDim WHERE WhId = @WhId AND LocationId = @LocationId)
		BEGIN --jika sudah ada, ambil InventDimId nya
			SELECT @InventDimId = InventDimId
			FROM InventDim WHERE WhId = @WhId AND LocationId = @LocationId
		END
		ELSE
		BEGIN --jika belum ada, buat InventDimId baru
			INSERT INTO InventDim (WhId, LocationId, CreatedDate, CreatedBy)
				VALUES (@WhId,@LocationId,GETDATE(),@ModifiedBy) 

			SET @InventDimId = SCOPE_IDENTITY()
		END

		--* InventTrans *--
		-- Delete dulu
		DELETE FROM [InventTrans]
		WHERE [RefType] = 'Penyesuaian Stock' AND [RefId] = @PsLineId;

		UPDATE InventSum 
		SET QTY = Qty - @Qty_Old
			, Value = Value - (@Qty_Old*@Price_Old)
			, ModifiedBy = @ModifiedBy
			, ModifiedDate = GETDATE()
		WHERE ItemId = @ItemId_Old and InventDimId = @InventDimId_Old

		-- Selanjutnya buat baru
		EXEC InventTrans_Insert @ItemId,@InventDimId,@PsDate,'Penyesuaian Stock',@PsLineId,@PsId
			,@Qty
			,@Value
			,@ModifiedBy
			
		--* Update Ps header Amount *--
		UPDATE [Ps]
		SET [Amount] = (SELECT ISNULL(SUM(ABS([Qty]) * [Price]), 0) FROM [PsLine] WHERE [PsId] = @PsId)
		WHERE [RecId] = @RecId;

		SELECT @PsId AS PsId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END
GO

