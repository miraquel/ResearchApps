CREATE PROCEDURE [dbo].[Gr_Insert]
    @SupplierId int,
    @GrDate datetime,
    @RefNo nvarchar(100) = '',
    @SubTotal numeric(32,16) = 0,
    @Ppn numeric(32,16) = 0,
    @Total numeric(32,16) = 0,
    @Notes nvarchar(100) = '',
    @GrStatusId int = 1,
    @CreatedBy nvarchar(20) = 'system'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GrId nvarchar(20), @PreGrId nvarchar(20), @MaxGrId nvarchar(20), @MaxnGrId int;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        -- Validate input parameters
        IF @SupplierId IS NULL OR @SupplierId <= 0
            BEGIN
                RAISERROR('Invalid SupplierId parameter', 16, 1);
            END

        IF @GrDate IS NULL
            BEGIN
                RAISERROR('GrDate parameter is required', 16, 1);
            END

        IF @CreatedBy IS NULL OR LEN(LTRIM(RTRIM(@CreatedBy))) = 0
            BEGIN
                RAISERROR('CreatedBy parameter is required', 16, 1);
            END

        -- Validate supplier exists
        IF NOT EXISTS (SELECT 1 FROM Supplier WHERE SupplierId = @SupplierId)
            BEGIN
                RAISERROR('Supplier not found', 16, 1);
            END

        --* Cek apakah Inventory Lock? *--
        IF EXISTS (
            SELECT * FROM InventLock
            WHERE [Year] = YEAR(@GrDate) AND [Month] = MONTH(@GrDate) AND Lock = 1
        )
            BEGIN
                DECLARE @LockYear INT = YEAR(@GrDate);
                DECLARE @LockMonth INT = MONTH(@GrDate);
                RAISERROR('Inventory is locked for year %d month %d. Cannot create Goods Receipt.', 16, 1, @LockYear, @LockMonth);
                RETURN;
            END

        --* Nomor GR Baru *--
        SET @PreGrId = 'GR' + RIGHT(CAST(YEAR(@GrDate) as nvarchar),2);
        SELECT @MaxGrId = MAX(GrId) FROM Gr WHERE LEFT(GrId,4) = @PreGrId;
        SET @MaxnGrId = SUBSTRING(ISNULL(@MaxGrId,0),5,4);
        SET @GrId = @PreGrId + REPLICATE('0', 4 - LEN(@MaxnGrId+1)) + CAST(@MaxnGrId+1 AS varchar);

        --* Gr Header *--
        INSERT INTO [Gr]
        ([GrId], [GrDate], [SupplierId], [RefNo], [SubTotal], [Ppn], [Total], [Notes]
        ,[GrStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [WfTransId])
        VALUES
            (@GrId, CAST(@GrDate as DATE), @SupplierId, @RefNo, @SubTotal, @Ppn, @Total, @Notes
            ,@GrStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, 0);

        SELECT SCOPE_IDENTITY() as RecId;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END
GO

