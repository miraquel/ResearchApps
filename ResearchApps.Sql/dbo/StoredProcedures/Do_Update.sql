CREATE PROCEDURE [dbo].[Do_Update]
    @RecId int,
    @DoDate datetime,
    @Descr nvarchar(50),
    @CustomerId int,
    @Dn nvarchar(20),
    @CoId nvarchar(20),
    @RefId nvarchar(20),
    @Notes nvarchar(100),
    @ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @DoId nvarchar(20);
    DECLARE @OldCoId nvarchar(20);
    
    BEGIN TRY
        
        -- Validate input parameters
        IF @RecId IS NULL OR @RecId <= 0
        BEGIN
            RAISERROR('Invalid RecId parameter', 16, 1);
            RETURN;
        END
        
        IF @DoDate IS NULL
        BEGIN
            RAISERROR('DoDate parameter is required', 16, 1);
            RETURN;
        END
        
        IF @CustomerId IS NULL OR @CustomerId <= 0
        BEGIN
            RAISERROR('Invalid CustomerId parameter', 16, 1);
            RETURN;
        END
        
        IF @ModifiedBy IS NULL OR LEN(LTRIM(RTRIM(@ModifiedBy))) = 0
        BEGIN
            RAISERROR('ModifiedBy parameter is required', 16, 1);
            RETURN;
        END
        
        -- Validate DO exists and get current CoId
        SELECT @DoId = DoId, @OldCoId = CoId 
        FROM Do 
        WHERE RecId = @RecId;
        
        IF @DoId IS NULL
        BEGIN
            RAISERROR('Delivery order not found', 16, 1);
            RETURN;
        END
        
        -- Validate customer exists
        IF NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerId = @CustomerId)
        BEGIN
            RAISERROR('Customer not found', 16, 1);
            RETURN;
        END
        
        --* If CoId changes, reverse all DoLine transactions *--
        IF @OldCoId <> @CoId OR (@OldCoId IS NULL AND @CoId IS NOT NULL) OR (@OldCoId IS NOT NULL AND @CoId IS NULL)
        BEGIN
            -- Reverse InventTrans entries (delete inventory transactions for this DO)
            DELETE FROM [InventTrans]
            WHERE RefType = 'Delivery Order'
              AND RefNo = @DoId;
            
            -- Delete all DoLine records for this DO
            DELETE FROM [DoLine]
            WHERE DoId = @DoId;
            
            -- Reset DO Amount to 0 since all lines are removed
            UPDATE [Do]
            SET Amount = 0
            WHERE DoId = @DoId;
        END
        
        --* Do Header *--
        UPDATE [Do]
        SET [DoDate] = @DoDate,
            [Descr] = @Descr,
            [CustomerId] = @CustomerId,
            Dn = @Dn,
            CoId = @CoId,
            RefId = @RefId,
            [Notes] = @Notes,
            [ModifiedBy] = @ModifiedBy,
            [ModifiedDate] = GETDATE()
        WHERE RecId = @RecId;
        
        SELECT @RecId as RecId;
        
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END

GO

