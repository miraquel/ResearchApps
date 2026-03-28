CREATE PROCEDURE [dbo].[Co_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @CoId nvarchar(20);

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Co WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Customer Order not found.', 1;
		END;

		IF (SELECT CoStatusId FROM Co WHERE RecId = @RecId) <> 0
		BEGIN
			THROW 50002, 'Only Draft Customer Orders can be deleted.', 1;
		END;

		SELECT @CoId = CoId FROM Co WHERE RecId = @RecId;

		--* Co Line *--
		DELETE FROM [CoLine]
		WHERE CoId = @CoId;

		--* Co Header *--
		DELETE FROM [Co]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
