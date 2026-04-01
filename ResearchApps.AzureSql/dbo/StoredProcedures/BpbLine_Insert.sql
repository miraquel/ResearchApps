CREATE PROCEDURE [dbo].[BpbLine_Insert]
    @RecId int,
    @ItemId int,
    @WhId int,
    @Qty numeric(32,16) = 0,
    @ProdId nvarchar(20),
    @Notes nvarchar(100) = '',
    @CreatedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @BpbId nvarchar(20), @BpbDate datetime;
    DECLARE @BpbLineId int, @CostPrice numeric(32,16), @OnHand numeric(32,16), @BufferStock numeric(32,16);
    DECLARE @InsertErrMsg nvarchar(2048);
    DECLARE @InventDimId int;
    DECLARE @NegQty numeric(32,16);
    DECLARE @Value numeric(32,16);

    BEGIN TRY
        --* Init *--
        SELECT @BpbId = BpbId, @BpbDate = BpbDate FROM Bpb WHERE RecId = @RecId;
        SELECT @CostPrice = CostPrice FROM InventSum WHERE ItemId = @ItemId;

        --* cek stock *--
        SELECT @OnHand = a.Qty, @BufferStock = i.BufferStock
        FROM InventSum a
                 JOIN Item i ON i.ItemId = a.ItemId
        WHERE a.ItemId = @ItemId AND a.WhId = @WhId;

        IF ISNULL(@OnHand, 0) < @Qty
            BEGIN
                SET @InsertErrMsg = N'Transaksi gagal, stock yg tersedia hanya ' + CAST(ISNULL(@OnHand, 0) AS nvarchar);
                THROW 50000, @InsertErrMsg, 1;
            END

        --* Bpb Line *--
        INSERT INTO [BpbLine]
        ([BpbId], [ItemId], [WhId], [Qty], [Price], ProdId, [Notes],
         [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
        VALUES
            (@BpbId, @ItemId, @WhId, @Qty, @CostPrice, @ProdId, @Notes,
             GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

        SELECT @BpbLineId = SCOPE_IDENTITY();

        --* InventDim *--
        IF EXISTS (SELECT InventDimId FROM InventDim WHERE WhId = @WhId AND LocationId = 1)
        BEGIN
            SELECT @InventDimId = InventDimId FROM InventDim WHERE WhId = @WhId AND LocationId = 1;
        END
        ELSE
        BEGIN
            INSERT INTO InventDim (WhId, LocationId, CreatedDate, CreatedBy)
                VALUES (@WhId, 1, GETDATE(), @CreatedBy);
            SET @InventDimId = SCOPE_IDENTITY();
        END

        --* InventTrans *--
        SET @NegQty = -1 * @Qty;
        SET @Value = @NegQty * ISNULL(@CostPrice, 0);
        EXEC InventTrans_Insert @ItemId, @InventDimId, @BpbDate, 'Pengambilan Barang', @BpbLineId, @BpbId, @NegQty, @Value, @CreatedBy;

        --* Prod Result *--
        UPDATE Prod
        SET ResultValue = ResultValue + (@Qty * @CostPrice)
        WHERE ProdId = @ProdId;

        IF ISNULL(@OnHand, 0) - @Qty < @BufferStock
            BEGIN
                SET @InsertErrMsg = 'Transaksi gagal. Informasi stock saat ini ' + CAST(ISNULL(@OnHand - @Qty, 0) AS nvarchar) + ', kurang dari minimal stock!';
                THROW 50000, @InsertErrMsg, 1;
            END

        SELECT @BpbLineId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

