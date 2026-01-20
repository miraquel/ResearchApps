CREATE PROCEDURE [dbo].[Si_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @SiId nvarchar(20)

	SELECT @SiId = SiId FROM Si WHERE RecId = @RecId

	--* Si Line *--
	DELETE FROM [SiLine] WHERE SiId = @SiId

	--* Si Header *--
	DELETE FROM [Si] WHERE RecId = @RecId
END

GO

