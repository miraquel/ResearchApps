CREATE PROCEDURE [dbo].[Co_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @CoId nvarchar(20)

	SELECT @CoId = CoId FROM Co WHERE RecId = @RecId

	--* Co Line *--
	DELETE FROM [CoLine]
	WHERE CoId = @CoId

	--* Co Header *--
	DELETE FROM [Co]
	WHERE RecId = @RecId
END

GO

