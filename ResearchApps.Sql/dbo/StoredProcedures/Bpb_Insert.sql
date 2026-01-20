CREATE PROCEDURE [dbo].[Bpb_Insert]
@BpbDate datetime,
@Descr nvarchar(50)='',
@RefType nvarchar(20)='',
@RefId nvarchar(20),
@Notes nvarchar(100)='', 
@BpbStatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @BpbId nvarchar(20), @PreBpbId nvarchar(20), @MaxBpbId nvarchar(20), @MaxnBpbId int 
	DECLARE @IsPpn bit
	
	--* Cek apakah Inventory Lock? *--
	IF EXISTS ( 
			SELECT * FROM InventLock
			WHERE [Year] = YEAR(@BpbDate) AND [Month] = MONTH(@BpbDate) AND Lock = 1
		)
	BEGIN
		SELECT -1 as RecId
		RETURN
	END

	--* Nomor Bpb Baru *--
	SET @PreBpbId = 'BPB' + RIGHT(CAST(YEAR(GETDATE()) as nvarchar),2)
	SELECT @MaxBpbId = MAX(BpbId) FROM Bpb WHERE LEFT(BpbId,5) = @PreBpbId
	SET @MaxnBpbId = SUBSTRING(ISNULL(@MaxBpbId,0),6,4)	
	SET @BpbId = @PreBpbId + REPLICATE('0', 4 - LEN(@MaxnBpbId+1)) + CAST(@MaxnBpbId+1 AS varchar)

	--* Bpb Header *--
	INSERT INTO [Bpb]
	([BpbId], [BpbDate], [Descr], [RefType], [RefId], Amount, [Notes]
	,[BpbStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@BpbId, CAST(@BpbDate as DATE), @Descr, @RefType, @RefId, 0, @Notes
	,@BpbStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT SCOPE_IDENTITY() as RecId, @BpbId as BpbId
END

GO

