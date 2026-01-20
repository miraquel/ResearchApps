CREATE PROCEDURE [dbo].[Prod_Insert]
@ProdDate datetime,
@Customer nvarchar(50)='',
@CustomerId int,
@ItemId int,
@PlanQty numeric(32,16),
@Notes nvarchar(100)='',
@ProdStatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @RecId int
	DECLARE @ProdId nvarchar(20), @PreProdId nvarchar(20), @MaxProdId nvarchar(20), @MaxnProdId int 
	
	--* Nomor PRO Baru *--
	SET @PreProdId = 'PRO' + RIGHT(CAST(YEAR(GETDATE()) as nvarchar),2)
	SELECT @MaxProdId = MAX(ProdId) FROM Prod WHERE LEFT(ProdId,5) = @PreProdId
	SET @MaxnProdId = SUBSTRING(ISNULL(@MaxProdId,0),6,4)	
	SET @ProdId = @PreProdId + REPLICATE('0', 4 - LEN(@MaxnProdId+1)) + CAST(@MaxnProdId+1 AS varchar)
	
	INSERT INTO [Prod]
	([ProdId], [ProdDate], [CustomerId], ItemId, [PlanQty], [Notes], [ProdStatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@ProdId, @ProdDate,  @CustomerId, @ItemId, @PlanQty, @Notes, @ProdStatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @RecId = SCOPE_IDENTITY()

	RETURN @RecId
END

GO

