CREATE PROCEDURE [dbo].[Pr_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PrId nvarchar(20);

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Pr WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Purchase Request not found.', 1;
		END;

		IF (SELECT PrStatusId FROM Pr WHERE RecId = @RecId) <> 0
		BEGIN
			THROW 50002, 'Only Draft Purchase Requests can be deleted.', 1;
		END;

		SELECT @PrId = PrId FROM Pr WHERE RecId = @RecId;

		--* Pr Line *--
		DELETE FROM [PrLine]
		WHERE PrId = @PrId;

		--* Pr Header *--
		DELETE FROM [Pr]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
