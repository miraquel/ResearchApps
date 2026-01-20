CREATE PROCEDURE [dbo].[Si_Insert]
@SiDate datetime,
@CustomerId int = 0,
@PoNo nvarchar(50) = '',
@TaxNo nvarchar(50) = '',
@Notes nvarchar(100)='', 
@SiStatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @SiId nvarchar(20), @PreSiId nvarchar(20), @MaxSiId nvarchar(20), @MaxnSiId int 
	DECLARE @IsPpn bit
	
	--* Cek apakah Inventory Lock? *--
	IF EXISTS ( 
			SELECT * FROM InventLock
			WHERE [Year] = YEAR(@SiDate) AND [Month] = MONTH(@SiDate) AND Lock = 1
		)
BEGIN
SELECT -1 as RecId
    RETURN
END

--* Nomor Do Baru *--
SET @PreSiId = 'FNAINV' + RIGHT(CAST(YEAR(GETDATE()) as nvarchar),2)
SELECT @MaxSiId = MAX(SiId) FROM Si WHERE LEFT(SiId,8) = @PreSiId
SET @MaxnSiId = SUBSTRING(ISNULL(@MaxSiId,0),9,4)
SET @SiId = @PreSiId + REPLICATE('0', 4 - LEN(@MaxnSiId+1)) + CAST(@MaxnSiId+1 AS varchar)

--* Si Header *--
INSERT INTO [Si]
([SiId], [SiDate], [CustomerId], [PoNo], [TaxNo], [Subtotal], [Ppn], [Amount], [Notes]
       ,[SiStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
VALUES
    (@SiId, CAST(@SiDate as DATE), @CustomerId, @PoNo, @TaxNo, 0, 0, 0, @Notes
        ,1	--@SiStatusId
		, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

SELECT SCOPE_IDENTITY() as RecId, @SiId as SiId
END

GO

