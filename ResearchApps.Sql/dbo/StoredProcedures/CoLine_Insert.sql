CREATE PROCEDURE [dbo].[CoLine_Insert]
/****** PPN 11% ******/
    @RecId int,
    @ItemId int,
    @WantedDeliveryDate date,
    @Qty numeric(32,16) = 0,
    @Price numeric(32,16) = 0,
    @Notes nvarchar(100)='',
    @CreatedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IsPpn bit, @CoId nvarchar(20), @ItemName nvarchar(100), @UnitId int;
    DECLARE @SubTotal numeric(32,16), @Ppn numeric(32,16), @Total numeric(32,16);
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    
    BEGIN TRY
        
        -- Validate input parameters
        IF @RecId IS NULL OR @RecId <= 0
        BEGIN
            RAISERROR('Invalid RecId parameter', 16, 1);
            RETURN;
        END
        
        IF @ItemId IS NULL OR @ItemId <= 0
        BEGIN
            RAISERROR('Invalid ItemId parameter', 16, 1);
            RETURN;
        END
        
        IF @WantedDeliveryDate IS NULL
        BEGIN
            RAISERROR('WantedDeliveryDate parameter is required', 16, 1);
            RETURN;
        END
        
        IF @Qty IS NULL OR @Qty <= 0
        BEGIN
            RAISERROR('Quantity must be greater than zero', 16, 1);
            RETURN;
        END
        
        IF @Price IS NULL OR @Price < 0
        BEGIN
            RAISERROR('Invalid Price parameter', 16, 1);
            RETURN;
        END
        
        -- Get Co details
        SELECT @CoId = CoId, @IsPpn = IsPpn FROM Co WHERE RecId = @RecId;
        
        IF @CoId IS NULL
        BEGIN
            RAISERROR('Customer order not found', 16, 1);
            RETURN;
        END
        
        -- Get Item details
        SELECT @ItemName = ItemName, @UnitId = UnitId FROM Item WHERE ItemId = @ItemId;
        
        IF @ItemName IS NULL
        BEGIN
            RAISERROR('Item not found', 16, 1);
            RETURN;
        END
        
        --* Co Line *--
        INSERT INTO [CoLine]
        ([CoId], [ItemId], [ItemName], [WantedDeliveryDate], [Qty], [UnitId], [Price], [Ppn], [Notes]
        ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
        VALUES
            (@CoId, @ItemId, @ItemName, CAST(@WantedDeliveryDate as DATE), @Qty, @UnitId, @Price, 0, @Notes
            ,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
        
        --* Co Header *--
        IF @IsPpn = 1
        BEGIN
            UPDATE [CoLine]
            SET Ppn = Price * 0.11
            WHERE CoId = @CoId;
            
            SELECT @SubTotal = ISNULL(SUM(Qty * Price), 0),
                   @Ppn = ISNULL(SUM(Qty * Price * 0.11), 0),
                   @Total = ISNULL(SUM(Qty * Price * 1.11), 0)
            FROM CoLine
            WHERE CoId = @CoId;
            
            UPDATE [Co]
            SET SubTotal = @SubTotal,
                Ppn = @Ppn,
                Total = @Total,
                ModifiedDate = GETDATE()
            WHERE CoId = @CoId;
        END
        ELSE
        BEGIN
            UPDATE [CoLine]
            SET Ppn = 0
            WHERE CoId = @CoId;
            
            SELECT @SubTotal = ISNULL(SUM(Qty * Price), 0),
                   @Ppn = 0,
                   @Total = ISNULL(SUM(Qty * Price), 0)
            FROM CoLine
            WHERE CoId = @CoId;
            
            UPDATE [Co]
            SET SubTotal = @SubTotal,
                Ppn = @Ppn,
                Total = @Total,
                ModifiedDate = GETDATE()
            WHERE CoId = @CoId;
        END
        
        SELECT @CoId;
        
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END

GO

