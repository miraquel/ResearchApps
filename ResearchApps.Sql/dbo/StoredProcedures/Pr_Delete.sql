CREATE PROCEDURE [dbo].[Pr_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PrId nvarchar(20)

	SELECT @PrId = PrId FROM Pr WHERE RecId = @RecId

	--* Pr Line *--
	DELETE FROM [PrLine]
	WHERE PrId = @PrId

	--* Pr Header *--
	DELETE FROM [Pr]
	WHERE RecId = @RecId
END

GO

