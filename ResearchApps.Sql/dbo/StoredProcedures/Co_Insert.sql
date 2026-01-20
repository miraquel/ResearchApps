CREATE PROCEDURE [dbo].[Co_Insert]
    @CustomerId int,
    @CoDate datetime,
    @PoCustomer nvarchar(20)='',
    @RefNo nvarchar(20)='',
    @CoTypeId int,
    @SubTotal decimal(18,2) = 0,
    @Ppn decimal(18,2) = 0,
    @Total decimal(18,2) = 0,
    @Notes nvarchar(MAX)='',
    @CoStatusId int = 0,
    @CreatedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CoId nvarchar(20), @PreCoId nvarchar(20), @MaxCoId nvarchar(20), @MaxnCoId int;
    DECLARE @IsPpn bit;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    
    BEGIN TRY
        -- Validate input parameters
        IF @CustomerId IS NULL OR @CustomerId <= 0
        BEGIN
            RAISERROR('Invalid CustomerId parameter', 16, 1);
        END
        
        IF @CoDate IS NULL
        BEGIN
            RAISERROR('CoDate parameter is required', 16, 1);
        END
        
        IF @CoTypeId IS NULL OR @CoTypeId <= 0
        BEGIN
            RAISERROR('Invalid CoTypeId parameter', 16, 1);
        END
        
        IF @CreatedBy IS NULL OR LEN(LTRIM(RTRIM(@CreatedBy))) = 0
        BEGIN
            RAISERROR('CreatedBy parameter is required', 16, 1);
        END
        
        -- Validate customer exists
        IF NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerId = @CustomerId)
        BEGIN
            RAISERROR('Customer not found', 16, 1);
        END
        
        --* Cek nomor PO Customer jika ada yg sama *--
        IF LEN(LTRIM(RTRIM(@PoCustomer))) > 0 AND EXISTS (
            SELECT PoCustomer FROM [Co]
            WHERE PoCustomer = @PoCustomer AND CustomerId = @CustomerId
        )
        BEGIN
            SELECT -1 as RecId, -1 as CoId;
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        --* Nomor Co Baru *--
        SET @PreCoId = 'FNA' + RIGHT(CAST(YEAR(@CoDate) as nvarchar),2) + 'CO-';
        SELECT @MaxCoId = MAX(CoId) FROM Co WHERE LEFT(CoId,8) = @PreCoId;
        SET @MaxnCoId = SUBSTRING(ISNULL(@MaxCoId,'0'),9,4);
        SET @CoId = @PreCoId + REPLICATE('0', 4 - LEN(@MaxnCoId+1)) + CAST(@MaxnCoId+1 AS varchar);
        
        --* Ppn *--
        SELECT @IsPpn = IsPpn FROM Customer WHERE CustomerId = @CustomerId;
        
        --* Co Header *--
        INSERT INTO [Co]
        ([CoId], [CoDate], [CustomerId], [PoCustomer], [RefNo], CoTypeId, [IsPpn], [SubTotal], [Ppn], [Total], [Notes]
        ,[CoStatusId], [WfTransId], [Revision], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
        VALUES
            (@CoId, CAST(@CoDate as DATE), @CustomerId, @PoCustomer, @RefNo, @CoTypeId, @IsPpn, @SubTotal, @Ppn, @Total, @Notes
            ,@CoStatusId, 0, 0, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);
        
        SELECT SCOPE_IDENTITY() as RecId, @CoId as CoId;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END

GO

