CREATE PROCEDURE [dbo].[Ps_Update]
@RecId int,
@Notes nvarchar(100)='',
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		UPDATE [Ps]
		SET Notes = @Notes
			, ModifiedBy = @ModifiedBy
			, ModifiedDate = GETDATE()
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
