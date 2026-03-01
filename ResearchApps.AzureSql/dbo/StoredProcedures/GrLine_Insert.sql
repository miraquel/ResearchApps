CREATE PROCEDURE [dbo].[GrLine_Insert]
    @RecId int,
    @PoLineId int,
    @Qty numeric(32,16),
    @WhId int,
    @Notes nvarchar(MAX) = '',
    @CreatedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GrId nvarchar(20), @PoId nvarchar(20), @ItemId int, @ItemName nvarchar(100), @UnitId int, @GrDate datetime;
    DECLARE @Price numeric(32,16), @SubTotal numeric(32,16), @Ppn numeric(32,16), @Total numeric(32,16);
    DECLARE @GrLineId int;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        -- Validate warehouse
        IF @WhId <> 1 AND @WhId <> 2
            BEGIN
                RAISERROR('Please select warehouse RM or TL.', 16, 1);
            END

        -- Validate qty
        IF @Qty <= 0
            BEGIN
                RAISERROR('Qty cannot be negative or zero.', 16, 1);
            END

        -- Get GR header info
        SELECT @GrId = GrId, @GrDate = GrDate FROM Gr WHERE RecId = @RecId;

        IF @GrId IS NULL
            BEGIN
                RAISERROR('Goods Receipt not found.', 16, 1);
            END

        -- Get PO Line info
        SELECT @PoId = PoId, @ItemId = ItemId, @ItemName = ItemName, @UnitId = UnitId
             , @Price = Price, @Ppn = Ppn
        FROM PoLine WHERE PoLineId = @PoLineId;

        IF @PoId IS NULL
            BEGIN
                RAISERROR('PO Line not found.', 16, 1);
            END

        --* Insert Gr Line *--
        INSERT INTO [GrLine]
        ([GrId], [PoLineId], [PoId], [ItemId], [ItemName], [Qty], [UnitId], [Price], [Ppn], [WhId], [Notes]
        ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
        VALUES
            (@GrId, @PoLineId, @PoId, @ItemId, @ItemName, @Qty, @UnitId, @Price, @Ppn, @WhId, @Notes
            ,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

        SET @GrLineId = SCOPE_IDENTITY();

        --* Update Gr Header totals *--
        SELECT @SubTotal = SUM(Qty * Price)
             , @Ppn = SUM(Qty * Ppn)
             , @Total = SUM(Qty * (Price + Ppn))
        FROM GrLine
        WHERE GrId = @GrId;

        UPDATE [Gr]
        SET SubTotal = @SubTotal
          ,Ppn = @Ppn
          ,Total = @Total
        WHERE GrId = @GrId;

        --* Insert InventTrans *--
        INSERT INTO [InventTrans]
        ([ItemId],[WhId],[TransDate],[RefType],[RefId],[RefNo],[Qty],[Value],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
        VALUES
            (@ItemId, @WhId, @GrDate, 'Goods Receipt', @GrLineId, @GrId, @Qty, @Qty*@Price
            ,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

        SELECT @GrLineId as GrLineId;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END
GO

