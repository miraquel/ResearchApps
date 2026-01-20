CREATE PROCEDURE [dbo].[Php_Insert]
@PhpDate datetime,
@Descr nvarchar(50)='',
@RefId nvarchar(20),
@Notes nvarchar(100)='', 
@PhpStatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PhpId nvarchar(20), @PrePhpId nvarchar(20), @MaxPhpId nvarchar(20), @MaxnPhpId int 
	DECLARE @IsPpn bit
	
	--* Cek apakah Inventory Lock? *--
	IF EXISTS ( 
			SELECT * FROM InventLock
			WHERE [Year] = YEAR(@PhpDate) AND [Month] = MONTH(@PhpDate) AND Lock = 1
		)
	BEGIN
		SELECT -1 as RecId
		RETURN
	END

	--* Nomor Php Baru *--
	SET @PrePhpId = 'PHP' + RIGHT(CAST(YEAR(GETDATE()) as nvarchar),2)
	SELECT @MaxPhpId = MAX(PhpId) FROM Php WHERE LEFT(PhpId,5) = @PrePhpId
	SET @MaxnPhpId = SUBSTRING(ISNULL(@MaxPhpId,0),6,4)	
	SET @PhpId = @PrePhpId + REPLICATE('0', 4 - LEN(@MaxnPhpId+1)) + CAST(@MaxnPhpId+1 AS varchar)

	--* Php Header *--
	INSERT INTO [Php]
	([PhpId], [PhpDate], [Descr], RefId, Amount, [Notes]
	,[PhpStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@PhpId, CAST(@PhpDate as DATE), @Descr, @RefId, 0, @Notes
	,@PhpStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT SCOPE_IDENTITY() as RecId, @PhpId as PhpId
END

GO

