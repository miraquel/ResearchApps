--EXEC Pr_RecallById 5,'thomas'
CREATE PROCEDURE [dbo].[Pr_RecallById]
	@RecId int,
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20)
DECLARE @PrStatusId int

BEGIN	

	DECLARE @WfTransId int

	IF EXISTS (SELECT WfTransId FROM Pr WHERE RecId = @RecId AND PrStatusId = 4)
	BEGIN
		SELECT @WfTransId = WfTransId FROM Pr WHERE RecId = @RecId AND PrStatusId = 4

		UPDATE [WfTrans]
		SET [WfStatusActionId] = 3
		WHERE [WfTransId] = @WfTransId

		UPDATE Pr
		SET PrStatusId = 0
		WHERE RecId = @RecId
	END	
END

GO

