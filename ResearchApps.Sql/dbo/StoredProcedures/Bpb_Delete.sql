CREATE PROCEDURE [dbo].[Bpb_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @BpbId nvarchar(20), @BpbLineId int

	SELECT @BpbId = BpbId FROM Bpb WHERE RecId = @RecId

	--* Bpb Line *--
	DECLARE x_cursor CURSOR FOR   
	SELECT BpbLineId  
	FROM [BpbLine]  
	WHERE BpbId = @BpbId  

	OPEN x_cursor  

	FETCH NEXT FROM x_cursor   
	INTO @BpbLineId  

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		EXEC [BpbLineDelete] @BpbLineId
		
		-- Get the next vendor.  
		FETCH NEXT FROM x_cursor   
		INTO @BpbLineId  
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor;



	--* Bpb Header *--
	DELETE FROM [Bpb]
	WHERE RecId = @RecId
END

GO

