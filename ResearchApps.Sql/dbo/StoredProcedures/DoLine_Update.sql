CREATE PROCEDURE [dbo].[DoLine_Update]
    @DoLineId int,
    @WhId int = NULL,
    @Qty decimal(18,2) = 0,
    @Notes nvarchar(100)='',
    @ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DoId nvarchar(20), @OldQty decimal(18,2), @DoDate datetime;
    DECLARE @ItemId int, @OldWhId int, @CoLineId int;
    DECLARE @QtyDiff decimal(18,2), @CostPrice decimal(18,2);
    DECLARE @Onhand decimal(18,2), @BufferStock decimal(18,2);
    DECLARE @WhChanged bit = 0;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @ResultMessage NVARCHAR(500);
    
    BEGIN TRY
        
        -- Validate input parameters
        IF @DoLineId IS NULL OR @DoLineId <= 0
        BEGIN
            RAISERROR('Invalid DoLineId parameter', 16, 1);
        END
        
        -- Validate quantity
        IF @Qty IS NULL OR @Qty <= 0
        BEGIN
            RAISERROR('Qty tidak boleh minus atau nol', 16, 1);
        END
        
        -- Get current line data
        SELECT @DoId = DoId, @OldQty = Qty,
               @ItemId = ItemId, @OldWhId = WhId, @CoLineId = CoLineId
        FROM DoLine
        WHERE DoLineId = @DoLineId;
        
        IF @DoId IS NULL
        BEGIN
            RAISERROR('Delivery order line not found', 16, 1);
        END
        
        -- If WhId not provided, use existing warehouse
        IF @WhId IS NULL OR @WhId <= 0
            BEGIN
                SET @WhId = @OldWhId;
            END

        -- Check if warehouse changed
        IF @WhId <> @OldWhId
            BEGIN
                SET @WhChanged = 1;
            END

        -- Get DO date for inventory transaction
        SELECT @DoDate = DoDate FROM Do WHERE DoId = @DoId;
        
        -- Check if DO is in draft status (can be edited)
        IF NOT EXISTS (SELECT 1 FROM Do WHERE DoId = @DoId AND DoStatusId = 0)
        BEGIN
            RAISERROR('Cannot update line - Delivery Order is not in Draft status', 16, 1);
        END
        
        -- Calculate quantity difference
        SET @QtyDiff = @Qty - @OldQty;
        
        -- Validate quantity against outstanding (including old qty)
        DECLARE @QtyOs decimal(18,2);
        SELECT @QtyOs = Qty - ISNULL((SELECT SUM(Qty) FROM DoLine WHERE CoLineId = @CoLineId AND DoLineId <> @DoLineId), 0)
        FROM CoLine WHERE CoLineId = @CoLineId;
        
        IF @Qty > @QtyOs
        BEGIN
            SET @ErrorMessage = 'Quantity exceeds outstanding amount. Max available: ' + CAST(@QtyOs AS NVARCHAR(20));
            RAISERROR(@ErrorMessage, 16, 1);
        END
        
        -- Check stock availability at the NEW warehouse
        IF @WhChanged = 1 OR @QtyDiff > 0
        BEGIN
            -- Get stock at NEW warehouse
            SELECT @Onhand = a.Qty, @BufferStock = i.BufferStock
            FROM InventSum a
                JOIN Item i ON i.ItemId = a.ItemId
            WHERE a.ItemId = @ItemId AND a.WhId = @WhId;
            
            -- For warehouse change, need full quantity available
            -- For quantity increase only, need the difference
            DECLARE @RequiredQty decimal(18,2);
            SET @RequiredQty = CASE WHEN @WhChanged = 1 THEN @Qty ELSE @QtyDiff END;

            IF ISNULL(@Onhand,0) < @RequiredQty
            BEGIN
                SET @ErrorMessage = 'Transaksi gagal, stock yg tersedia di warehouse hanya ' + CAST(ISNULL(@Onhand,0) as nvarchar);
                RAISERROR(@ErrorMessage, 16, 1);
            END
        END
        
        -- Update the delivery line
        UPDATE DoLine
        SET Qty = @Qty,
            WhId = @WhId,
            Notes = @Notes,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE DoLineId = @DoLineId;
        
        SET @CostPrice = 0; -- Cost price for DO out

        -- Handle inventory transactions
        IF @WhChanged = 1
        BEGIN
            -- Warehouse changed - delete old transaction and create new one

            -- Delete old inventory transaction at old warehouse
            DELETE FROM InventTrans
            WHERE RefType = 'Delivery Order'
              AND RefId = @DoLineId;

            -- Create new inventory transaction at new warehouse
            INSERT INTO InventTrans (RefType, RefId, TransDate, ItemId, WhId, Qty, Value, CreatedBy, CreatedDate)
            VALUES ('Delivery Order', @DoLineId, @DoDate, @ItemId, @WhId, -1 * @Qty, -1 * @Qty * @CostPrice, @ModifiedBy, GETDATE());

            SET @ResultMessage = 'DO line updated successfully (warehouse changed)';
        END
        ELSE IF @QtyDiff <> 0
            BEGIN
                -- Warehouse same, only quantity changed
            
            -- Update existing InventTrans record
            UPDATE InventTrans
            SET Qty = -1 * @Qty,
                Value = -1 * @Qty * @CostPrice,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = GETDATE()
            WHERE RefType = 'Delivery Order'
              AND RefId = @DoLineId;
            
            -- Check buffer stock warning if quantity increased
            IF @QtyDiff > 0
            BEGIN
                IF ISNULL(@Onhand,0) - @QtyDiff < @BufferStock
                BEGIN
                    SET @ResultMessage = 'Transaksi Berhasil. Informasi stock saat ini ' + 
                        CAST(ISNULL(@Onhand-@QtyDiff,0) as nvarchar) + ', kurang dari minimal stock!';
                END
            END
        END
        ELSE
        BEGIN
            -- Only notes changed, no inventory impact
            SET @ResultMessage = 'DO line updated successfully (notes only)';
        END
        
        -- Return result
        IF @ResultMessage IS NULL
            SET @ResultMessage = 'DO line updated successfully';
            
        SELECT @ResultMessage AS Message;
        
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END

GO

