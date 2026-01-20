CREATE PROCEDURE [dbo].[Do_Insert]
    @DoDate datetime,
    @Descr nvarchar(50)='',
    @CoId nvarchar(20),
    @RefId nvarchar(20),
    @Notes nvarchar(100)='',
    @DoStatusId int = 1,
    @CreatedBy nvarchar(20) = 'system',
    @CustomerId int = 0,
    @Dn nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DoId nvarchar(20), @PreDoId nvarchar(20), @MaxDoId nvarchar(20), @MaxnDoId int;
    DECLARE @IsPpn bit;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    
    BEGIN TRY
        -- Validate input parameters
        IF @DoDate IS NULL
        BEGIN
            RAISERROR('DoDate parameter is required', 16, 1);
            RETURN;
        END
        
        IF @CoId IS NULL OR LEN(LTRIM(RTRIM(@CoId))) = 0
        BEGIN
            RAISERROR('CoId parameter is required', 16, 1);
            RETURN;
        END
        
        IF @CustomerId IS NULL OR @CustomerId <= 0
        BEGIN
            RAISERROR('Invalid CustomerId parameter', 16, 1);
            RETURN;
        END
        
        IF @CreatedBy IS NULL OR LEN(LTRIM(RTRIM(@CreatedBy))) = 0
        BEGIN
            RAISERROR('CreatedBy parameter is required', 16, 1);
            RETURN;
        END
        
        -- Validate customer exists
        IF NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerId = @CustomerId)
        BEGIN
            RAISERROR('Customer not found', 16, 1);
            RETURN;
        END
        
        -- Validate CO exists
        IF NOT EXISTS (SELECT 1 FROM Co WHERE CoId = @CoId)
        BEGIN
            RAISERROR('Customer order not found', 16, 1);
            RETURN;
        END
        
        --* Cek apakah Inventory Lock? *--
        IF EXISTS (
            SELECT 1 FROM InventLock
            WHERE [Year] = YEAR(@DoDate) AND [Month] = MONTH(@DoDate) AND Lock = 1
        )
        BEGIN
            SELECT -1 as RecId, '' as DoId;
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        --* Nomor Do Baru *--
        SET @PreDoId = 'DO' + RIGHT(CAST(YEAR(GETDATE()) as nvarchar),2);
        SELECT @MaxDoId = MAX(DoId) FROM Do WHERE LEFT(DoId,4) = @PreDoId;
        SET @MaxnDoId = SUBSTRING(ISNULL(@MaxDoId,'0'),5,4);
        SET @DoId = @PreDoId + REPLICATE('0', 4 - LEN(@MaxnDoId+1)) + CAST(@MaxnDoId+1 AS varchar);
        
        --* Do Header *--
        INSERT INTO [Do]
        ([DoId], [DoDate], [Descr], CoId, RefId, Amount, [Notes], [CustomerId], Dn
        ,[DoStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
        VALUES
            (@DoId, CAST(@DoDate as DATE), @Descr, @CoId, @RefId, 0, @Notes, @CustomerId, @Dn
            ,1	--@DoStatusId
			, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
        
		SELECT SCOPE_IDENTITY() as RecId, @DoId as DoId;

		--* Mengisi Ref *--
		UPDATE [Do] 
		SET RefId = b.RefNo
		FROM [Do] a
		JOIN [Co] b ON b.CoId = a.CoId
		WHERE a.DoId = @DoId
        
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END

GO

