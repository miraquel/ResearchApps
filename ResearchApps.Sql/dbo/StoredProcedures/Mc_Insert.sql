CREATE PROCEDURE [dbo].[Mc_Insert]
@McDate datetime,
@CustomerId int,
@SjNo nvarchar(50),
@RefNo nvarchar(50),
@Notes nvarchar(100), 
@McStatusId int = 1,
@CreatedBy nvarchar(20)
AS
BEGIN
	DECLARE @McId nvarchar(20), @PreMcId nvarchar(20), @MaxMcId nvarchar(20), @MaxnMcId int 
	DECLARE @IsPpn bit
	
	--* Cek apakah Inventory Lock? *--
	IF EXISTS ( 
			SELECT * FROM InventLock
			WHERE [Year] = YEAR(@McDate) AND [Month] = MONTH(@McDate) AND Lock = 1
		)
	BEGIN
		SELECT -1 as RecId
		RETURN
	END

	--* Nomor Mc Baru *--
	SET @PreMcId = 'MC' + RIGHT(CAST(YEAR(GETDATE()) as nvarchar),2)
	SELECT @MaxMcId = MAX(McId) FROM Mc WHERE LEFT(McId,4) = @PreMcId
	SET @MaxnMcId = SUBSTRING(ISNULL(@MaxMcId,0),5,4)	
	SET @McId = @PreMcId + REPLICATE('0', 4 - LEN(@MaxnMcId+1)) + CAST(@MaxnMcId+1 AS varchar)

	--* Mc Header *--
	INSERT INTO [Mc]
	([McId], [McDate], [CustomerId], [SjNo], [RefNo], [Notes]
	,[McStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@McId, CAST(@McDate as DATE), @CustomerId, @SjNo, @RefNo, @Notes
	,@McStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT SCOPE_IDENTITY() as RecId, @McId as McId
END

GO

