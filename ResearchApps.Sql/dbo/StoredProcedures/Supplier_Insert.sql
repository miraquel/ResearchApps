CREATE PROCEDURE [dbo].[Supplier_Insert]
@SupplierName nvarchar(100),
@Address nvarchar(200),
@City  nvarchar(50),
@Telp nvarchar(100), 
@Fax nvarchar(100),
@Email nvarchar(100), 
@TopId int, 
@IsPpn bit,
@Npwp nvarchar(20),
@Notes nvarchar(100),  
@StatusId int = 1,
@CreatedBy nvarchar(20)
AS
BEGIN
	DECLARE @SupplierId int
	
	--* Cek ada nama yg sama *--
	IF EXISTS ( 
			SELECT SupplierId FROM [Supplier]
			WHERE [SupplierName] = @SupplierName
		)
	BEGIN
		SELECT -1 as SupplierId
		RETURN
	END
	
	--* Proses Insert *--
	INSERT INTO [Supplier]
	([SupplierName], [Address], [City], [Telp], [Fax], [Email], [TopId], [IsPpn], [Npwp], [Notes] 
	,[StatusId], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@SupplierName, @Address, @City, @Telp, @Fax, @Email, @TopId, @IsPpn, @Npwp, @Notes
	,@StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @SupplierId = SCOPE_IDENTITY()

	SELECT @SupplierId as SupplierId
END

GO

