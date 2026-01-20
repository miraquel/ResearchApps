CREATE PROCEDURE [dbo].[Po_RecallById]
	@RecId int,
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20)
DECLARE @PoStatusId int

BEGIN	

	DECLARE @WfTransId int

	IF EXISTS (SELECT WfTransId FROM Po WHERE RecId = @RecId AND PoStatusId = 4)
	BEGIN
		SELECT @WfTransId = WfTransId FROM Po WHERE RecId = @RecId AND PoStatusId = 4

		UPDATE [WfTrans]
		SET [WfStatusActionId] = 3
		WHERE [WfTransId] = @WfTransId

		UPDATE Po
		SET PoStatusId = 0
		WHERE RecId = @RecId
	END	
END

GO

