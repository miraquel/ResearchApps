CREATE PROCEDURE [dbo].[DoLine_Insert]
    @RecId int,
    @ItemId int,
    @WhId int,
    @Qty numeric(32,16) = 0,
    @Notes nvarchar(100)='',
    @CreatedBy nvarchar(20) = 'system',
    @CoLineId int,
    @CoId nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DoId nvarchar(20), @DoDate datetime, @CustomerIdHd int;
    DECLARE @DoLineId int, @CostPrice numeric(32,16), @Onhand numeric(32,16), @BufferStock numeric(32,16);
    DECLARE @SalesPrice numeric(32,16);
    DECLARE @QtyOs numeric(32,16);
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @ResultMessage NVARCHAR(500);
    
    BEGIN TRY
        
        -- Validate input parameters
        IF @RecId IS NULL OR @RecId <= 0
        BEGIN
            RAISERROR('Invalid RecId parameter', 16, 1);
        END
        
        IF @ItemId IS NULL OR @ItemId <= 0
        BEGIN
            RAISERROR('Invalid ItemId parameter', 16, 1);
        END
        
        IF @WhId IS NULL OR @WhId <= 0
        BEGIN
            RAISERROR('Invalid WhId parameter', 16, 1);
        END
        
        IF @CoLineId IS NULL OR @CoLineId <= 0
        BEGIN
            RAISERROR('Invalid CoLineId parameter', 16, 1);
        END
        
        -- Validate quantity
        IF @Qty IS NULL OR @Qty <= 0
        BEGIN
            RAISERROR('Qty tidak boleh minus atau nol', 16, 1);
        END
        
        --* Init *--
        SELECT @DoId = DoId, @DoDate = DoDate, @CustomerIdHd = CustomerId 
        FROM Do 
        WHERE RecId = @RecId;
        
        IF @DoId IS NULL
        BEGIN
            RAISERROR('Delivery order not found', 16, 1);
        END
        
        -- Validate item exists
        IF NOT EXISTS (SELECT 1 FROM Item WHERE ItemId = @ItemId)
        BEGIN
            RAISERROR('Item not found', 16, 1);
        END
        
        -- Validate warehouse exists
        IF NOT EXISTS (SELECT 1 FROM Wh WHERE WhId = @WhId)
        BEGIN
            RAISERROR('Warehouse not found', 16, 1);
        END
        
        -- Get price from CO line
        SET @CostPrice = 0;
        SELECT @SalesPrice = Price FROM CoLine WHERE CoLineId = @CoLineId;
        
        IF @SalesPrice IS NULL
        BEGIN
            RAISERROR('Customer order line not found', 16, 1);
        END
        
        --* cek qty tidak boleh lebih dari Qty CO OS *--
        SELECT @QtyOs = (b.Qty - ISNULL(c.QtyDo,0))
        FROM Co a
            JOIN CoLine b ON b.CoId = a.CoId
            LEFT JOIN
                (
                    SELECT b.CoLineId, SUM(Qty) as QtyDo
                    FROM Do a 
                    JOIN DoLine b ON b.DoId = a.DoId
                    WHERE a.DoStatusId <> 3
                    GROUP BY b.CoLineId
                ) c ON c.CoLineId = b.CoLineId
        WHERE a.CoStatusId = 1
          AND b.CoLineId = @CoLineId
          AND b.Qty - ISNULL(c.QtyDo,0) > 0;
        
        IF ISNULL(@QtyOs, 0) < @Qty
        BEGIN
            RAISERROR('Qty tidak boleh melebihi Qty Outstanding CO', 16, 1);
        END
        
        --* cek stock *--
        SELECT @Onhand = a.Qty, @BufferStock = i.BufferStock
        FROM InventSum a
            JOIN Item i ON i.ItemId = a.ItemId
        WHERE a.ItemId = @ItemId AND a.WhId = @WhId;
        
        IF ISNULL(@Onhand,0) < @Qty
        BEGIN
            SET @ErrorMessage = 'Transaksi gagal, stock yg tersedia hanya ' + CAST(ISNULL(@Onhand,0) as nvarchar);
            RAISERROR(@ErrorMessage, 16, 1);
        END
        
        --* Do Line *--
        INSERT INTO [DoLine]
        ([DoId], [ItemId], [WhId], [Qty], [Price], [CustomerId], [Notes]
        ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [CoLineId], [CoId])
        VALUES
            (@DoId, @ItemId, @WhId, @Qty, @SalesPrice, @CustomerIdHd, @Notes
            ,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, @CoLineId, @CoId);
        
        SET @DoLineId = SCOPE_IDENTITY();
        
        --* InventTrans *--
        INSERT INTO [InventTrans]
        ([ItemId],[WhId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
        VALUES
            (@ItemId, @WhId, @DoDate, 'Delivery Order', @DoLineId, @DoId, -1*@Qty, (-1*@Qty*@CostPrice)
            ,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
        
        -- Check buffer stock warning
        IF ISNULL(@Onhand,0)-@Qty < @BufferStock
        BEGIN
            SET @ResultMessage = 'Transaksi Berhasil. Informasi stock saat ini ' + 
                CAST(ISNULL(@Onhand-@Qty,0) as nvarchar) + ', kurang dari minimal stock!';
        END
        ELSE
        BEGIN
            SET @ResultMessage = @DoId;
        END
        
        SELECT @ResultMessage;
        
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END

GO

