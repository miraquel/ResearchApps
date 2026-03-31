CREATE PROCEDURE [dbo].[McLine_Delete]
    @McLineId int,
    @ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @McId nvarchar(20), @Qty decimal(18,2), @Onhand decimal(18,2), @ItemId int, @WhId int;

    BEGIN TRY
        --* Init *--
        SELECT @McId = a.McId
             , @Qty = Qty
             , @ItemId = ItemId
             , @WhId = WhId
        FROM McLine a JOIN Mc b ON b.McId = a.McId
        WHERE McLineId = @McLineId;

        --* cek stock *--
        IF @Qty > 0
            BEGIN
                SELECT @Onhand = Qty FROM InventSum WHERE ItemId = @ItemId AND WhId = @WhId;
                IF @Onhand < @Qty
                    BEGIN
                        SELECT '-1:::Transaksi gagal, stock yg tersedia hanya ' + cast(@Onhand as nvarchar) AS Result;
                        RETURN;
                    END
            END

        --* Mc Line *--
        DELETE FROM [McLine]
        WHERE McLineId = @McLineId;

        --* InventTrans *--
        DELETE FROM [InventTrans]
        WHERE [RefType] = 'Material Customer' AND [RefId] = cast(@McLineId as nvarchar);

        SELECT @McId AS Result;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO
