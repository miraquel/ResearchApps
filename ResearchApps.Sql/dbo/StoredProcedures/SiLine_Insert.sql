CREATE PROCEDURE [dbo].[SiLine_Insert]
@RecId int, 
@DoLineId int,
@DoId nvarchar(20),
@ItemId int, 
@Qty numeric(32,16) = 0,
@Price numeric(32,16),
@Notes nvarchar(100)='', 
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @SiId nvarchar(20), @SiLineId int

	--* Init *--
	SELECT @SiId = SiId FROM Si WHERE RecId = @RecId

	--* Si Line *--
	INSERT INTO [SiLine]
	([SiId], [DoLineId], [DoId], [ItemId], [Qty], [Price], [Ppn], [Notes]
	  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@SiId, @DoLineId, @DoId, @ItemId, @Qty, @Price, 0, @Notes
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SELECT @SiLineId = SCOPE_IDENTITY()
	
	--Isi Harga dan PPN di SiLine
	UPDATE [SiLine]
	SET Price = c.Price
		,Ppn = c.Ppn
	FROM [SiLine] a
	JOIN [DoLine] b ON b.DoLineId = a.DoLineId
	JOIN [CoLine] c ON c.CoLineId = b.CoLineId
	WHERE a.SiLineId = @SiLineId

	
	--Isi Harga dan PPN di Si
	UPDATE [Si]
    SET Subtotal = b.Price
		,Ppn =  b.Ppn
		,Amount = b.Price+b.Ppn
	FROM [Si] a 
	JOIN (
		SELECT SiId
			,SUM(Qty*Price) as Price
			,SUM(Qty*Ppn) as Ppn
		FROM SiLine
		WHERE SiId = @SiId
		GROUP BY SiId
	) b ON b.SiId = a.SiId
    WHERE a.SiId = @SiId

	--Isi PoNo di Si
	UPDATE [Si]
    SET PoNo = e.PoCustomer
	FROM [Si] a 
	JOIN SiLine b ON b.SiLineId = @SiLineId	
	JOIN [DoLine] c ON c.DoLineId = b.DoLineId
	JOIN [CoLine] d ON d.CoLineId = c.CoLineId
	JOIN [Co] e ON e.CoId = d.CoId
    WHERE a.SiId = @SiId

	SELECT '1:::' + @SiId
END

GO

