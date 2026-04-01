--EXEC [Ps_Insert] '3 Mar 2026', 'test ID 37100', '', 1, 'admin'
CREATE PROCEDURE [dbo].[Ps_Insert]
@PsDate datetime,
@Descr nvarchar(50),
@Notes nvarchar(100)='',
@PsStatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PsId nvarchar(20), @PrePsId nvarchar(20), @MaxPsId nvarchar(20), @MaxnPsId int;
	DECLARE @IsPpn bit;

	--* Cek apakah Inventory Lock? *--
	IF EXISTS (
			SELECT * FROM InventLock
			WHERE [Year] = YEAR(@PsDate) AND [Month] = MONTH(@PsDate) AND Lock = 1
		)
	BEGIN
		SELECT -1 as RecId;
		RETURN;
	END

	BEGIN TRY
		--* Nomor Ps Baru *--
		SET @PrePsId = 'PS' + RIGHT(CAST(YEAR(GETDATE()) as nvarchar),2);
		SELECT @MaxPsId = MAX(PsId) FROM Ps WHERE LEFT(PsId,4) = @PrePsId;
		SET @MaxnPsId = SUBSTRING(ISNULL(@MaxPsId,0),5,4);
		SET @PsId = @PrePsId + REPLICATE('0', 4 - LEN(@MaxnPsId+1)) + CAST(@MaxnPsId+1 AS varchar);

		--* Ps Header *--
		INSERT INTO [Ps]
		([PsId], [PsDate], [Descr], Amount, [Notes]
		,[PsStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
		VALUES
		(@PsId, CAST(@PsDate as DATE), @Descr, 0, @Notes
		,@PsStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

		SELECT SCOPE_IDENTITY() as RecId, @PsId as PsId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
