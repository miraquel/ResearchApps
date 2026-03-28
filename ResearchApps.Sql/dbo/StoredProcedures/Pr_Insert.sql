-- EXEC [Pr_Insert] '1 Nov 2025', 'Renovasi ruang IT', 1, '5 Nov 2025', 'Perluasan ruang kerjar', 0, 'system'
CREATE PROCEDURE [dbo].[Pr_Insert]
@PrDate datetime,
@PrName nvarchar(100)='',
@BudgetId int,
@RequestDate datetime,
@Notes nvarchar(100)='',
@PrStatusId int = 0,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PrId nvarchar(20), @PrePrId nvarchar(20), @MaxPrId nvarchar(20), @MaxnPrId int;

	BEGIN TRY
		--* Nomor PR Baru *--
		SET @PrePrId = 'FNA' + RIGHT(CAST(YEAR(@PrDate) as nvarchar),2) + 'PR-';
		SELECT @MaxPrId = MAX(PrId) FROM Pr WHERE LEFT(PrId,8) = @PrePrId;
		SET @MaxnPrId = SUBSTRING(ISNULL(@MaxPrId,0),9,4);
		SET @PrId = @PrePrId + REPLICATE('0', 4 - LEN(@MaxnPrId+1)) + CAST(@MaxnPrId+1 AS varchar);

		--* Pr Header *--
		INSERT INTO [Pr]
			([PrId], [PrDate], [PrName], [BudgetId], [RequestDate], [Total], [Notes]
			,[PrStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
			VALUES
			(@PrId, CAST(@PrDate as DATE), @PrName, @BudgetId, @RequestDate, 0, @Notes
			,@PrStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy);

		SELECT SCOPE_IDENTITY() as RecId, @PrId as PrId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
