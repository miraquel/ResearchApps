CREATE PROCEDURE [dbo].[Gr_Insert]
@SupplierId int, 
@GrDate datetime,
@RefNo nvarchar(100), 
@SubTotal numeric(32,16) = 0, 
@Ppn numeric(32,16) = 0, 
@Total numeric(32,16) = 0, 
@Notes nvarchar(100), 
@GrStatusId int = 1,
@CreatedBy nvarchar(20)
AS
BEGIN
	DECLARE @GrId nvarchar(20), @PreGrId nvarchar(20), @MaxGrId nvarchar(20), @MaxnGrId int 

	--* Cek apakah Inventory Lock? *--
	IF EXISTS ( 
			SELECT * FROM InventLock
			WHERE [Year] = YEAR(@GrDate) AND [Month] = MONTH(@GrDate) AND Lock = 1
		)
	BEGIN
		SELECT -1 as RecId
		RETURN
	END
	
	--* Nomor GR Baru *--
	SET @PreGrId = 'GR' + RIGHT(CAST(YEAR(@GrDate) as nvarchar),2)
	SELECT @MaxGrId = MAX(GrId) FROM Gr WHERE LEFT(GrId,4) = @PreGrId
	SET @MaxnGrId = SUBSTRING(ISNULL(@MaxGrId,0),5,4)	
	SET @GrId = @PreGrId + REPLICATE('0', 4 - LEN(@MaxnGrId+1)) + CAST(@MaxnGrId+1 AS varchar)
	
	--* Gr Header *--
	INSERT INTO [Gr]
	([GrId], [GrDate], [SupplierId], [RefNo], [SubTotal], [Ppn], [Total], [Notes]
	,[GrStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@GrId, CAST(@GrDate as DATE), @SupplierId, @RefNo, @SubTotal, @Ppn, @Total, @Notes
	,@GrStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT SCOPE_IDENTITY() as RecId, @GrId as GrId
END

GO

