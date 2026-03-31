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

		--* Ps Line *--
		INSERT INTO [PsLine]
		([PsId], [ItemId], [WhId], [Qty], [Price], [Notes]
		  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
		VALUES
		(@PsId, @ItemId, @WhId, @Qty, ISNULL(@CostPrice,0), @Notes
		,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

		SELECT @PsLineId = SCOPE_IDENTITY();

		--* InventTrans *--
		--INSERT INTO [InventTrans]
		--([ItemId],[WhId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
		--VALUES
		--(@ItemId, @WhId, @PsDate, 'Penyesuaian Stock', @PsLineId, @PsId, @Qty, @Qty*ISNULL(@CostPrice,0)
		--,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

		EXEC InventTrans_Insert @ItemId,@WhId,@LocationId,@PsDate,'Penyesuaian Stock',@PsLineId,@PsId
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

