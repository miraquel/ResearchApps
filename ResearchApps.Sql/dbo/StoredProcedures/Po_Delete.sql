CREATE PROCEDURE [dbo].[Po_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PoId nvarchar(20)

	SELECT @PoId = PoId FROM Po WHERE RecId = @RecId

	--* Po Line *--
	DELETE FROM [PoLine]
	WHERE PoId = @PoId

	--* Po Header *--
	DELETE FROM [Po]
	WHERE RecId = @RecId
END

GO

