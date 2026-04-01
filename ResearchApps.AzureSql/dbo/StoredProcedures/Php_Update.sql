CREATE PROCEDURE [dbo].[Php_Update]
@RecId int,
@PhpDate date,
@Descr nvarchar(200),
@RefId nvarchar(50),
@Notes nvarchar(100),
@ModifiedBy nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		UPDATE [Php]
		SET PhpDate = @PhpDate
			, Descr = @Descr
			, RefId = @RefId
			, Notes = @Notes
			, ModifiedBy = @ModifiedBy
			, ModifiedDate = GETDATE()
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
