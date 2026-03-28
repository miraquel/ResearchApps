CREATE PROCEDURE [dbo].[DoLine_Delete]
@DoLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @DoId nvarchar(20), @Qty decimal(18,2), @CostPrice decimal(18,2), @Onhand decimal(18,2)
        , @ItemId int, @WhId int;

    BEGIN TRY
        --* Init *--
        SELECT @DoId = a.DoId
            , @Qty = Qty
            , @CostPrice = Price
            , @ItemId = ItemId
            , @WhId = WhId
        FROM DoLine a JOIN Do b ON b.DoId = a.DoId
        WHERE DoLineId = @DoLineId;

        IF @DoId IS NULL
        BEGIN
            THROW 50001, 'Delivery order line not found.', 1;
        END;

        --* cek stock *--
        IF @Qty < 0
        BEGIN
            SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND WhId = @WhId;
            IF @Onhand < -1 * @Qty
            BEGIN
                DECLARE @ErrorMessage nvarchar(200) =
                    'Transaksi gagal, stock yg tersedia hanya ' + CAST(ISNULL(@Onhand, 0) as nvarchar);
                THROW 50002, @ErrorMessage, 1;
            END
        END

        --* Do Line *--
        DELETE FROM [DoLine]
        WHERE DoLineId = @DoLineId;

        --* InventTrans *--
        DELETE FROM [InventTrans]
        WHERE [RefType] = 'Delivery Order' AND [RefId] = CAST(@DoLineId as nvarchar);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END

GO

