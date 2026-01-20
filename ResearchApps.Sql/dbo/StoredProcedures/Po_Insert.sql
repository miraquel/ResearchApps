CREATE PROCEDURE [dbo].[Po_Insert]
@SupplierId int, 
@Pic nvarchar(20), 
@PoDate datetime,
@RefNo nvarchar(100), 
@SubTotal numeric(32,16) = 0, 
@Ppn numeric(32,16) = 0, 
@Total numeric(32,16) = 0, 
@Notes nvarchar(100), 
@PoStatusId int = 0,
@CreatedBy nvarchar(20)
AS
BEGIN
	DECLARE @PoId nvarchar(20), @PrePoId nvarchar(20), @MaxPoId nvarchar(20), @MaxnPoId int 
	DECLARE @IsPpn bit
	
	--* Nomor PO Baru *--
	SET @PrePoId = 'FNA' + RIGHT(CAST(YEAR(@PoDate) as nvarchar),2) + 'PO-'
	SELECT @MaxPoId = MAX(PoId) FROM Po WHERE LEFT(PoId,8) = @PrePoId
	SET @MaxnPoId = SUBSTRING(ISNULL(@MaxPoId,0),9,4)	
	SET @PoId = @PrePoId + REPLICATE('0', 4 - LEN(@MaxnPoId+1)) + CAST(@MaxnPoId+1 AS varchar)

	--* Ppn *--
	SELECT @IsPpn = IsPpn FROM Supplier WHERE SupplierId = @SupplierId
	
	--* Po Header *--
	INSERT INTO [Po]
	([PoId], [PoDate], [SupplierId], [Pic], [RefNo], [IsPpn], [SubTotal], [Ppn], [Total], [Notes]
	,[PoStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@PoId, CAST(@PoDate as DATE), @SupplierId, @Pic, @RefNo, @IsPpn, @SubTotal, @Ppn, @Total, @Notes
	,@PoStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT SCOPE_IDENTITY() as RecId, @PoId as PoId
END

GO

